using System.Text;
using System.Text.RegularExpressions;
using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

public sealed record CppConditionPropagationResult(CppSequence Body,int ForwardedUses,int RemovedDefinitions);

/// Forward total, pure predicate snapshots only while every operand retains
/// its value and lexical binding on every reaching path. Saved tests remain
/// when edge copies change an operand, including do/while backedges.
public static class CppConditionPropagation {
    const string Flag="branch_condition";
    static readonly HashSet<string> PrimitiveTypes=["bool","bits","std::uint8_t","std::uint16_t","std::uint32_t","std::uint64_t","u8*","const u8*"];
    static readonly HashSet<string> PureNames=["true","false","std","bool","bits","uint8_t","uint16_t","uint32_t","uint64_t","static_cast",
        "mask","narrow","sext","shift_left","shift_right","shift_arithmetic","rotate_right","signed_key","popcount","leading_zeroes","reverse_bits",
        "address","byte_offset","byte_pointer","const_byte_pointer"];
    sealed class Node {
        public string? Text;
        public HashSet<string> Writes=[];
        public HashSet<string> Dependencies=[];
        public List<int> Next=[];
        public bool Barrier,ReadsFlag,WritesFlag,Candidate,Condition;
        public string? Expression;
        public string? Replacement;
    }
    sealed class Occurrence(CppStatement statement) {
        public CppStatement Statement=statement;
        public List<Occurrence> Children=[];
        public int Entry,Node=-1;
    }
    sealed record Loop(int Test,int Exit,int ScopeDepth);

    public static CppConditionPropagationResult Reduce(CppSequence body,IReadOnlyDictionary<string,string> types,IReadOnlySet<string>? byValueCalls=null) {
        var scalar=types.Where(p=>PrimitiveTypes.Contains(p.Value)).Select(p=>p.Key).ToHashSet();
        var escaped=new HashSet<string>();
        bool valid=true; int count=0;
        Lexed Lex(string text) { var l=Tokenize(text);valid&=l.Valid;return l; }
        void Scan(CppStatement statement) {
            if(++count>16384) { valid=false;return; }
            if(statement is CppLine { Declares:{ } name } line) {
                // Only explicitly typed primitive locals are eligible operands.
                if(Regex.IsMatch(line.Text,@"^(?:\[\[maybe_unused\]\]\s*)?(?:const\s+)?(?:(?:bool|bits|std::uint(?:8|16|32|64)_t)\s+|u8\s*\*\s*(?:const\s+)?)"+Regex.Escape(name)+@"\b")) scalar.Add(name);
                if(name==Flag) valid=false;
            }
            foreach(string text in Texts(statement)) {
                var tokens=Lex(text).Tokens;
                if(statement is CppLine {Declares:{ } declared}) {
                    int at=Enumerable.Range(0,tokens.Count).FirstOrDefault(i=>tokens[i].Text==declared,tokens.Count);
                    if(tokens.Take(at).Any(t=>t.Text is "&" or "&&")) escaped.UnionWith(tokens.Where(t=>t.Identifier).Select(t=>t.Text));
                }
                for(int i=0;i+1<tokens.Count;i++) if(tokens[i].Identifier && tokens[i+1].Text=="(" &&
                    !PureNames.Contains(tokens[i].Text) && byValueCalls?.Contains(tokens[i].Text)!=true &&
                    tokens[i].Text is not "pointer" and not "address" and not "unsigned_divide" and not "signed_divide" and not "checked_flag" and not "if" and not "sizeof" and not "alignof" and not "decltype" and not "noexcept")
                    escaped.UnionWith(tokens.Where(t=>t.Identifier).Select(t=>t.Text));
                for(int i=0;i<tokens.Count;i++) if(tokens[i].Text==Flag &&
                    (i>0 && tokens[i-1].Text is "." or "->" or "::" || i+1<tokens.Count && tokens[i+1].Text=="::")) valid=false;
                if(tokens.Any(t=>t.Text is "goto" or "#" or "for" or "while" or "do" or "switch" or "try" or "catch" or "co_await" or "co_yield")) valid=false;
                if(tokens.Count>1 && (tokens[0].Text is "case" or "default" || tokens[0].Identifier && tokens[1].Text==":")) valid=false;
                if(tokens.Any(t=>t.Identifier && t.Text is "addressof" or "ref" or "cref") ||
                   Enumerable.Range(0,Math.Max(0,tokens.Count-1)).Any(i=>tokens[i].Text=="[" && tokens[i+1].Text=="&"))
                    escaped.UnionWith(tokens.Where(t=>t.Identifier).Select(t=>t.Text));
                for(int i=0;i<tokens.Count;i++) if(tokens[i].Text is "&" or "&&") {
                    int at=i+1;while(at<tokens.Count && tokens[at].Text=="(") at++;
                    if(at<tokens.Count && tokens[at].Identifier) escaped.Add(tokens[at].Text);
                }
            }
            foreach(var child in Children(statement)) Scan(child);
        }
        Scan(body);
        if(!valid || escaped.Contains(Flag)) return new(body,0,0);
        bool Pure(string expression,out HashSet<string> dependencies) {
            var tokens=Lex(expression).Tokens;
            dependencies=tokens.Where(t=>t.Identifier && scalar.Contains(t.Text)).Select(t=>t.Text).ToHashSet();
            if(expression.Contains('"') || expression.Contains('\'') || dependencies.Contains(Flag) || dependencies.Overlaps(escaped)) return false;
            if(tokens.Any(t=>t.Identifier && !scalar.Contains(t.Text) && !PureNames.Contains(t.Text))) return false;
            // No dereferences, addresses, division, subscripts or mutations.
            // Multiplication is conservatively excluded with unary dereference.
            if(tokens.Any(t=>t.Text is "*" or "/" or "%" or "[" or "]" or "." or "->" or "=" or "++" or "--" or ";" or "{" or "}" || t.Text.EndsWith('=') && t.Text is not "==" and not "!=" and not "<=" and not ">=")) return false;
            for(int i=0;i+1<tokens.Count;i++) if(tokens[i].Identifier && tokens[i+1].Text=="(" && !PureNames.Contains(tokens[i].Text)) return false;
            return valid && tokens.Count>0;
        }
        var nodes=new List<Node> {new()};
        int Add(Node n) { nodes.Add(n);return nodes.Count-1; }
        void Effects(Node node,string text,string? declares=null) {
            var tokens=Lex(text).Tokens;
            for(int i=0;i<tokens.Count;i++) {
                var t=tokens[i];
                if(t.Identifier && i+1<tokens.Count && tokens[i+1].Text is "=" or "+=" or "-=" or "*=" or "/=" or "%=" or "&=" or "|=" or "^=" or "++" or "--") node.Writes.Add(t.Text);
                if(t.Identifier && i>0 && tokens[i-1].Text is "++" or "--") node.Writes.Add(t.Text);
                if(t.Identifier && i+2<tokens.Count && tokens[i+1].Text is "<<" or ">>" && tokens[i+2].Text=="=") node.Writes.Add(t.Text);
                if(t.Text=="=" && (i==0 || !tokens[i-1].Identifier &&
                    !(i>=2 && tokens[i-1].Text is "<<" or ">>" && tokens[i-2].Identifier))) node.Barrier=true;
                if(t.Text is "++" or "--" && !(i>0 && tokens[i-1].Identifier || i+1<tokens.Count && tokens[i+1].Identifier)) node.Barrier=true;
                if(t.Text is "+=" or "-=" or "*=" or "/=" or "%=" or "&=" or "|=" or "^=" &&
                    (i==0 || !tokens[i-1].Identifier || !scalar.Contains(tokens[i-1].Text) || i>=2 && tokens[i-2].Text is "*" or "." or "->")) node.Barrier=true;
                if(t.Identifier && i+1<tokens.Count && tokens[i+1].Text=="(" && !PureNames.Contains(t.Text) && t.Text is not "pointer" and not "address") node.Barrier=true;
            }
            if(declares!=null) node.Writes.Add(declares);
            if(tokens.Any(t=>t.Text=="=") && node.Writes.Count==0) node.Barrier=true;
            node.WritesFlag=node.Writes.Contains(Flag);
            node.ReadsFlag=tokens.Any(t=>t.Identifier && t.Text==Flag);
        }
        int Kill(IEnumerable<string> names,int next) {
            var writes=names.ToHashSet();return writes.Count==0 ? next : Add(new() {Writes=writes,Next=[next]});
        }
        IEnumerable<string> Locals(CppSequence s) => s.Body.SelectMany(c=>c switch {
            CppLine { Declares:{ } n } => new[] {n},CppSequence nested=>Locals(nested),_=>[]
        });
        Occurrence Sequence(CppSequence s,int next,Loop? loop,IReadOnlyList<IReadOnlySet<string>> active,bool lexical) {
            var occurrence=new Occurrence(s);var locals=lexical ? Locals(s).ToHashSet() : [];
            var scopes=lexical ? active.Append((IReadOnlySet<string>)locals).ToArray() : active;
            int entry=Kill(locals,next);var children=new Occurrence[s.Body.Count];
            for(int i=s.Body.Count-1;i>=0;i--) { children[i]=Build(s.Body[i],entry,loop,scopes);entry=children[i].Entry; }
            occurrence.Entry=Kill(locals,entry);occurrence.Children.AddRange(children);return occurrence;
        }
        Node Test(string expression) {
            var node=new Node {Text=expression,Condition=true};Effects(node,expression);return node;
        }
        Occurrence Build(CppStatement statement,int next,Loop? loop,IReadOnlyList<IReadOnlySet<string>> active) {
            var o=new Occurrence(statement);
            switch(statement) {
                case CppSequence s:return Sequence(s,next,loop,active,false);
                case CppScope s: {
                    var child=Sequence(s.Body,next,loop,active,true);o.Children.Add(child);o.Entry=child.Entry;return o;
                }
                case CppIf conditional: {
                    var yes=Sequence(conditional.Then,next,loop,active,true);var no=Sequence(conditional.Else,next,loop,active,true);
                    var n=Test(conditional.Condition.Expression);n.Next=[yes.Entry,no.Entry];
                    o.Children.AddRange([yes,no]);o.Entry=o.Node=Add(n);return o;
                }
                case CppWhile w: {
                    int test=Add(Test(w.Condition));var child=Sequence(w.Body,test,new(test,next,active.Count),active,true);
                    nodes[test].Next=[child.Entry,next];o.Children.Add(child);o.Entry=o.Node=test;return o;
                }
                case CppDoWhile w: {
                    int test=Add(Test(w.Condition.Expression));var child=Sequence(w.Body,test,new(test,next,active.Count),active,true);
                    nodes[test].Next=[child.Entry,next];o.Children.Add(child);o.Entry=child.Entry;o.Node=test;return o;
                }
                case CppLine line: {
                    var n=new Node {Text=line.Text,Next=[next]};Effects(n,line.Text,line.Declares);
                    var match=Regex.Match(line.Text,@"^\s*branch_condition\s*=(?!=)\s*(.*);\s*$");
                    if(match.Success) {
                        n.Expression=match.Groups[1].Value;n.ReadsFlag=Lex(n.Expression).Tokens.Any(t=>t.Identifier && t.Text==Flag);
                        n.Candidate=line.IsConditionDefinition && Pure(n.Expression,out n.Dependencies);
                    }
                    var tokens=Lex(line.Text).Tokens;
                    if(tokens.Any(t=>t.Text is "break" or "continue")) {
                        if(tokens.Count!=2 || tokens[1].Text!=";" || loop==null) valid=false;
                        else n.Next=[Kill(active.Skip(loop.ScopeDepth).SelectMany(s=>s),tokens[0].Text=="break" ? loop.Exit : loop.Test)];
                    } else if(tokens.Count>0 && tokens[0].Text is "return" or "throw") n.Next=[];
                    o.Entry=o.Node=Add(n);return o;
                }
                default:valid=false;o.Entry=next;return o;
            }
        }
        var tree=Sequence(body,0,null,Array.Empty<IReadOnlySet<string>>(),true);
        if(!valid || nodes.Count>16384) return new(body,0,0);
        var reachable=new HashSet<int>();var pending=new Stack<int>();pending.Push(tree.Entry);
        while(pending.TryPop(out int i)) if(reachable.Add(i)) foreach(int target in nodes[i].Next) pending.Push(target);
        var predecessors=Enumerable.Range(0,nodes.Count).Select(_=>new List<int>()).ToArray();
        foreach(int i in reachable) foreach(int target in nodes[i].Next) predecessors[target].Add(i);
        var incoming=Enumerable.Repeat(-1,nodes.Count).ToArray();var outgoing=(int[])incoming.Clone();
        bool changed;long work=0;
        do {
            changed=false;
            foreach(int i in reachable.OrderDescending()) {
                if(++work>8_000_000) return new(body,0,0);
                int fact=-1;
                if(i!=tree.Entry && predecessors[i].Count>0) {
                    fact=outgoing[predecessors[i][0]];
                    if(predecessors[i].Any(p=>outgoing[p]!=fact)) fact=-1;
                }
                int result=fact;var n=nodes[i];
                if(result>=0 && (n.Barrier || n.Writes.Overlaps(nodes[result].Dependencies))) result=-1;
                if(n.WritesFlag) result=n.Candidate ? i : -1;
                if(incoming[i]!=fact || outgoing[i]!=result) { incoming[i]=fact;outgoing[i]=result;changed=true; }
            }
        } while(changed);
        int forwarded=0;
        foreach(int i in reachable) {
            var n=nodes[i];int definition=incoming[i];
            if(!n.Condition || !n.ReadsFlag || definition<0 || n.Barrier || n.Writes.Count>0) continue;
            var text=new StringBuilder();int start=0;
            foreach(var token in Lex(n.Text!).Tokens.Where(t=>t.Identifier && t.Text==Flag)) {
                text.Append(n.Text,start,token.Start-start);
                // A direct test supplies the bool conversion itself. Embedded
                // uses must preserve the original bool assignment conversion.
                if(n.Text==Flag) text.Append(Unwrap(nodes[definition].Expression!));
                else text.Append("bool(").Append(nodes[definition].Expression).Append(')');
                start=token.Start+token.Length;forwarded++;
            }
            text.Append(n.Text,start,n.Text!.Length-start);n.Replacement=text.ToString();n.ReadsFlag=false;
        }
        // Delete only tagged total evaluations whose saved boolean is now dead.
        var live=new bool[nodes.Count];
        do {
            changed=false;
            foreach(int i in reachable.Order()) {
                var n=nodes[i];bool value=n.ReadsFlag || !n.WritesFlag && n.Next.Any(t=>live[t]);
                if(live[i]!=value) { live[i]=value;changed=true; }
            }
        } while(changed);
        int removed=0;
        CppStatement Rewrite(Occurrence o) {
            var n=o.Node<0 ? null : nodes[o.Node];
            return o.Statement switch {
                CppSequence=>new CppSequence(o.Children.Select(Rewrite).Where(s=>s is not CppSequence {Body.Count:0}).ToArray()),
                CppScope s=>s with {Body=(CppSequence)Rewrite(o.Children[0])},
                CppIf c=>c with {Condition=c.Condition with {Expression=n!.Replacement??c.Condition.Expression},Then=(CppSequence)Rewrite(o.Children[0]),Else=(CppSequence)Rewrite(o.Children[1])},
                CppWhile w=>w with {Condition=n!.Replacement??w.Condition,Body=(CppSequence)Rewrite(o.Children[0])},
                CppDoWhile w=>w with {Condition=w.Condition with {Expression=n!.Replacement??w.Condition.Expression},Body=(CppSequence)Rewrite(o.Children[0])},
                CppLine when n!.Candidate && !n.Next.Any(t=>live[t])=>Remove(),
                _=>o.Statement
            };
        }
        CppStatement Remove() { removed++;return new CppSequence([]); }
        return new((CppSequence)Rewrite(tree),forwarded,removed);
    }
    static IEnumerable<string> Texts(CppStatement s)=>s switch {
        CppLine l=>[l.Text],CppIf c=>[c.Condition.Expression],CppWhile w=>[w.Condition],CppDoWhile w=>[w.Condition.Expression],_=>[]
    };
    static string Unwrap(string text) {
        text=text.Trim();
        while(text.StartsWith('(') && text.EndsWith(')')) {
            var lexed=Tokenize(text);if(!lexed.Valid) break;
            int depth=0;bool whole=true;
            for(int i=0;i<lexed.Tokens.Count-1;i++) {
                if(lexed.Tokens[i].Text=="(") depth++;
                if(lexed.Tokens[i].Text==")" && --depth==0) { whole=false;break; }
            }
            if(!whole) break;
            text=text[1..^1].Trim();
        }
        return text;
    }
    static IEnumerable<CppStatement> Children(CppStatement s)=>s switch {
        CppSequence q=>q.Body,CppScope q=>[q.Body],CppIf c=>[c.Then,c.Else],CppWhile w=>[w.Body],CppDoWhile w=>[w.Body],_=>[]
    };
}
