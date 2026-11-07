using System.Text;
using System.Text.RegularExpressions;
using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

public sealed record CppVariableLifetimesResult(CppSequence Body,IReadOnlyDictionary<string,string> Types,int SplitVariables,int AddedLifetimes);

/// Split compiler-owned storage into reaching-definition webs before placing
/// declarations. Reads unify every reaching definition, including loop/join
/// inputs. Effects and load snapshots stay at their original evaluation point.
/// This is storage renaming only: no statement, assignment or effect is removed.
public static class CppVariableLifetimes {
    sealed class Occurrence(CppStatement source,int order) {
        public readonly CppStatement Source=source;
        public readonly int Order=order;
        public readonly List<Occurrence> Children=[];
        public Lexed? Lexed;
        public string? Text;
        public int Node=-1;
    }
    sealed class Node {
        public HashSet<string> Reads=[];
        public string? Defines;
        public int Target=-1;
        public bool Compound;
        public List<int> Next=[];
        public int Order;
    }
    sealed record Loop(int Test,int Exit);
    sealed record Owner(IReadOnlyDictionary<int,string> Names,HashSet<int>[] Reaching,int[] Parent);
    static readonly HashSet<string> ValueCalls=["static_cast","reinterpret_cast","const_cast","address","pointer","byte_pointer","const_byte_pointer","byte_offset","mask","narrow","sext","shift_left","shift_right","shift_arithmetic",
        "rotate_right","signed_key","unsigned_divide","signed_divide","popcount","leading_zeroes","reverse_bits","checked_flag","std::size_t","std::ptrdiff_t"];

    public static CppVariableLifetimesResult Split(CppSequence body,IReadOnlyDictionary<string,string> types) {
        CppVariableLifetimesResult Unchanged()=>new(body,types,0,0);
        if(types.Count==0||types.Count>2048)return Unchanged();
        var blocked=new HashSet<string>();var identifiers=new HashSet<string>(types.Keys);bool valid=true;int scanned=0;
        var fragments=new Dictionary<string,Lexed>();
        Lexed Lex(string text){if(!fragments.TryGetValue(text,out var result))fragments[text]=result=Tokenize(text);if(!result.Valid)valid=false;return result;}
        HashSet<string> Names(IEnumerable<Token> tokens)=>tokens.Where(t=>t.Identifier&&types.ContainsKey(t.Text)).Select(t=>t.Text).ToHashSet();
        bool Mutation(IReadOnlyList<Token> tokens,out string? target,out int position,out bool compound){
            target=null;position=-1;compound=false;
            if(tokens.Count<3||tokens[^1].Text!=";"||tokens.SkipLast(1).Any(t=>t.Text==";"))return false;
            if(tokens[0].Identifier&&types.ContainsKey(tokens[0].Text)){
                if(tokens.Count>=4&&tokens[1].Text=="="){target=tokens[0].Text;position=0;return true;}
                if(tokens[1].Text is "+=" or "-=" or "*=" or "/=" or "%=" or "&=" or "|=" or "^=" || tokens.Count>=5&&tokens[1].Text is "<<" or ">>"&&tokens[2].Text=="=" || tokens.Count==3&&tokens[1].Text is "++" or "--"){
                    target=tokens[0].Text;position=0;compound=true;return true;
                }
            }
            if(tokens.Count==3&&tokens[0].Text is "++" or "--"&&tokens[1].Identifier&&types.ContainsKey(tokens[1].Text)){target=tokens[1].Text;position=1;compound=true;return true;}
            return false;
        }
        void Inspect(Occurrence occurrence,CppLine? line){
            var tokens=occurrence.Lexed!.Tokens;identifiers.UnionWith(tokens.Where(t=>t.Identifier).Select(t=>t.Text));
            if(tokens.Any(t=>t.Text is "goto" or "#" or "if" or "else" or "asm" or "__asm__" or "co_return" or "for" or "while" or "do" or "switch" or "try" or "catch" or "co_await" or "co_yield"))valid=false;
            if(tokens.Count>=2&&(tokens[0].Text is "case" or "default"||tokens[0].Identifier&&tokens[1].Text==":"))valid=false;
            if(tokens.Any(t=>t.Text is "addressof" or "ref" or "cref" or "decltype" or "typeid" or "sizeof" or "alignof"||t.Text.StartsWith("__builtin_",StringComparison.Ordinal)))blocked.UnionWith(Names(tokens));
            if(line?.Declares is { } declared&&types.ContainsKey(declared))blocked.Add(declared);
            if(line?.Declares is { } alias){
                int at=0;while(at<tokens.Count&&tokens[at].Text!=alias)at++;
                if(tokens.Take(at).Any(t=>t.Text is "&" or "&&"))blocked.UnionWith(Names(tokens));
            }
            string? target=null;int targetPosition=-1;bool compound=false;
            if(line!=null)Mutation(tokens,out target,out targetPosition,out compound);
            for(int i=0;i<tokens.Count;i++){
                var token=tokens[i];
                if(token.Text=="["){
                    int depth=1,close=i+1;for(;close<tokens.Count&&depth>0;close++){if(tokens[close].Text=="[")depth++;else if(tokens[close].Text=="]")depth--;}
                    if(depth==0&&close<tokens.Count&&tokens[close].Text is "(" or "{" or "mutable" or "noexcept")valid=false;
                    if(i+1<tokens.Count&&tokens[i+1].Text is "&" or "=" or "]")valid=false;
                }
                if(token.Text is ")" or "]"&&i+1<tokens.Count&&tokens[i+1].Text=="(")blocked.UnionWith(Names(tokens));
                if(!token.Identifier||!types.ContainsKey(token.Text))continue;
                if(i>0&&tokens[i-1].Text is "." or "->" or "::"||i+1<tokens.Count&&tokens[i+1].Text=="::")blocked.Add(token.Text);
                int previous=i-1;while(previous>=0&&tokens[previous].Text=="(")previous--;
                if(previous>=0&&tokens[previous].Text is "&" or "&&")blocked.Add(token.Text);
                bool writes=i+1<tokens.Count&&tokens[i+1].Text is "=" or "+=" or "-=" or "*=" or "/=" or "%=" or "&=" or "|=" or "^=" or "++" or "--" ||
                    i>0&&tokens[i-1].Text is "++" or "--" || i+2<tokens.Count&&tokens[i+1].Text is "<<" or ">>"&&tokens[i+2].Text=="=";
                if(writes&&i!=targetPosition)blocked.Add(token.Text);
            }
            // Opaque bare arguments may bind T& or a storage-capturing proxy.
            // Only the emitter-owned primitive casts/helpers are known by value.
            for(int i=0;i<tokens.Count;i++){
                if(!tokens[i].Identifier)continue;
                int open=i+1;string name=tokens[i].Text;
                if(i>0&&tokens[i-1].Text=="::")continue;
                while(open+1<tokens.Count&&tokens[open].Text=="::"&&tokens[open+1].Identifier){name+="::"+tokens[open+1].Text;open+=2;}
                if(open<tokens.Count&&tokens[open].Text=="<"){
                    int depth=1,begin=++open;
                    while(open<tokens.Count&&depth>0){if(tokens[open].Text=="<")depth++;else if(tokens[open].Text==">")depth--;else if(tokens[open].Text==">>")depth-=2;open++;}
                    if(depth!=0||open>=tokens.Count||tokens[open].Text!="(")continue;
                    if(tokens.Skip(begin).Take(open-begin).Any(t=>t.Text is "&" or "&&"))blocked.UnionWith(Names(tokens));
                }
                if(open>=tokens.Count||tokens[open].Text!="("||ValueCalls.Contains(name)||Trivial(name))continue;
                int level=1;
                for(int at=open+1;at<tokens.Count&&level>0;at++){
                    if(tokens[at].Text=="(")level++;else if(tokens[at].Text==")")level--;
                    if(level>0&&tokens[at].Identifier&&types.ContainsKey(tokens[at].Text))blocked.Add(tokens[at].Text);
                }
            }
        }
        Occurrence Scan(CppStatement statement,int depth){
            var occurrence=new Occurrence(statement,scanned++);if(scanned>16384||depth>192){valid=false;return occurrence;}
            switch(statement){
                case CppLine line:occurrence.Text=line.Text;occurrence.Lexed=Lex(line.Text);Inspect(occurrence,line);break;
                case CppSequence sequence:foreach(var child in sequence.Body)occurrence.Children.Add(Scan(child,depth+1));break;
                case CppScope scope:occurrence.Children.Add(Scan(scope.Body,depth+1));break;
                case CppIf conditional:occurrence.Text=conditional.Condition.Expression;occurrence.Lexed=Lex(occurrence.Text);Inspect(occurrence,null);occurrence.Children.Add(Scan(conditional.Then,depth+1));occurrence.Children.Add(Scan(conditional.Else,depth+1));break;
                case CppWhile loop:occurrence.Text=loop.Condition;occurrence.Lexed=Lex(occurrence.Text);Inspect(occurrence,null);occurrence.Children.Add(Scan(loop.Body,depth+1));break;
                case CppDoWhile loop:occurrence.Text=loop.Condition.Expression;occurrence.Lexed=Lex(occurrence.Text);Inspect(occurrence,null);occurrence.Children.Add(Scan(loop.Body,depth+1));break;
                default:valid=false;break;
            }
            return occurrence;
        }
        var root=Scan(body,0);foreach(var(name,type)in types)if(!Trivial(type))blocked.Add(name);if(!valid)return Unchanged();
        var nodes=new List<Node>{new()};
        int Add(Occurrence occurrence,Node node){node.Order=occurrence.Order;nodes.Add(node);return occurrence.Node=nodes.Count-1;}
        int Build(Occurrence occurrence,int next,Loop? loop){
            switch(occurrence.Source){
                case CppSequence:for(int i=occurrence.Children.Count-1;i>=0;i--)next=Build(occurrence.Children[i],next,loop);return next;
                case CppScope:return Build(occurrence.Children[0],next,loop);
                case CppIf:{int yes=Build(occurrence.Children[0],next,loop),no=Build(occurrence.Children[1],next,loop);return Add(occurrence,new(){Reads=Names(occurrence.Lexed!.Tokens),Next=[yes,no]});}
                case CppWhile or CppDoWhile:{int test=Add(occurrence,new(){Reads=Names(occurrence.Lexed!.Tokens)});int child=Build(occurrence.Children[0],test,new(test,next));nodes[test].Next=[child,next];return occurrence.Source is CppDoWhile?child:test;}
                case CppLine:{
                    var tokens=occurrence.Lexed!.Tokens;var node=new Node(){Reads=Names(tokens),Next=[next]};
                    if(Mutation(tokens,out var target,out int position,out bool compound)){
                        node.Defines=target;node.Target=position;node.Compound=compound;
                        if(!compound)node.Reads=Names(tokens.Skip(2).SkipLast(1));
                    }
                    if(tokens.Count==2&&tokens[1].Text==";"&&tokens[0].Text is "break" or "continue"){
                        if(loop==null){valid=false;node.Next=[];}else node.Next=[tokens[0].Text=="break"?loop.Exit:loop.Test];
                    }else if(tokens.Count!=0&&tokens[0].Text is "return" or "throw")node.Next=[];
                    else if(tokens.Any(t=>t.Text is "break" or "continue" or "return" or "throw")){valid=false;node.Next=[];}
                    return Add(occurrence,node);
                }
                default:valid=false;return next;
            }
        }
        int entry=Build(root,0,null);if(!valid||(long)nodes.Count*types.Count>2_000_000)return Unchanged();
        var reachable=new HashSet<int>();var traversal=new Stack<int>();traversal.Push(entry);while(traversal.TryPop(out int index))if(reachable.Add(index))foreach(int target in nodes[index].Next)traversal.Push(target);
        var plans=new Dictionary<string,Owner>();var updated=new Dictionary<string,string>(types);int split=0,added=0;long work=0;
        foreach(var(name,type)in types.OrderBy(p=>p.Key,StringComparer.Ordinal)){
            if(blocked.Contains(name))continue;
            var definitions=nodes.Select((node,index)=>(node,index)).Where(p=>p.node.Defines==name).Select(p=>p.index).ToArray();if(definitions.Length<2)continue;
            if(nodes.Select((node,index)=>(node,index)).Any(p=>!reachable.Contains(p.index)&&(p.node.Defines==name||p.node.Reads.Contains(name))))continue;
            var reaching=Enumerable.Range(0,nodes.Count).Select(_=>new HashSet<int>()).ToArray();
            var pending=new Queue<int>();var queued=new HashSet<int>{entry};reaching[entry].Add(0);pending.Enqueue(entry);
            while(pending.TryDequeue(out int index)){
                queued.Remove(index);var node=nodes[index];IEnumerable<int> outgoing=node.Defines==name?[index]:reaching[index];
                foreach(int successor in node.Next){
                    int before=reaching[successor].Count;reaching[successor].UnionWith(outgoing);work+=reaching[index].Count+1;
                    if(work>8_000_000)return Unchanged();
                    if(reaching[successor].Count!=before&&queued.Add(successor))pending.Enqueue(successor);
                }
            }
            if(reachable.Any(i=>nodes[i].Reads.Contains(name)&&reaching[i].Contains(0)))continue;
            var parent=Enumerable.Range(0,nodes.Count).ToArray();
            int Find(int i){if(parent[i]!=i)parent[i]=Find(parent[i]);return parent[i];}
            void Union(int a,int b){a=Find(a);b=Find(b);if(a!=b)parent[Math.Max(a,b)]=Math.Min(a,b);}
            foreach(int index in reachable)if(nodes[index].Reads.Contains(name)){
                var incoming=reaching[index].ToArray();for(int i=1;i<incoming.Length;i++)Union(incoming[0],incoming[i]);
                if(nodes[index].Compound&&nodes[index].Defines==name&&incoming.Length!=0)Union(index,incoming[0]);
            }
            var groups=definitions.GroupBy(Find).OrderBy(g=>g.Min(i=>nodes[i].Order)).ToArray();if(groups.Length<2)continue;
            var names=new Dictionary<int,string>();
            for(int i=0;i<groups.Length;i++){
                string replacement=name;
                if(i!=0){int suffix=0;do{replacement=name+"_lifetime_"+suffix++;}while(!identifiers.Add(replacement));updated[replacement]=type;}
                foreach(int definition in groups[i])names[definition]=replacement;
            }
            plans[name]=new(names,reaching,parent);split++;added+=groups.Length-1;
        }
        if(split==0)return Unchanged();
        string Rewrite(Occurrence occurrence){
            var result=new StringBuilder();int start=0;var node=nodes[occurrence.Node];var tokens=occurrence.Lexed!.Tokens;
            for(int i=0;i<tokens.Count;i++){
                var token=tokens[i];if(!token.Identifier||!plans.TryGetValue(token.Text,out var owner))continue;
                int definition;
                if(node.Defines==token.Text&&node.Target==i)definition=occurrence.Node;
                else{
                    var reaching=owner.Reaching[occurrence.Node];if(reaching.Count==0)continue;definition=reaching.First();
                }
                while(owner.Parent[definition]!=definition)definition=owner.Parent[definition];
                string replacement=owner.Names[definition];
                if(replacement==token.Text)continue;
                result.Append(occurrence.Text!,start,token.Start-start).Append(replacement);start=token.Start+token.Length;
            }
            return result.Append(occurrence.Text!,start,occurrence.Text!.Length-start).ToString();
        }
        CppStatement Replace(Occurrence occurrence)=>occurrence.Source switch{
            CppLine line=>line with{Text=Rewrite(occurrence)},
            CppSequence=>new CppSequence(occurrence.Children.Select(Replace).ToArray()),
            CppScope scope=>scope with{Body=(CppSequence)Replace(occurrence.Children[0])},
            CppIf conditional=>conditional with{Condition=conditional.Condition with{Expression=Rewrite(occurrence)},Then=(CppSequence)Replace(occurrence.Children[0]),Else=(CppSequence)Replace(occurrence.Children[1])},
            CppWhile loop=>loop with{Condition=Rewrite(occurrence),Body=(CppSequence)Replace(occurrence.Children[0])},
            CppDoWhile loop=>loop with{Condition=loop.Condition with{Expression=Rewrite(occurrence)},Body=(CppSequence)Replace(occurrence.Children[0])},_=>occurrence.Source
        };
        return new((CppSequence)Replace(root),updated,split,added);
    }
    static bool Trivial(string type){
        var lexed=Tokenize(type);if(!lexed.Valid||lexed.Tokens.Count==0||lexed.Tokens.Any(t=>t.Text is "volatile" or "&" or "&&"))return false;
        string plain=string.Join(" ",lexed.Tokens.Where(t=>t.Text is not "const" and not "*").Select(t=>t.Text)).Replace(" :: ","::",StringComparison.Ordinal);
        return Regex.IsMatch(plain,@"^(?:(?:std::)?(?:u?int(?:8|16|32|64)_t|size_t|ptrdiff_t)|u(?:8|16|32|64|128)|bits|bool|char|int|void|(?:unsigned|signed)(?:\s+(?:char|short|int|long|__int128)){0,2}|short|long(?:\s+long)?|__int128)$");
    }
}
