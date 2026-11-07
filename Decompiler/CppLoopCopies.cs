using System.Text.RegularExpressions;

namespace SharpRetro.Decompiler;

/// Speculate only emitter-owned, pure loop-back copies whose old destinations
/// are dead on the loop's false exit. Both liveness and definite initialization
/// are solved on the structured tree's control-flow graph, including enclosing
/// loops and exact break/continue targets. No copy is reordered or recombined.
public static class CppLoopCopies {
    sealed class Node {
        public HashSet<string> Reads=[];
        public HashSet<string> Defines=[];
        public HashSet<string> Kills=[];
        public string? Assigned;
        public HashSet<string> AssignmentReads=[];
        public List<int> Next=[];
    }
    sealed class Occurrence(CppStatement statement) {
        public CppStatement Statement=statement;
        public List<Occurrence> Children=[];
        public int Entry;
        public int FalseExit=-1;
    }
    sealed record Loop(int Test,int Exit,int ScopeDepth);

    public static CppSequence Simplify(CppSequence body,IReadOnlySet<string> carriers) {
        if(carriers.Count==0) return body;
        var nodes=new List<Node> { new() }; // normal function exit
        bool valid=true;
        var addressTaken=new HashSet<string>();
        HashSet<string> Names(string text) => Regex.Matches(text,@"[A-Za-z_]\w*").Select(m=>m.Value).Where(carriers.Contains).ToHashSet();
        void Addresses(string text) {
            foreach(Match match in Regex.Matches(text,@"(?:&\s*(?:\(\s*)?|(?:std::)?addressof\s*\(\s*)([A-Za-z_]\w*)"))
                if(carriers.Contains(match.Groups[1].Value)) addressTaken.Add(match.Groups[1].Value);
        }
        int Add(Node node) { nodes.Add(node); return nodes.Count-1; }
        int Kill(IEnumerable<string> names,int next) {
            var killed=names.ToHashSet();
            return killed.Count==0 ? next : Add(new() { Kills=killed,Next=[next] });
        }
        IEnumerable<string> Declared(CppSequence sequence) => sequence.Body.SelectMany(s=>s switch {
            CppLine { Declares:{ } name } when carriers.Contains(name) => new[] {name},
            CppSequence nested => Declared(nested),
            _ => []
        });
        bool Assignment(CppLine line,out string target,out string rhs) {
            var match=line.Declares is { } declared
                ? Regex.Match(line.Text,$@"\b{Regex.Escape(declared)}\s*=(?!=)\s*(.*);\s*$")
                : Regex.Match(line.Text,@"^\s*([A-Za-z_]\w*)\s*=(?!=)\s*(.*);\s*$");
            target=line.Declares ?? (match.Success ? match.Groups[1].Value : "");
            rhs=match.Success ? match.Groups[line.Declares==null ? 2 : 1].Value : "";
            return match.Success && carriers.Contains(target);
        }
        Occurrence Sequence(CppSequence sequence,int next,Loop? loop,IReadOnlyList<IReadOnlySet<string>> active,bool lexical) {
            var occurrence=new Occurrence(sequence);
            var locals=lexical ? Declared(sequence).ToHashSet() : new HashSet<string>();
            var scope=lexical ? active.Append((IReadOnlySet<string>)locals).ToArray() : active;
            int continuation=Kill(locals,next);
            // Occurrences, rather than statement references, distinguish copies
            // of the same immutable AST object in different lexical positions.
            var children=new Occurrence[sequence.Body.Count];
            for(int i=sequence.Body.Count-1;i>=0;i--) {
                children[i]=Build(sequence.Body[i],continuation,loop,scope);
                continuation=children[i].Entry;
            }
            occurrence.Children.AddRange(children);
            occurrence.Entry=Kill(locals,continuation);
            return occurrence;
        }
        Occurrence Build(CppStatement statement,int next,Loop? loop,IReadOnlyList<IReadOnlySet<string>> active) {
            var occurrence=new Occurrence(statement);
            switch(statement) {
                case CppSequence sequence:
                    return Sequence(sequence,next,loop,active,lexical:false);
                case CppScope scope: {
                    var child=Sequence(scope.Body,next,loop,active,lexical:true);
                    occurrence.Children.Add(child); occurrence.Entry=child.Entry; return occurrence;
                }
                case CppIf conditional: {
                    Addresses(conditional.Condition.Expression);
                    var yes=Sequence(conditional.Then,next,loop,active,lexical:true);
                    var no=Sequence(conditional.Else,next,loop,active,lexical:true);
                    occurrence.Children.Add(yes); occurrence.Children.Add(no);
                    occurrence.Entry=Add(new() { Reads=Names(conditional.Condition.Expression),Next=[yes.Entry,no.Entry] });
                    return occurrence;
                }
                case CppWhile conditional: {
                    Addresses(conditional.Condition);
                    int test=Add(new() { Reads=Names(conditional.Condition) });
                    var child=Sequence(conditional.Body,test,new(test,next,active.Count),active,lexical:true);
                    nodes[test].Next=[child.Entry,next];
                    occurrence.Children.Add(child); occurrence.Entry=test; occurrence.FalseExit=next; return occurrence;
                }
                case CppDoWhile conditional: {
                    Addresses(conditional.Condition.Expression);
                    int test=Add(new() { Reads=Names(conditional.Condition.Expression) });
                    var child=Sequence(conditional.Body,test,new(test,next,active.Count),active,lexical:true);
                    nodes[test].Next=[child.Entry,next];
                    occurrence.Children.Add(child); occurrence.Entry=child.Entry; occurrence.FalseExit=next; return occurrence;
                }
                case CppLine line: {
                    Addresses(line.Text);
                    var node=new Node() { Reads=Names(line.Text),Next=[next] };
                    if(Assignment(line,out string target,out string rhs)) {
                        node.Assigned=target; node.AssignmentReads=Names(rhs);
                        // Declarations can shadow an ancestor binding. Keep
                        // their liveness conservative; only plain assignments
                        // kill a prior carrier value in backward analysis.
                        if(line.Declares==null) { node.Reads=Names(rhs); node.Defines.Add(target); }
                    }
                    if(line.Declares is { } declared && carriers.Contains(declared)) node.Kills.Add(declared);
                    string text=line.Text.Trim();
                    // This pass consumes structured bodies only. A raw label
                    // or goto would need additional CFG edges; retain baseline
                    // output rather than treating it as ordinary fallthrough.
                    if(Regex.IsMatch(text,@"\bgoto\b|^(?:[A-Za-z_]\w*|case\b.*)\s*:(?!:)")) valid=false;
                    if(text is "break;" or "continue;") {
                        if(loop==null) { valid=false; node.Next=[]; }
                        else {
                            int destination=text=="break;" ? loop.Exit : loop.Test;
                            node.Next=[Kill(active.Skip(loop.ScopeDepth).SelectMany(s=>s),destination)];
                        }
                    } else if(text is "return;" || text.StartsWith("return ") || text.StartsWith("throw ")) node.Next=[];
                    occurrence.Entry=Add(node); return occurrence;
                }
                default:
                    valid=false; occurrence.Entry=next; return occurrence;
            }
        }
        var tree=Sequence(body,0,null,Array.Empty<IReadOnlySet<string>>(),lexical:true);
        if(!valid) return body;
        var reachable=new HashSet<int>(); var pending=new Stack<int>(); pending.Push(tree.Entry);
        while(pending.TryPop(out int index)) if(reachable.Add(index)) foreach(int target in nodes[index].Next) pending.Push(target);
        var predecessors=Enumerable.Range(0,nodes.Count).Select(_=>new List<int>()).ToArray();
        foreach(int index in reachable) foreach(int target in nodes[index].Next) predecessors[target].Add(index);

        var live=Enumerable.Range(0,nodes.Count).Select(_=>new HashSet<string>()).ToArray();
        bool changed;
        do {
            changed=false;
            foreach(int index in reachable.Order()) {
                var node=nodes[index];
                var next=node.Next.SelectMany(target=>live[target]).ToHashSet();
                next.ExceptWith(node.Defines); next.UnionWith(node.Reads);
                // Scope exits deliberately do not kill liveness: a declaration
                // with the same spelling could shadow an outer live carrier.
                if(!live[index].SetEquals(next)) { live[index]=next; changed=true; }
            }
        } while(changed);

        var initialized=Enumerable.Range(0,nodes.Count).Select(_=>carriers.ToHashSet()).ToArray();
        var after=Enumerable.Range(0,nodes.Count).Select(_=>carriers.ToHashSet()).ToArray();
        do {
            changed=false;
            foreach(int index in reachable.OrderDescending()) {
                var entry=index==tree.Entry ? new HashSet<string>() : carriers.ToHashSet();
                foreach(int predecessor in predecessors[index]) entry.IntersectWith(after[predecessor]);
                var node=nodes[index]; var exit=new HashSet<string>(entry);
                exit.ExceptWith(node.Kills);
                if(node.Assigned is { } assigned) {
                    exit.Remove(assigned);
                    if(node.AssignmentReads.IsSubsetOf(entry)) exit.Add(assigned);
                }
                if(!initialized[index].SetEquals(entry) || !after[index].SetEquals(exit)) {
                    initialized[index]=entry; after[index]=exit; changed=true;
                }
            }
        } while(changed);

        bool CanFlatten(Occurrence loop,CppDoWhile statement) {
            if(statement.Body.Body.LastOrDefault() is not CppIf guard || guard.Condition!=statement.Condition ||
               guard.Else.Body.Count!=0 || guard.Then.Body.Count==0) return false;
            var body=loop.Children[0];
            var tail=body.Children[^1];
            if(!reachable.Contains(tail.Entry)) return false;
            var ready=new HashSet<string>(initialized[tail.Entry]);
            var condition=Names(statement.Condition.Expression);
            foreach(var child in guard.Then.Body) {
                if(child is not CppLine { IsEdgeTransfer:true,Declares:null } line ||
                   !Assignment(line,out string target,out string rhs) || addressTaken.Contains(target) ||
                   condition.Contains(target) || live[loop.FalseExit].Contains(target)) return false;
                var reads=Names(rhs);
                if(!reads.IsSubsetOf(ready)) return false;
                // Preserve the already scheduled order, including RHS values
                // initialized by a preceding copy in this same bundle.
                ready.Add(target);
            }
            return true;
        }
        CppStatement Rewrite(Occurrence occurrence) {
            switch(occurrence.Statement) {
                case CppSequence:
                    return new CppSequence(occurrence.Children.Select(Rewrite).ToArray());
                case CppScope scope:
                    return scope with { Body=(CppSequence)Rewrite(occurrence.Children[0]) };
                case CppIf conditional:
                    return conditional with { Then=(CppSequence)Rewrite(occurrence.Children[0]),Else=(CppSequence)Rewrite(occurrence.Children[1]) };
                case CppWhile loop:
                    return loop with { Body=(CppSequence)Rewrite(occurrence.Children[0]) };
                case CppDoWhile loop: {
                    var rewritten=(CppSequence)Rewrite(occurrence.Children[0]);
                    if(CanFlatten(occurrence,loop) && rewritten.Body[^1] is CppIf guard)
                        rewritten=new([..rewritten.Body.SkipLast(1),..guard.Then.Body]);
                    return loop with { Body=rewritten };
                }
                default: return occurrence.Statement;
            }
        }
        return (CppSequence)Rewrite(tree);
    }
}
