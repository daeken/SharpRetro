namespace SharpRetro.Decompiler;

/// Place compiler-owned trivial SSA carriers in their smallest common lexical
/// scope. A single direct assignment can become a const declaration only when
/// there is no earlier use; nested/conditional definitions retain storage.
public static class CppVariableScopes {
    public static CppSequence Place(CppSequence body,IReadOnlyDictionary<string,string> variables) {
        var mentions=new Dictionary<CppStatement,HashSet<string>>(ReferenceEqualityComparer.Instance);
        HashSet<string> Mentions(CppStatement statement) {
            if(mentions.TryGetValue(statement,out var cached)) return cached;
            IEnumerable<string> Tokens(string text) => System.Text.RegularExpressions.Regex.Matches(text,@"[A-Za-z_]\w*")
                .Select(m=>m.Value).Where(variables.ContainsKey);
            var names=statement switch {
                CppLine l => Tokens(l.Text),
                CppSequence s => s.Body.SelectMany(Mentions),
                CppScope s => Mentions(s.Body),
                CppIf i => Tokens(i.Condition.Expression).Concat(Mentions(i.Then)).Concat(Mentions(i.Else)),
                CppWhile w => Tokens(w.Condition).Concat(Mentions(w.Body)),
                CppDoWhile w => Tokens(w.Condition.Expression).Concat(Mentions(w.Body)),
                _ => throw new NotSupportedException("Unknown variable scope statement")
            };
            return mentions[statement]=names.ToHashSet();
        }
        var placements=new Dictionary<CppSequence,List<string>>(ReferenceEqualityComparer.Instance);
        void Locate(CppSequence sequence,string name) {
            var uses=sequence.Body.Where(s=>Mentions(s).Contains(name)).ToArray();
            if(uses.Length==1) {
                CppSequence? nested=uses[0] switch {
                    CppScope s => s.Body,
                    CppSequence s => s,
                    CppIf i when !System.Text.RegularExpressions.Regex.IsMatch(i.Condition.Expression,$@"\b{name}\b") &&
                        Mentions(i.Then).Contains(name)!=Mentions(i.Else).Contains(name) => Mentions(i.Then).Contains(name) ? i.Then : i.Else,
                    // A do/while test is outside its body's lexical scope.
                    CppWhile w when !System.Text.RegularExpressions.Regex.IsMatch(w.Condition,$@"\b{name}\b") => w.Body,
                    CppDoWhile w when !System.Text.RegularExpressions.Regex.IsMatch(w.Condition.Expression,$@"\b{name}\b") => w.Body,
                    _ => null
                };
                if(nested!=null) { Locate(nested,name); return; }
            }
            if(uses.Length==0) return;
            if(!placements.TryGetValue(sequence,out var list)) placements[sequence]=list=[];
            list.Add(name);
        }
        foreach(var name in variables.Keys) Locate(body,name);

        CppSequence Rewrite(CppSequence sequence) {
            var declarations=new Dictionary<int,List<CppStatement>>();
            var replacements=new Dictionary<CppStatement,CppStatement>(ReferenceEqualityComparer.Instance);
            if(placements.TryGetValue(sequence,out var names)) foreach(var name in names) {
                var pattern=$@"^{name}\s*=(?!=)\s*(.*);$";
                var assignments=Lines(sequence).Where(l=>System.Text.RegularExpressions.Regex.IsMatch(l.Text,pattern)).ToArray();
                int firstIndex=Enumerable.Range(0,sequence.Body.Count).First(i=>Mentions(sequence.Body[i]).Contains(name));
                var first=sequence.Body[firstIndex];
                var fragments=Fragments(sequence).ToArray();
                bool addressTaken=fragments.Any(text=>System.Text.RegularExpressions.Regex.IsMatch(text,$@"&\s*(?:\(\s*)*{name}\b") ||
                    System.Text.RegularExpressions.Regex.IsMatch(text,@"\b(?:addressof|ref|cref)\b") &&
                    System.Text.RegularExpressions.Regex.IsMatch(text,$@"\b{name}\b")) || Lines(sequence).Any(l=>l.Declares!=null &&
                    l.Text[..System.Text.RegularExpressions.Regex.Match(l.Text,$@"\b{System.Text.RegularExpressions.Regex.Escape(l.Declares)}\b").Index].Contains('&') &&
                    System.Text.RegularExpressions.Regex.IsMatch(l.Text,$@"\b{name}\b"));
                string writes=$@"\b{name}\s*(?:=(?!=)|(?:<<|>>|[+\-*/%&|^])=|\+\+|--)|(?:\+\+|--)\s*\b{name}\b";
                int writeCount=fragments.Sum(text=>System.Text.RegularExpressions.Regex.Matches(text,writes).Count);
                if(assignments.Length!=0 && ReferenceEquals(first,assignments[0])) {
                    string rhs=System.Text.RegularExpressions.Regex.Match(assignments[0].Text,pattern).Groups[1].Value;
                    if(!System.Text.RegularExpressions.Regex.IsMatch(rhs,$@"\b{name}\b") && SingleRhs(rhs)) {
                        string qualifier=!addressTaken && writeCount==1 && assignments.Length==1 ? "const " : "";
                        string declaration=variables[name].EndsWith('*')
                            ? $"{variables[name]}{(qualifier.Length!=0 ? " const" : "")} {name}"
                            : $"{qualifier}{variables[name]} {name}";
                        replacements[first]=new CppLine($"[[maybe_unused]] {declaration} = {rhs};",name);
                        continue;
                    }
                }
                // Keep the established lexical scope, but shorten the trivial
                // carrier's lifetime to its first mentioning sibling. This
                // includes reads in conditions and assignment RHS expressions.
                if(!declarations.TryGetValue(firstIndex,out var before)) declarations[firstIndex]=before=[];
                before.Add(new CppLine($"[[maybe_unused]] {variables[name]} {name};",name));
            }
            CppStatement Child(CppStatement statement) {
                if(replacements.TryGetValue(statement,out var replacement)) return replacement;
                return statement switch {
                    CppSequence s => Rewrite(s),
                    CppScope s => s with { Body=Rewrite(s.Body) },
                    CppIf i => i with { Then=Rewrite(i.Then),Else=Rewrite(i.Else) },
                    CppWhile w => w with { Body=Rewrite(w.Body) },
                    CppDoWhile w => w with { Body=Rewrite(w.Body) },
                    _ => statement
                };
            }
            return new(Enumerable.Range(0,sequence.Body.Count).SelectMany(index=>
                (declarations.GetValueOrDefault(index) ?? []).Append(Child(sequence.Body[index]))).ToArray());
        }
        return Rewrite(body);
    }

    static bool SingleRhs(string rhs) {
        var lexed=CppLexical.Tokenize(rhs);if(!lexed.Valid) return false;
        int depth=0;
        foreach(var token in lexed.Tokens) {
            if(token.Text is "(" or "[" or "{") depth++;
            if(token.Text is ")" or "]" or "}") depth--;
            // `x=f(),g();` assigns f() to x, then evaluates g(). Turning it
            // into a declaration parses g as a declarator, not an RHS effect.
            if(depth==0 && token.Text is "," or ";") return false;
        }
        return depth==0;
    }

    static IEnumerable<CppLine> Lines(CppStatement statement) => statement switch {
        CppLine l => [l],
        CppSequence s => s.Body.SelectMany(Lines),
        CppScope s => Lines(s.Body),
        CppIf i => Lines(i.Then).Concat(Lines(i.Else)),
        CppWhile w => Lines(w.Body),
        CppDoWhile w => Lines(w.Body),
        _ => []
    };

    static IEnumerable<string> Fragments(CppStatement statement) => statement switch {
        CppLine l => [l.Text],
        CppSequence s => s.Body.SelectMany(Fragments),
        CppScope s => Fragments(s.Body),
        CppIf i => new[] {i.Condition.Expression}.Concat(Fragments(i.Then)).Concat(Fragments(i.Else)),
        CppWhile w => new[] {w.Condition}.Concat(Fragments(w.Body)),
        CppDoWhile w => new[] {w.Condition.Expression}.Concat(Fragments(w.Body)),
        _ => []
    };
}
