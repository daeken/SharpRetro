using System.Text;
using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

public sealed record CppCarrierCoalescingResult(CppSequence Body,IReadOnlyDictionary<string,string> Types,int Merged,int CopiesRemoved);

/// Move coalescing for compiler-owned, trivial, nonvolatile value carriers.
/// This consumes the already scheduled C++ copies, never the earlier parallel
/// assignment abstraction. Each merge must satisfy the original occurrence
/// CFG's interference constraints. No expression or effect is moved.
public static class CppCarrierCoalescing {
    sealed class Node {
        public HashSet<string> Reads=[];
        public string? Defines;
        public string? MoveSource;
        public List<int> Next=[];
    }
    sealed record Loop(int Test,int Exit);

    public static CppCarrierCoalescingResult Reduce(CppSequence body,IReadOnlyDictionary<string,string> types) {
        if(types.Count==0 || types.Count>2048) return new(body,types,0,0);
        var lexical=new Dictionary<string,Lexed>();
        bool valid=true;
        var blocked=new HashSet<string>();
        var moves=new List<(string Target,string Source)>();
        var bundles=new List<HashSet<string>>();
        int scanned=0;
        Lexed Lex(string text) {
            if(!lexical.TryGetValue(text,out var result)) lexical[text]=result=Tokenize(text);
            if(!result.Valid) valid=false;
            return result;
        }
        HashSet<string> Names(IEnumerable<Token> tokens) => tokens.Where(t=>t.Identifier && types.ContainsKey(t.Text)).Select(t=>t.Text).ToHashSet();
        bool PlainAssignment(IReadOnlyList<Token> tokens,out string target,out IReadOnlyList<Token> rhs) {
            target=""; rhs=[];
            if(tokens.Count<4 || !tokens[0].Identifier || !types.ContainsKey(tokens[0].Text) || tokens[1].Text!="=" || tokens[^1].Text!=";") return false;
            // Multiple raw statements in one line are outside this pass's CFG.
            if(tokens.Skip(2).SkipLast(1).Any(t=>t.Text==";")) return false;
            target=tokens[0].Text; rhs=tokens.Skip(2).SkipLast(1).ToArray(); return true;
        }
        bool Copy(CppLine line,out string target,out string source) {
            source="";
            var tokens=Lex(line.Text).Tokens;
            if(line.Declares!=null || !PlainAssignment(tokens,out target,out var rhs) || rhs.Count!=1 ||
               !rhs[0].Identifier || !types.ContainsKey(rhs[0].Text)) { target=""; return false; }
            source=rhs[0].Text; return true;
        }
        void Inspect(string text) {
            var tokens=Lex(text).Tokens;
            if(tokens.Any(t=>t.Text is "goto" or "#" or "for" or "while" or "do" or "switch" or "try" or "catch" or "co_await" or "co_yield")) valid=false;
            if(tokens.Count>=2 && (tokens[0].Text is "case" or "default" || tokens[0].Identifier && tokens[1].Text==":")) valid=false;
            // Explicit template arguments can separate a reference helper's
            // name from its argument; conservatively protect every carrier in
            // that fragment rather than parsing arbitrary template syntax.
            if(tokens.Any(t=>t.Identifier && t.Text is "addressof" or "ref" or "cref")) blocked.UnionWith(Names(tokens));
            for(int i=0;i+1<tokens.Count;i++) if(tokens[i].Text=="[" && tokens[i+1].Text=="&") valid=false;
            for(int i=0;i<tokens.Count;i++) {
                var token=tokens[i];
                if(!token.Identifier || !types.ContainsKey(token.Text)) continue;
                // Member/type spellings are not local variable bindings. Keep
                // the whole name out of renaming if such a collision occurs.
                if(i>0 && tokens[i-1].Text is "." or "->" or "::" || i+1<tokens.Count && tokens[i+1].Text=="::") blocked.Add(token.Text);
                int previous=i-1;
                while(previous>=0 && tokens[previous].Text=="(") previous--;
                if(previous>=0 && tokens[previous].Text is "&" or "&&" or "addressof" or "ref" or "cref") blocked.Add(token.Text);
            }
        }
        void Scan(CppStatement statement) {
            if(!valid) return;
            if(++scanned>16384) { valid=false; return; }
            switch(statement) {
                case CppLine line:
                    Inspect(line.Text);
                    var lineTokens=Lex(line.Text).Tokens;
                    if(lineTokens.Any(t=>t.Text is "break" or "continue") &&
                       !(lineTokens.Count==2 && lineTokens[1].Text==";" && lineTokens[0].Text is "break" or "continue")) valid=false;
                    // SSA storage declarations are inserted AFTER this pass.
                    // An earlier declaration may shadow an ancestor binding.
                    if(line.Declares is { } declared && types.ContainsKey(declared)) valid=false;
                    if(line.Declares!=null && Lex(line.Text).Tokens.Any(t=>t.Text is "&" or "&&")) blocked.UnionWith(Names(Lex(line.Text).Tokens));
                    if(line.IsEdgeTransfer && Copy(line,out var target,out var source) && types[target]==types[source]) moves.Add((target,source));
                    break;
                case CppSequence sequence: {
                    HashSet<string> targets=[];
                    void Finish() { if(targets.Count>1) bundles.Add(targets); targets=[]; }
                    foreach(var child in sequence.Body) {
                        if(!valid) return;
                        if(child is CppLine { IsEdgeTransfer:true,Declares:null } edge &&
                           PlainAssignment(Lex(edge.Text).Tokens,out var bundleTarget,out _)) targets.Add(bundleTarget);
                        else Finish();
                        Scan(child);
                    }
                    Finish(); break;
                }
                case CppScope scope: Scan(scope.Body); break;
                case CppIf conditional:
                    Inspect(conditional.Condition.Expression); Scan(conditional.Then); Scan(conditional.Else); break;
                case CppWhile loop: Inspect(loop.Condition); Scan(loop.Body); break;
                case CppDoWhile loop: Inspect(loop.Condition.Expression); Scan(loop.Body); break;
                default: valid=false; break;
            }
        }
        Scan(body);
        foreach(var (name,type) in types)
            if(type.Contains("volatile",StringComparison.Ordinal) || type.Contains("atomic",StringComparison.Ordinal)) blocked.Add(name);
        if(!valid || moves.Count==0) return new(body,types,0,0);

        var nodes=new List<Node> { new() };
        int Add(Node node) { nodes.Add(node); return nodes.Count-1; }
        int Build(CppStatement statement,int next,Loop? loop) {
            switch(statement) {
                case CppSequence sequence:
                    for(int i=sequence.Body.Count-1;i>=0;i--) next=Build(sequence.Body[i],next,loop);
                    return next;
                case CppScope scope: return Build(scope.Body,next,loop);
                case CppIf conditional: {
                    int yes=Build(conditional.Then,next,loop),no=Build(conditional.Else,next,loop);
                    return Add(new() { Reads=Names(Lex(conditional.Condition.Expression).Tokens),Next=[yes,no] });
                }
                case CppWhile conditional: {
                    int test=Add(new() { Reads=Names(Lex(conditional.Condition).Tokens) });
                    int child=Build(conditional.Body,test,new(test,next));
                    nodes[test].Next=[child,next]; return test;
                }
                case CppDoWhile conditional: {
                    int test=Add(new() { Reads=Names(Lex(conditional.Condition.Expression).Tokens) });
                    int child=Build(conditional.Body,test,new(test,next));
                    nodes[test].Next=[child,next]; return child;
                }
                case CppLine line: {
                    var tokens=Lex(line.Text).Tokens;
                    var node=new Node() { Reads=Names(tokens),Next=[next] };
                    if(PlainAssignment(tokens,out var target,out var rhs)) {
                        node.Defines=target; node.Reads=Names(rhs);
                        if(line.IsEdgeTransfer && Copy(line,out _,out var source) && types[target]==types[source]) node.MoveSource=source;
                    }
                    if(tokens.Count==2 && tokens[1].Text==";" && tokens[0].Text is "break" or "continue") {
                        if(loop==null) { valid=false; node.Next=[]; }
                        else node.Next=[tokens[0].Text=="break" ? loop.Exit : loop.Test];
                    } else if(tokens.Count!=0 && tokens[0].Text is "return" or "throw") node.Next=[];
                    return Add(node);
                }
                default: valid=false; return next;
            }
        }
        int entry=Build(body,0,null);
        if(!valid || (long)nodes.Count*types.Count>2_000_000) return new(body,types,0,0);
        var reachable=new HashSet<int>(); var pending=new Stack<int>(); pending.Push(entry);
        while(pending.TryPop(out int index)) if(reachable.Add(index)) foreach(int target in nodes[index].Next) pending.Push(target);
        var live=Enumerable.Range(0,nodes.Count).Select(_=>new HashSet<string>()).ToArray();
        bool changed;
        long work=0;
        do {
            changed=false;
            foreach(int index in reachable.Order()) {
                var node=nodes[index];
                work+=node.Reads.Count+node.Next.Sum(target=>live[target].Count)+1;
                if(work>8_000_000) return new(body,types,0,0);
                var incoming=node.Next.SelectMany(target=>live[target]).ToHashSet();
                if(node.Defines!=null) incoming.Remove(node.Defines);
                incoming.UnionWith(node.Reads);
                if(!incoming.SetEquals(live[index])) { live[index]=incoming; changed=true; }
            }
        } while(changed);
        var interference=types.Keys.ToDictionary(name=>name,_=>new HashSet<string>());
        void Conflict(string a,string b) {
            if(a==b) return;
            interference[a].Add(b); interference[b].Add(a);
        }
        foreach(int index in reachable) {
            var node=nodes[index];
            if(node.Defines is { } definition)
                foreach(var name in node.Next.SelectMany(target=>live[target]).Distinct())
                    if(name!=node.MoveSource) Conflict(definition,name);
            // Conservative simultaneous-live cliques also protect read/modify
            // statements not represented as a plain assignment (e.g. ++x),
            // and any values read at an external entry without a local def.
            var simultaneous=live[index].ToArray();
            work+=(long)simultaneous.Length*simultaneous.Length;
            if(work>8_000_000) return new(body,types,0,0);
            for(int a=0;a<simultaneous.Length;a++) for(int b=a+1;b<simultaneous.Length;b++) Conflict(simultaneous[a],simultaneous[b]);
        }

        var parent=types.Keys.ToDictionary(name=>name,name=>name);
        var members=types.Keys.ToDictionary(name=>name,name=>new HashSet<string> {name});
        string Find(string name) {
            if(parent[name]!=name) parent[name]=Find(parent[name]);
            return parent[name];
        }
        int merged=0;
        foreach(var move in moves) {
            string a=Find(move.Target),b=Find(move.Source);
            if(a==b || types[a]!=types[b] || members[a].Any(blocked.Contains) || members[b].Any(blocked.Contains) ||
               members[a].Any(x=>members[b].Any(y=>interference[x].Contains(y)))) continue;
            // The original schedule has distinct phi destinations. Do not
            // collapse two of those destinations within a copy bundle, even
            // when one happens to be dead in the present output.
            if(bundles.Any(bundle=>bundle.Any(x=>Find(x)==a) && bundle.Any(x=>Find(x)==b))) continue;
            string keep=StringComparer.Ordinal.Compare(a,b)<0 ? a : b,drop=keep==a ? b : a;
            parent[drop]=keep; members[keep].UnionWith(members[drop]); members.Remove(drop); merged++;
        }
        if(merged==0) return new(body,types,0,0);
        string Replace(string text) {
            var tokens=Lex(text).Tokens;
            var output=new StringBuilder(); int start=0;
            foreach(var token in tokens) {
                if(!token.Identifier || !parent.ContainsKey(token.Text)) continue;
                string replacement=Find(token.Text);
                if(replacement==token.Text) continue;
                output.Append(text,start,token.Start-start); output.Append(replacement); start=token.Start+token.Length;
            }
            output.Append(text,start,text.Length-start); return output.ToString();
        }
        int removed=0;
        CppStatement Rewrite(CppStatement statement) => statement switch {
            CppSequence sequence => new CppSequence(sequence.Body.Select(Rewrite).Where(s=>s is not CppSequence { Body.Count:0 }).ToArray()),
            CppScope scope => scope with { Body=(CppSequence)Rewrite(scope.Body) },
            CppIf conditional => conditional with { Condition=conditional.Condition with { Expression=Replace(conditional.Condition.Expression) },
                Then=(CppSequence)Rewrite(conditional.Then),Else=(CppSequence)Rewrite(conditional.Else) },
            CppWhile loop => loop with { Condition=Replace(loop.Condition),Body=(CppSequence)Rewrite(loop.Body) },
            CppDoWhile loop => loop with { Condition=loop.Condition with { Expression=Replace(loop.Condition.Expression) },Body=(CppSequence)Rewrite(loop.Body) },
            CppLine line => Line(line),
            _ => throw new NotSupportedException("Unknown carrier-coalescing statement")
        };
        CppStatement Line(CppLine line) {
            if(line.IsEdgeTransfer && Copy(line,out var target,out var source) && Find(target)==Find(source) && !blocked.Contains(target)) {
                removed++;
                var comments=Lex(line.Text).Comments;
                return comments is { Count:>0 } ? new CppLine(string.Join(" ",comments.Select(c=>line.Text.Substring(c.Start,c.Length)))) : new CppSequence([]);
            }
            return line with { Text=Replace(line.Text) };
        }
        var reduced=types.Keys.Select(Find).Distinct().ToDictionary(name=>name,name=>types[name]);
        return new((CppSequence)Rewrite(body),reduced,merged,removed);
    }

}
