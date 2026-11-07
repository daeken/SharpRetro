using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

public sealed record CppByteCopyLoopsResult(CppSequence Body,int Loops,int Copies);

/// Reroll exact forward byte-copy staircases, with their original unsigned stop
/// test and fixed unroll cap. Never replace sequential reads/writes with memcpy
/// or derive a trip count by subtracting addresses. All analyses requiring the
/// original instruction CFG precede this pass; sharing/presentation follow it.
public static class CppByteCopyLoops {
    sealed class Binding(string name,string type) {
        public readonly string Name=name,Type=type;
        public int Uses;
        public bool Escapes;
    }
    sealed record Affine(Binding Base,ulong Offset) {
        public string Numeric=>PointerType(Base.Type)?$"address({Base.Name})":Base.Name;
    }
    sealed record Pair(Affine Source,Affine Destination,Binding Value);
    static readonly HashSet<string> ValueCalls=["address","pointer","byte_offset","byte_pointer","const_byte_pointer","static_cast","reinterpret_cast","const_cast",
        "mask","narrow","sext","shift_left","shift_right","shift_arithmetic","rotate_right","signed_key","unsigned_divide","signed_divide","checked_flag"];
    static string Plain(string type)=>string.Concat(Tokenize(type).Tokens.Where(t=>t.Text!="const").Select(t=>t.Text));
    static bool WordType(string type)=>Plain(type)=="std::uint64_t";
    static bool PointerType(string type)=>Plain(type) is "u8*" or "std::uint8_t*";

    public static CppByteCopyLoopsResult Reduce(CppSequence input,IReadOnlyDictionary<string,string> types,IReadOnlySet<string>? byValueCalls=null) {
        int nodes=0;bool valid=true;
        CppStatement Clone(CppStatement s,int depth) {
            if(++nodes>16384||depth>192){valid=false;return s;}
            return s switch {
                CppLine q=>q with {},CppSequence q=>new CppSequence(q.Body.Select(c=>Clone(c,depth+1)).ToArray()),CppScope q=>q with {Body=(CppSequence)Clone(q.Body,depth+1)},
                CppIf q=>q with {Then=(CppSequence)Clone(q.Then,depth+1),Else=(CppSequence)Clone(q.Else,depth+1)},CppWhile q=>q with {Body=(CppSequence)Clone(q.Body,depth+1)},
                CppDoWhile q=>q with {Body=(CppSequence)Clone(q.Body,depth+1)},CppFor q=>q with {Body=(CppSequence)Clone(q.Body,depth+1)},_=>Invalid(s)
            };
        }
        CppStatement Invalid(CppStatement s){valid=false;return s;}
        var body=(CppSequence)Clone(input,0);if(!valid)return new(input,0,0);
        var declared=new HashSet<string>();
        IEnumerable<CppStatement> Children(CppStatement s)=>s switch {CppSequence q=>q.Body,CppScope q=>[q.Body],CppIf q=>[q.Then,q.Else],CppWhile q=>[q.Body],CppDoWhile q=>[q.Body],CppFor q=>[q.Body],_=>[]};
        void Inventory(CppStatement s){if(s is CppLine {Declares:{ } name})declared.Add(name);if(s is CppFor {Declares:{ } loopName})declared.Add(loopName);foreach(var c in Children(s))Inventory(c);}
        Inventory(body);
        var environment=types.Where(p=>!declared.Contains(p.Key)).ToDictionary(p=>p.Key,p=>new Binding(p.Key,p.Value));
        var scopes=new Dictionary<CppStatement,Dictionary<string,Binding>>(ReferenceEqualityComparer.Instance);
        var declarations=new Dictionary<CppLine,Binding>(ReferenceEqualityComparer.Instance);
        var identifiers=types.Keys.ToHashSet();
        void Fragment(string text,Dictionary<string,Binding> scope,string? declaration=null) {
            var lexed=Tokenize(text);if(!lexed.Valid){valid=false;return;}var t=lexed.Tokens;
            void Escape(IEnumerable<Token> tokens){foreach(var token in tokens)if(token.Identifier&&scope.TryGetValue(token.Text,out var b))b.Escapes=true;}
            foreach(var token in t)if(token.Identifier){identifiers.Add(token.Text);if(scope.TryGetValue(token.Text,out var b))b.Uses++;}
            if(t.Any(t=>t.Text is "goto" or "switch" or "case" or "try" or "catch" or "#"))valid=false;
            // Aggregate/reference constructors and opaque template arguments
            // need not expose an ampersand or a simple function-call shape.
            if(t.Any(t=>t.Text=="{"))Escape(t);
            if(declaration!=null&&t.TakeWhile(t=>t.Text!=declaration).Any(t=>t.Text is "&" or "&&"))Escape(t);
            for(int i=0;i<t.Count;i++) {
                if(t[i].Text is "&" or "&&") {int at=i+1;while(at<t.Count&&t[at].Text=="(")at++;if(at<t.Count&&scope.TryGetValue(t[at].Text,out var b))b.Escapes=true;}
                if(t[i].Text=="["&&i+1<t.Count&&t[i+1].Text is "&" or "=" or "]")Escape(t);
                if(t[i].Text is ")" or "]"&&i+1<t.Count&&t[i+1].Text=="(")Escape(t);
                if(!t[i].Identifier||i>0&&t[i-1].Text=="::")continue;
                int open=i+1;string call=t[i].Text;
                while(open+1<t.Count&&t[open].Text=="::"&&t[open+1].Identifier){call+="::"+t[open+1].Text;open+=2;}
                if(open<t.Count&&t[open].Text=="<"&&!ValueCalls.Contains(call)&&!WordType(call)&&byValueCalls?.Contains(call)!=true){Escape(t);continue;}
                if(open<t.Count&&t[open].Text=="<") {int level=1;open++;while(open<t.Count&&level>0){if(t[open].Text=="<")level++;else if(t[open].Text==">")level--;else if(t[open].Text==">>")level-=2;open++;}if(level!=0)continue;}
                if(open>=t.Count||t[open].Text!="("||ValueCalls.Contains(call)||WordType(call)||byValueCalls?.Contains(call)==true)continue;
                int depth=1,begin=open+1,atEnd=begin;
                for(;atEnd<t.Count&&depth>0;atEnd++){if(t[atEnd].Text=="(")depth++;else if(t[atEnd].Text==")")depth--;}
                Escape(t.Skip(begin).Take(atEnd-begin));
            }
        }
        void Scan(CppStatement s,Dictionary<string,Binding> scope) {
            switch(s) {
                case CppLine q:
                    if(q.Declares is { } name){var b=new Binding(name,types.GetValueOrDefault(name,"?"));scope[name]=b;declarations[q]=b;}
                    scopes[s]=new(scope);Fragment(q.Text,scope,q.Declares);break;
                case CppSequence q:foreach(var c in q.Body)Scan(c,scope);break;
                case CppScope q:Scan(q.Body,new(scope));break;
                case CppIf q:scopes[s]=new(scope);Fragment(q.Condition.Expression,scope);Scan(q.Then,new(scope));Scan(q.Else,new(scope));break;
                case CppWhile q:Fragment(q.Condition,scope);Scan(q.Body,new(scope));break;
                case CppDoWhile q:Scan(q.Body,new(scope));Fragment(q.Condition.Expression,scope);break;
                case CppFor q:
                    var nested=new Dictionary<string,Binding>(scope);
                    if(q.Declares is { } nameFor)nested[nameFor]=new(nameFor,types.GetValueOrDefault(nameFor,"?"));
                    Fragment(q.Initializer,nested,q.Declares);Fragment(q.Condition,nested);Fragment(q.Increment,nested);Scan(q.Body,new(nested));break;
            }
        }
        Scan(body,environment);if(!valid)return new(input,0,0);
        Token[] Lex(string text) {var l=Tokenize(text);return !l.Valid||l.Comments is {Count:>0}||text.Contains('"')||text.Contains('\'')?[]:l.Tokens.ToArray();}
        string Fresh(string prefix){int n=0;string name;do{name=prefix+n++;}while(!identifiers.Add(name));return name;}
        Affine? Address(Token[] original,CppStatement at) {
            var t=Strip(original);if(t.Length==0)return null;
            if(t.Length==1&&t[0].Identifier&&scopes[at].TryGetValue(t[0].Text,out var b)&&!b.Escapes&&(WordType(b.Type)||PointerType(b.Type)))return new(b,0);
            if(Call(t,"address") is { } arg &&Address(arg,at) is {Offset:0} p&&PointerType(p.Base.Type))return p;
            if(Call(t,"byte_offset") is { } offsetArgs&&Split(offsetArgs,",") is { } args&&Address(args.Left,at) is { } p2&&PointerType(p2.Base.Type)&&Literal(Strip(args.Right)) is { } k)return p2 with {Offset=unchecked(p2.Offset+k)};
            if(Split(t,"+") is { } add&&Address(add.Left,at) is { } baseAddress&&Literal(Strip(add.Right)) is { } constant)return baseAddress with {Offset=unchecked(baseAddress.Offset+constant)};
            return null;
        }
        Affine? Memory(Token[] original,CppStatement at,bool write) {
            var t=Strip(original);if(t.Length<2||t[0].Text!="*")return null;t=Strip(t[1..]);
            if(t.Length>5&&t[0].Text=="reinterpret_cast"&&t[1].Text=="<") {
                int end=Array.FindIndex(t,x=>x.Text==">");if(end<0)return null;
                string type=string.Concat(t.Skip(2).Take(end-2).Select(t=>t.Text));
                if(type!=(write?"u8*":"constu8*"))return null;
                var outer=Strip(t[(end+1)..]);
                if(Call(outer,"pointer") is not { } bits)return null;
                var a=Address(bits,at);return a!=null&&!PointerType(a.Base.Type)?a:null;
            }
            var pointer=Address(t,at);return pointer!=null&&PointerType(pointer.Base.Type)?pointer:null;
        }
        Pair? Copy(IReadOnlyList<CppStatement> statements,int index,ulong offset) {
            if(index+1>=statements.Count||statements[index] is not CppLine {Declares:{ } name} read||statements[index+1] is not CppLine {Declares:null} write)return null;
            var r=Lex(read.Text);if(r.Length==0||r[^1].Text!=";"||!declarations.TryGetValue(read,out var value)||value.Uses!=2||value.Escapes)return null;
            int eq=Array.FindIndex(r,t=>t.Text=="=");if(eq<1||r[eq-1].Text!=name)return null;
            string type=string.Concat(r.Take(eq-1).Select(t=>t.Text));if(type is not "conststd::uint8_t" and not "std::uint8_t")return null;
            var w=Lex(write.Text);if(w.Length<4||w[^1].Text!=";"||Split(w[..^1],"=") is not { } store||store.Right.Length!=1||store.Right[0].Text!=name)return null;
            var src=Memory(r[(eq+1)..^1],read,false);var dst=Memory(store.Left,write,true);
            return src?.Offset==offset&&dst?.Offset==offset ? new(src,dst,value) : null;
        }
        (Affine End,Affine Destination)? Guard(CppIf test,bool stop) {
            var t=Strip(Lex(test.Condition.Expression));string op=test.Condition.Negated ? (stop?">":"<=") : (stop?"<=":">");
            if(Split(t,op) is not { } parts)return null;
            var end=Address(parts.Left,test);var dest=Address(parts.Right,test);
            return end is {Offset:0}&&dest!=null ? (end,dest) : null;
        }
        bool Continuation(CppSequence q)=>q.Body.Count is >0 and <=8&&q.Body.All(s=>s is CppLine {Declares:null})&&q.Body[^1] is CppLine last&&CppSourceTree.Terminates(last);
        bool Equal(CppSequence a,CppSequence b)=>a.Body.Count==b.Body.Count&&a.Body.Zip(b.Body).All(p=>p.First is CppLine x&&p.Second is CppLine y&&x.Text==y.Text);
        bool Following(IReadOnlyList<CppStatement> source,int at,CppSequence c)=>at+c.Body.Count<=source.Count&&Equal(new(source.Skip(at).Take(c.Body.Count).ToArray()),c);
        bool Same(Pair a,Pair b)=>ReferenceEquals(a.Source.Base,b.Source.Base)&&ReferenceEquals(a.Destination.Base,b.Destination.Base);
        int loops=0,copies=0;
        CppSequence Rewrite(CppSequence source) {
            var result=new List<CppStatement>();
            for(int first=0;first<source.Body.Count;first++) {
                if(Copy(source.Body,first,0) is { } initial) {
                    int count=1,cursor=first+2;CppSequence? continuation=null;Affine? end=null;bool complete=false;
                    while(count<=64) {
                        if(continuation!=null&&Following(source.Body,cursor,continuation)){cursor+=continuation.Body.Count;complete=true;break;}
                        if(cursor>=source.Body.Count||source.Body[cursor] is not CppIf {Else.Body.Count:0} branch)break;
                        if(Guard(branch,true) is { } stop&&ReferenceEquals(stop.Destination.Base,initial.Destination.Base)&&stop.Destination.Offset==(ulong)count&&Continuation(branch.Then)) {
                            if(continuation!=null&&!Equal(continuation,branch.Then)||end!=null&&!ReferenceEquals(end.Base,stop.End.Base))break;
                            continuation??=branch.Then;end??=stop.End;
                            if(Copy(source.Body,cursor+1,(ulong)count) is not { } next||!Same(initial,next))break;
                            count++;cursor+=3;continue;
                        }
                        if(continuation!=null&&end!=null&&Guard(branch,false) is { } keep&&ReferenceEquals(keep.End.Base,end.Base)&&ReferenceEquals(keep.Destination.Base,initial.Destination.Base)&&keep.Destination.Offset==(ulong)count&&
                           branch.Then.Body.Count==2&&Copy(branch.Then.Body,0,(ulong)count) is { } final&&Same(initial,final)&&Following(source.Body,cursor+1,continuation)) {
                            count++;cursor+=1+continuation.Body.Count;complete=true;
                        }
                        break;
                    }
                    if(complete&&count is >=3 and <=64&&end!=null&&continuation!=null) {
                        string index=Fresh("copy_index_"),value=Fresh("copied_byte_");
                        string MemoryAt(Affine a,bool write)=>$"*reinterpret_cast<{(write?"":"const ")}u8*>(pointer({a.Numeric} + {index}))";
                        // The final extra bound comparison is total and reads
                        // only unexposed, nonvolatile primitive locals. The fixed
                        // cap preserves the original final unrolled step.
                        var loop=new CppFor($"std::uint64_t {index} = 0ULL",$"{index} < {count}ULL",$"++{index}",new([
                            new CppLine($"const std::uint8_t {value}={MemoryAt(initial.Source,false)};",value),
                            new CppLine($"{MemoryAt(initial.Destination,true)}={value};"),
                            new CppIf(new($"{end.Numeric} <= {initial.Destination.Numeric} + {index} + 1ULL"),new([new CppLine("break;")]),new([]))]),index);
                        result.Add(loop);result.AddRange(continuation.Body);first=cursor-1;loops++;copies+=count;continue;
                    }
                }
                result.Add(source.Body[first] switch {
                    CppSequence q=>Rewrite(q),CppScope q=>q with {Body=Rewrite(q.Body)},CppIf q=>q with {Then=Rewrite(q.Then),Else=Rewrite(q.Else)},
                    CppWhile q=>q with {Body=Rewrite(q.Body)},CppDoWhile q=>q with {Body=Rewrite(q.Body)},CppFor q=>q with {Body=Rewrite(q.Body)},var s=>s
                });
            }
            return new(result);
        }
        var output=Rewrite(body);return loops==0?new(input,0,0):new(output,loops,copies);
    }
    static Token[] Strip(Token[] t) {
        while(t.Length>=2&&t[0].Text=="("&&t[^1].Text==")") {
            int depth=0;bool wraps=true;for(int i=0;i<t.Length-1;i++){if(t[i].Text=="(")depth++;else if(t[i].Text==")")depth--;if(depth==0){wraps=false;break;}}
            if(!wraps)break;t=t[1..^1];
        }
        return t;
    }
    static Token[]? Call(Token[] t,string name) {
        if(t.Length<3||t[0].Text!=name||t[1].Text!="("||t[^1].Text!=")")return null;
        int depth=0;
        for(int i=1;i<t.Length;i++) {
            if(t[i].Text=="(")depth++;else if(t[i].Text==")")depth--;
            if(depth<0||depth==0&&i!=t.Length-1)return null;
        }
        return depth==0?t[2..^1]:null;
    }
    static (Token[] Left,Token[] Right)? Split(Token[] t,string op) {
        int depth=0,found=-1;
        for(int i=0;i<t.Length;i++){if(t[i].Text is "(" or "[")depth++;else if(t[i].Text is ")" or "]")depth--;else if(depth==0&&t[i].Text==op)found=i;if(depth<0)return null;}
        return depth==0&&found>0&&found<t.Length-1 ? (t[..found],t[(found+1)..]) : null;
    }
    static ulong? Literal(Token[] t) {
        if(t.Length!=1)return null;string text=t[0].Text;
        if(!text.EndsWith("ULL",StringComparison.OrdinalIgnoreCase))return null;text=text[..^3];
        if(text.StartsWith("0x",StringComparison.OrdinalIgnoreCase))return ulong.TryParse(text[2..],System.Globalization.NumberStyles.HexNumber,null,out var h)?h:null;
        if(text.Length>1&&text[0]=='0')return null;return ulong.TryParse(text,out var n)?n:null;
    }
}
