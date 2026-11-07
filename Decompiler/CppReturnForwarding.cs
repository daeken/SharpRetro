using System.Text.RegularExpressions;

namespace SharpRetro.Decompiler;

/// Forward a final carrier into an immediately adjacent return. Same-type copies
/// may also feed a pure return expression, including native pointer/address uses.
/// Arbitrary RHS forwarding still requires a bare returned integral carrier. With
/// the same exact C++ carrier/function return type, return initialization makes
/// the same conversion as the omitted assignment. Inner casts remain intact;
/// RHS evaluation stays at the same point and happens exactly once.
public static class CppReturnForwarding {
    sealed record Fragment(string Code,IReadOnlyList<(int Start,int Length)> Comments,bool Valid);
    static bool Integral(string type) => type is "bool" or "char" or "signed char" or "unsigned char" or
        "short" or "unsigned short" or "int" or "unsigned" or "unsigned int" or "long" or "unsigned long" or
        "long long" or "unsigned long long" or "std::int8_t" or "std::uint8_t" or "std::int16_t" or "std::uint16_t" or
        "std::int32_t" or "std::uint32_t" or "std::int64_t" or "std::uint64_t" or "std::intptr_t" or "std::uintptr_t" or
        "__int128" or "unsigned __int128" or "bits";

    public static CppSequence Reduce(CppSequence body,IReadOnlyDictionary<string,string> types,string returnType) {
        if(!Integral(returnType) || types.Count==0) return body;
        var fragments=new Dictionary<string,Fragment>();
        var escaped=new HashSet<string>();
        var declarations=new Dictionary<string,List<CppLine>>();
        var mentions=types.Keys.ToDictionary(name=>name,_=>0);
        bool valid=true;
        Fragment Read(string text) {
            if(!fragments.TryGetValue(text,out var fragment)) fragments[text]=fragment=Mask(text);
            valid&=fragment.Valid; return fragment;
        }
        IEnumerable<string> Names(string code) => Regex.Matches(code,@"[A-Za-z_]\w*").Select(m=>m.Value).Where(types.ContainsKey);
        bool ValueCall(string name) => Integral(name) || name is "static_cast" or "reinterpret_cast" or "const_cast" or
            "return" or "if" or "while" or "for" or "switch" or "catch" or "sizeof" or "alignof" or "decltype" or "noexcept" or
            "std::size_t" or "std::ptrdiff_t" or "address" or "pointer" or "mask" or "narrow" or "sext" or
            "shift_left" or "shift_right" or "shift_arithmetic" or "rotate_right" or "signed_key" or
            "unsigned_divide" or "signed_divide" or "popcount" or "leading_zeroes" or "reverse_bits" or "checked_flag";
        void Inspect(string text) {
            string code=Read(text).Code;
            foreach(var name in Names(code)) mentions[name]++;
            foreach(Match match in Regex.Matches(code,@"&\s*(?:\(\s*)*([A-Za-z_]\w*)"))
                if(types.ContainsKey(match.Groups[1].Value)) escaped.Add(match.Groups[1].Value);
            if(Regex.IsMatch(code,@"\b(?:addressof|ref|cref)\b")) escaped.UnionWith(Names(code));
            if(Regex.IsMatch(code,@"\[\s*&")) escaped.UnionWith(types.Keys);
            // A bare argument can bind to an opaque callee's T& or a proxy
            // that captures storage. Only the integral casts/runtime helpers
            // whose signatures are emitter-owned are known to pass by value.
            foreach(Match call in Regex.Matches(code,@"(?=\b(?<name>[A-Za-z_]\w*(?:::[A-Za-z_]\w*)*)(?:\s*<[^;{}]*?>)?\s*(?<open>\())")) {
                if(call.Index!=0 && code[call.Index-1]==':') continue;
                if(ValueCall(call.Groups["name"].Value)) continue;
                int start=call.Groups["open"].Index+1,end=start,depth=1;
                for(;end<code.Length && depth!=0;end++) {
                    if(code[end]=='(') depth++;
                    else if(code[end]==')') depth--;
                }
                escaped.UnionWith(Names(code[start..end]));
            }
            if(Regex.IsMatch(code,@"[)\]]\s*\(")) escaped.UnionWith(Names(code));
            if(Regex.IsMatch(code,@"\bgoto\b|^\s*(?:[A-Za-z_]\w*|case\b.*)\s*:(?!:)")) valid=false;
        }
        void Scan(CppStatement statement) {
            switch(statement) {
                case CppLine line:
                    Inspect(line.Text);
                    if(line.Declares is { } name && types.ContainsKey(name)) {
                        if(!declarations.TryGetValue(name,out var list)) declarations[name]=list=[];
                        list.Add(line);
                    } else if(line.Declares!=null && Read(line.Text).Code.Contains('&')) escaped.UnionWith(Names(Read(line.Text).Code));
                    break;
                case CppSequence sequence: foreach(var child in sequence.Body) Scan(child); break;
                case CppScope scope: Scan(scope.Body); break;
                case CppIf conditional: Inspect(conditional.Condition.Expression); Scan(conditional.Then); Scan(conditional.Else); break;
                case CppWhile loop: Inspect(loop.Condition); Scan(loop.Body); break;
                case CppDoWhile loop: Inspect(loop.Condition.Expression); Scan(loop.Body); break;
                default: valid=false; break;
            }
        }
        Scan(body);
        if(!valid) return body;

        bool ReturnedCarrier(CppLine returned,out string name) {
            name="";
            if(returned.Declares!=null) return false;
            var returnMatch=Regex.Match(Read(returned.Text).Code,@"^\s*return\s+([A-Za-z_]\w*)\s*;\s*$");
            if(!returnMatch.Success) return false;
            name=returnMatch.Groups[1].Value;
            return types.TryGetValue(name,out string? type) && type==returnType && !escaped.Contains(name);
        }
        bool Primitive(string type)=>Integral(type) || Regex.IsMatch(type,@"^(?:const\s+)?(?:u(?:8|16|32|64|128)|std::u?int(?:8|16|32|64)_t|void)\s*\*$");
        bool CopyIntoExpression(CppLine assignment,CppLine returned,out CppSequence replacement) {
            replacement=new([]);
            if(assignment.Declares!=null || returned.Declares!=null)return false;
            var copy=Regex.Match(Read(assignment.Text).Code,@"^\s*(?<to>[A-Za-z_]\w*)\s*=\s*(?<from>[A-Za-z_]\w*)\s*;\s*$");
            if(!copy.Success)return false;
            string target=copy.Groups["to"].Value,source=copy.Groups["from"].Value;
            if(target==source || escaped.Contains(target) || declarations.ContainsKey(target) ||
               !types.TryGetValue(target,out var type) || !Primitive(type) || types.GetValueOrDefault(source)!=type)return false;
            var lexed=CppLexical.Tokenize(returned.Text);if(!lexed.Valid)return false;
            var tokens=lexed.Tokens;
            if(tokens.Count<3 || tokens[0].Text!="return" || tokens[^1].Text!=";" || tokens.Count(t=>t.Text==";")!=1 ||
               !tokens.Skip(1).Any(t=>t.Identifier&&t.Text==target))return false;
            // Delaying a read through an unknown call, reference conversion or
            // side effect could observe a changed source. Keep that snapshot.
            if(tokens.Any(t=>t.Text is "=" or "+=" or "-=" or "*=" or "/=" or "%=" or "|=" or "&=" or "^=" or "++" or "--" or "." or "->" or "[" or "]" or "{" or "}" or
                "sizeof" or "alignof" or "decltype" or "typeid" or "noexcept" or "volatile" or "new" or "delete" or "throw"))return false;
            string code=Read(returned.Text).Code;
            foreach(Match call in Regex.Matches(code,@"(?=\b(?<name>[A-Za-z_]\w*(?:::[A-Za-z_]\w*)*)(?:\s*<[^;{}]*?>)?\s*\()")) {
                if(call.Index!=0&&code[call.Index-1]==':')continue;
                string name=call.Groups["name"].Value;
                if(!ValueCall(name))return false;
                // Primitive by-value casts cannot introduce a T& alias or
                // conversion whose behavior depends on local storage identity.
                if(name is "static_cast" or "reinterpret_cast" or "const_cast") {
                    int begin=code.IndexOf('<',call.Index),end=begin<0?-1:code.IndexOf('>',begin);
                    if(end<0 || !Primitive(code[(begin+1)..end].Trim()))return false;
                }
            }
            if(Regex.IsMatch(code,@"[)\]]\s*\("))return false;
            var output=new System.Text.StringBuilder();int start=0;
            foreach(var token in tokens)if(token.Identifier&&token.Text==target) {
                output.Append(returned.Text,start,token.Start-start).Append(source);start=token.Start+token.Length;
            }
            output.Append(returned.Text,start,returned.Text.Length-start);
            var comments=Read(assignment.Text).Comments.Select(c=>new CppLine(assignment.Text.Substring(c.Start,c.Length)));
            replacement=new([..comments,new CppLine(output.ToString())]);return true;
        }
        bool Forward(CppLine assignment,CppLine returned,out CppSequence replacement) {
            replacement=new([]);
            if(CopyIntoExpression(assignment,returned,out replacement))return true;
            if(!ReturnedCarrier(returned,out string name)) return false;
            string type=types[name];
            string code=Read(assignment.Text).Code;
            string prefix=assignment.Declares==null ? $@"^\s*{Regex.Escape(name)}\s*=(?!=)"
                : $@"^\s*(?:\[\[\s*maybe_unused\s*\]\]\s*)?(?:const\s+)?{Regex.Escape(type).Replace(" ",@"\s+")}\s+{Regex.Escape(name)}\s*=(?!=)";
            if(assignment.Declares!=null && assignment.Declares!=name) return false;
            var match=Regex.Match(code,prefix);
            if(!match.Success || code.Count(c=>c==';')!=1) return false;
            int end=code.LastIndexOf(';'),start=match.Index+match.Length;
            if(end<=start || !string.IsNullOrWhiteSpace(code[(end+1)..])) return false;
            if(Read(assignment.Text).Comments.Any(c=>c.Start<end && c.Start+c.Length>start &&
               assignment.Text.AsSpan(c.Start,c.Length).StartsWith("//",StringComparison.Ordinal))) return false;
            // A top-level comma would make this an expression sequence such as
            // `(x=f()),g()`, rather than the claimed single assignment RHS.
            var stack=new Stack<char>();
            foreach(char c in code[start..end]) {
                if(c is '(' or '[' or '{') stack.Push(c);
                else if(c is ')' or ']' or '}') {
                    if(!stack.TryPop(out char open) || (open,c) is not (('(',')') or ('[',']') or ('{','}'))) return false;
                } else if(c==',' && stack.Count==0) return false;
            }
            if(stack.Count!=0) return false;
            if(declarations.TryGetValue(name,out var declared)) {
                if(assignment.Declares==null || declared.Count!=1 || !ReferenceEquals(declared[0],assignment) || mentions[name]!=2) return false;
            } else if(assignment.Declares!=null) return false;
            string rhs=assignment.Text[start..end].Trim();
            if(rhs.Length==0) return false;
            var comments=Read(assignment.Text).Comments.Where(c=>c.Start+c.Length<=start || c.Start>=end)
                .Select(c=>new CppLine(assignment.Text.Substring(c.Start,c.Length)))
                .Concat(Read(returned.Text).Comments.Select(c=>new CppLine(returned.Text.Substring(c.Start,c.Length))));
            replacement=new([..comments,new CppLine($"return {rhs};")]); return true;
        }
        CppStatement Child(CppStatement statement) => statement switch {
            CppSequence sequence => Rewrite(sequence),
            CppScope scope => scope with { Body=Rewrite(scope.Body) },
            CppIf conditional => conditional with { Then=Rewrite(conditional.Then),Else=Rewrite(conditional.Else) },
            CppWhile loop => loop with { Body=Rewrite(loop.Body) },
            CppDoWhile loop => loop with { Body=Rewrite(loop.Body) },
            _ => statement
        };
        CppSequence Rewrite(CppSequence sequence) {
            var result=new List<CppStatement>();
            void Append(CppStatement statement) {
                result.Add(statement);
                // A fold can expose another adjacent same-type copy feeding
                // this return. Consume the existing ordered chain, never move
                // its RHS across an effect or duplicate its evaluation.
                while(result.Count>=2 && result[^2] is CppLine assignment && result[^1] is CppLine returned &&
                      Forward(assignment,returned,out var replacement)) {
                    result.RemoveRange(result.Count-2,2);
                    foreach(var child in replacement.Body) Append(child);
                }
            }
            for(int i=0;i<sequence.Body.Count;i++) {
                if(i+1<sequence.Body.Count && sequence.Body[i] is CppLine assignment && sequence.Body[i+1] is CppLine returned &&
                   Forward(assignment,returned,out var replacement)) {
                    foreach(var child in replacement.Body) Append(child); i++;
                } else if(i+1<sequence.Body.Count && sequence.Body[i] is CppIf conditional && sequence.Body[i+1] is CppLine exit &&
                          ReturnedCarrier(exit,out string name) && !declarations.ContainsKey(name)) {
                    // Moving an exact return into a normally completing arm
                    // crosses only compiler-owned trivial scopes. Abrupt arms
                    // retain their own return/throw/break/continue destination.
                    CppSequence Finish(CppSequence arm) => CppSourceTree.Terminates(arm) ? arm : new([..arm.Body,exit]);
                    Append(conditional with { Then=Rewrite(Finish(conditional.Then)),Else=Rewrite(Finish(conditional.Else)) }); i++;
                } else Append(Child(sequence.Body[i]));
            }
            return new(result);
        }
        return Rewrite(body);
    }

    // Preserve original indices while hiding literals/comments from matching.
    // A literal receives a harmless numeric marker so a sole character literal
    // still counts as an RHS expression; output always uses the original text.
    static Fragment Mask(string text) {
        var code=text.ToCharArray(); var comments=new List<(int Start,int Length)>();
        void Hide(int start,int end,bool literal=false) {
            Array.Fill(code,' ',start,end-start);
            if(literal && end>start) code[start]='0';
        }
        int i=0;
        while(i<text.Length) {
            if(text[i]=='/' && i+1<text.Length && text[i+1]=='/') {
                int newline=text.IndexOf('\n',i+2),end=newline<0 ? text.Length : newline,last=end-1;
                while(last>=i && text[last] is '\r' or ' ' or '\t') last--;
                if(last>=i && text[last]=='\\') return new(new string(code),comments,false);
                comments.Add((i,end-i)); Hide(i,end); i=end; continue;
            }
            if(text[i]=='/' && i+1<text.Length && text[i+1]=='*') {
                int end=text.IndexOf("*/",i+2,StringComparison.Ordinal);
                if(end<0) return new(new string(code),comments,false);
                end+=2; comments.Add((i,end-i)); Hide(i,end); i=end; continue;
            }
            string? prefix=new[] {"u8R\"","uR\"","UR\"","LR\"","R\"","u8\"","u\"","U\"","L\"","\"","u8'","u'","U'","L'","'"}
                .FirstOrDefault(p=>text.AsSpan(i).StartsWith(p,StringComparison.Ordinal));
            if(prefix!=null) {
                int quote=i+prefix.Length-1,end=-1;
                if(prefix.Contains('R')) {
                    int open=text.IndexOf('(',quote+1);
                    if(open<0 || open-quote-1>16) return new(new string(code),comments,false);
                    string delimiter=text[(quote+1)..open];
                    if(delimiter.Any(c=>char.IsWhiteSpace(c) || c is ')' or '\\')) return new(new string(code),comments,false);
                    string close=")"+delimiter+"\""; int found=text.IndexOf(close,open+1,StringComparison.Ordinal);
                    if(found>=0) end=found+close.Length;
                } else {
                    char terminator=text[quote]; int at=quote+1;
                    while(at<text.Length) {
                        if(text[at]=='\\') { at+=2; continue; }
                        if(text[at]==terminator) { end=at+1; break; }
                        if(text[at] is '\r' or '\n') break;
                        at++;
                    }
                }
                if(end<0) return new(new string(code),comments,false);
                while(end<text.Length && (char.IsLetterOrDigit(text[end]) || text[end]=='_')) end++;
                Hide(i,end,true); i=end; continue;
            }
            if(text[i]=='\\' || text[i]=='#') return new(new string(code),comments,false);
            // Consume an entire identifier/number before checking prefixes at
            // the next position, avoiding a literal prefix inside a name.
            if(char.IsLetterOrDigit(text[i]) || text[i]=='_' || text[i]>=128) {
                i++; while(i<text.Length && (char.IsLetterOrDigit(text[i]) || text[i]=='_' || text[i]>=128)) i++;
            } else i++;
        }
        return new(new string(code),comments,true);
    }
}
