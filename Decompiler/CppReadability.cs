using System.Text.RegularExpressions;
using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

/// Late presentation of emitter-owned declarations. Earlier analyses retain
/// their original qualifiers. Pointee const, reference/opaque-call constraints,
/// constant-expression uses and all initializer evaluations remain intact.
public static class CppReadability {
    static readonly HashSet<string> ValueCalls=["return","static_cast","reinterpret_cast","const_cast","address","pointer","byte_pointer","const_byte_pointer","byte_offset",
        "mask","narrow","sext","shift_left","shift_right","shift_arithmetic","rotate_right","signed_key","unsigned_divide","signed_divide","popcount","leading_zeroes","reverse_bits","checked_flag"];

    public static CppSequence Clean(CppSequence body,IReadOnlySet<string>? byValueCalls=null) {
        var fragments=new List<(string Text,string? Declares)>();
        void Scan(CppStatement s) {
            switch(s) {
                case CppLine q:fragments.Add((q.Text,q.Declares));break;
                case CppSequence q:foreach(var child in q.Body)Scan(child);break;
                case CppScope q:Scan(q.Body);break;
                case CppIf q:fragments.Add((q.Condition.Expression,null));Scan(q.Then);Scan(q.Else);break;
                case CppWhile q:fragments.Add((q.Condition,null));Scan(q.Body);break;
                case CppDoWhile q:fragments.Add((q.Condition.Expression,null));Scan(q.Body);break;
                case CppFor q:fragments.Add((q.Initializer+";",q.Declares));fragments.Add((q.Condition,null));fragments.Add((q.Increment,null));Scan(q.Body);break;
            }
        }
        Scan(body);
        var names=fragments.Where(f=>f.Declares!=null).Select(f=>f.Declares!).ToHashSet();
        var definitions=fragments.Where(f=>f.Declares!=null).GroupBy(f=>f.Declares!).ToDictionary(g=>g.Key,g=>g.Count());
        var reads=names.ToDictionary(n=>n,_=>0);var blocked=new HashSet<string>();
        HashSet<string> Names(IEnumerable<Token> tokens)=>tokens.Where(t=>t.Identifier&&names.Contains(t.Text)).Select(t=>t.Text).ToHashSet();
        foreach(var (text,declaration) in fragments) {
            var lexed=Tokenize(text);if(!lexed.Valid)return body;var t=lexed.Tokens;
            string? assigned=declaration;
            if(assigned==null&&t.Count>2&&t[0].Identifier&&t[1].Text is "=" or "+=" or "-=" or "*=" or "/=" or "%=" or "&=" or "|=" or "^=" or "++" or "--")assigned=t[0].Text;
            if(assigned==null&&t.Count==3&&t[0].Text is "++" or "--"&&t[1].Identifier)assigned=t[1].Text;
            if(assigned==null&&t.Count>3&&t[0].Identifier&&t[1].Text is "<<" or ">>"&&t[2].Text=="=")assigned=t[0].Text;
            foreach(var token in t)if(token.Identifier&&reads.ContainsKey(token.Text)&&token.Text!=assigned)reads[token.Text]++;
            if(t.Any(t=>t.Text is "decltype" or "typeid" or "sizeof" or "alignof" or "alignas" or "noexcept" or "constexpr" or "consteval" or "static_assert" or "case" or "enum" or "static" or "requires" || t.Text.StartsWith("__builtin_",StringComparison.Ordinal)))blocked.UnionWith(Names(t));
            if(declaration!=null&&t.TakeWhile(t=>t.Text!=declaration).Any(t=>t.Text is "&" or "&&"))blocked.UnionWith(Names(t));
            if(declaration!=null) {
                int nameAt=Enumerable.Range(0,t.Count).FirstOrDefault(i=>t[i].Text==declaration,-1);
                if(nameAt>=0&&nameAt+1<t.Count&&t[nameAt+1].Text=="[")blocked.UnionWith(Names(t));
            }
            for(int i=0;i<t.Count;i++) {
                if(t[i].Text is "&" or "&&") {int at=i+1;while(at<t.Count&&t[at].Text=="(")at++;if(at<t.Count&&names.Contains(t[at].Text))blocked.Add(t[at].Text);}
                if(t[i].Text=="["&&i+1<t.Count&&t[i+1].Text is "&" or "=" or "]")blocked.UnionWith(names);
                if(t[i].Text is ")" or "]"&&i+1<t.Count&&t[i+1].Text=="(")blocked.UnionWith(Names(t));
                if(!t[i].Identifier||i>0&&t[i-1].Text=="::")continue;
                string call=t[i].Text;int open=i+1;
                while(open+1<t.Count&&t[open].Text=="::"&&t[open+1].Identifier){call+="::"+t[open+1].Text;open+=2;}
                if(open<t.Count&&t[open].Text=="<") {
                    int begin=open+1,depth=1;open++;
                    while(open<t.Count&&depth>0){if(t[open].Text=="<")depth++;else if(t[open].Text==">")depth--;else if(t[open].Text==">>")depth-=2;open++;}
                    if(depth!=0)continue;
                    blocked.UnionWith(Names(t.Skip(begin).Take(open-begin-1)));
                }
                if(open>=t.Count||t[open].Text!="("||ValueCalls.Contains(call)||Primitive(call)||byValueCalls?.Contains(call)==true)continue;
                int level=1;
                for(int at=open+1;at<t.Count&&level>0;at++) {
                    if(t[at].Text=="(")level++;else if(t[at].Text==")")level--;
                    if(level>0&&t[at].Identifier&&names.Contains(t[at].Text))blocked.Add(t[at].Text);
                }
            }
        }
        string Declaration(string text,string? name) {
            if(name==null)return text;
            var tokens=Tokenize(text).Tokens;int at=0;
            var edits=new List<(int Start,int Length)>();
            if(tokens.Count>4&&tokens[0].Text=="["&&tokens[1].Text=="["&&tokens[2].Text=="maybe_unused"&&tokens[3].Text=="]"&&tokens[4].Text=="]") {
                int end=tokens[4].Start+tokens[4].Length;while(end<text.Length&&char.IsWhiteSpace(text[end]))end++;
                if(definitions.GetValueOrDefault(name)==1&&reads.GetValueOrDefault(name)>0)edits.Add((tokens[0].Start,end-tokens[0].Start));at=5;
            }
            int declarator=Enumerable.Range(at,tokens.Count-at).FirstOrDefault(i=>tokens[i].Identifier&&tokens[i].Text==name,-1);
            if(declarator>at&&declarator+1<tokens.Count&&tokens[declarator+1].Text is "=" or ";" && definitions.GetValueOrDefault(name)==1&&!blocked.Contains(name)) {
                var type=tokens.Skip(at).Take(declarator-at).ToArray();
                if(Primitive(string.Join(" ",type.Select(t=>t.Text)))) {
                    int lastPointer=Array.FindLastIndex(type,t=>t.Text=="*");
                    for(int i=0;i<type.Length;i++)if(type[i].Text=="const"&&(lastPointer<0||i>lastPointer)) {
                        int end=type[i].Start+type[i].Length;while(end<text.Length&&char.IsWhiteSpace(text[end]))end++;
                        edits.Add((type[i].Start,end-type[i].Start));
                    }
                }
            }
            foreach(var edit in edits.OrderByDescending(e=>e.Start))text=text.Remove(edit.Start,edit.Length);
            return text;
        }
        CppStatement Rewrite(CppStatement s)=>s switch {
            CppLine q=>q with {Text=Statement(Declaration(q.Text,q.Declares),q.Declares!=null)},CppSequence q=>new CppSequence(q.Body.Select(Rewrite).ToArray()),CppScope q=>q with {Body=(CppSequence)Rewrite(q.Body)},
            CppIf q=>q with {Then=(CppSequence)Rewrite(q.Then),Else=(CppSequence)Rewrite(q.Else)},CppWhile q=>q with {Body=(CppSequence)Rewrite(q.Body)},CppDoWhile q=>q with {Body=(CppSequence)Rewrite(q.Body)},
            CppFor q=>q with {Initializer=Statement(Declaration(q.Initializer+";",q.Declares),q.Declares!=null).TrimEnd(';'),Body=(CppSequence)Rewrite(q.Body)},_=>s
        };
        return (CppSequence)Rewrite(body);
    }

    // Value context intentionally retains numeric bool conversions such as !!x.
    // Cast types and unknown/template/comment/literal expressions stay intact;
    // only understood operators and known value-call arguments lose parentheses.
    static string Value(string expression,int depth=0)=>depth>=24 ? expression : CppBooleanExpressions.Format(expression,false,
        integerComparisons:false,truthContext:false,formatAtom:atom=>Arguments(atom,depth+1));

    static string Arguments(string atom,int depth) {
        if(atom.Contains('"')||atom.Contains('\''))return atom;
        var lexed=Tokenize(atom);var t=lexed.Tokens;if(!lexed.Valid||lexed.Comments is {Count:>0}||t.Count<3||t[^1].Text!=")"||!t[0].Identifier)return atom;
        int open=1;string call=t[0].Text;
        while(open+1<t.Count&&t[open].Text=="::"&&t[open+1].Identifier){call+="::"+t[open+1].Text;open+=2;}
        int CastType(int index) {
            if(index<=0||t[index].Text!="<"||t[index-1].Text is not "static_cast" and not "reinterpret_cast" and not "const_cast")return -1;
            int end=index+1;while(end<t.Count&&t[end].Text!=">"&&t[end].Text is not "<" and not ">>")end++;
            return end<t.Count&&t[end].Text==">"&&Primitive(string.Join(" ",t.Skip(index+1).Take(end-index-1).Select(t=>t.Text))) ? end : -1;
        }
        if(open<t.Count&&t[open].Text=="<") {int end=CastType(open);if(end<0)return atom;open=end+1;}
        if(open>=t.Count||t[open].Text!="("||!ValueCalls.Contains(call)&&!Primitive(call))return atom;
        int nesting=0,start=t[open].Start+1;var pieces=new List<string>();
        for(int i=open+1;i<t.Count-1;i++) {
            if(t[i].Text=="<") {int end=CastType(i);if(end<0)return atom;i=end;continue;}
            if(t[i].Text==">")return atom;
            if(t[i].Text is "(" or "[" or "{")nesting++;
            if(t[i].Text is ")" or "]" or "}") {if(--nesting<0)return atom;}
            if(t[i].Text==","&&nesting==0){pieces.Add(Value(atom[start..t[i].Start].Trim(),depth));start=t[i].Start+1;}
        }
        if(nesting!=0)return atom;
        string last=atom[start..t[^1].Start].Trim();if(last.Length!=0)pieces.Add(Value(last,depth));
        return atom[..(t[open].Start+1)]+string.Join(", ",pieces)+")";
    }

    public static string Statement(string text,bool declaration=false) {
        var lexed=Tokenize(text);var t=lexed.Tokens;
        if(!lexed.Valid||lexed.Comments is {Count:>0}||t.Count<2||t[^1].Text!=";"||t.Count(t=>t.Text==";")!=1)return text;
        if(t[0].Text=="return"&&t.Count>2) {
            string expression=text[(t[0].Start+t[0].Length)..t[^1].Start].Trim();
            return "return "+Value(expression)+";";
        }
        int depth=0,assignment=-1;
        for(int i=0;i<t.Count;i++) {
            if(t[i].Text is "(" or "[" or "{")depth++;
            if(t[i].Text is ")" or "]" or "}")depth--;
            if(depth==0&&t[i].Text=="="){if(assignment>=0)return text;assignment=i;}
        }
        if(assignment>0) {
            string left=text[..t[assignment].Start].Trim();
            string right=text[(t[assignment].Start+1)..t[^1].Start].Trim();
            return (declaration ? left : Value(left))+" = "+Value(right)+";";
        }
        return text;
    }

    public static string Commas(string text) {
        var lexed=Tokenize(text);if(!lexed.Valid)return text;
        foreach(var token in lexed.Tokens.Reverse())if(token.Text==","&&token.Start+1<text.Length&&!char.IsWhiteSpace(text[token.Start+1]))text=text.Insert(token.Start+1," ");
        return text;
    }

    static bool Primitive(string type) {
        var tokens=Tokenize(type).Tokens;
        if(tokens.Any(t=>t.Text is "volatile" or "&" or "&&"))return false;
        string plain=string.Concat(tokens.Where(t=>t.Text is not "const" and not "*").Select(t=>t.Text));
        return Regex.IsMatch(plain,@"^(?:(?:std::)?(?:u?int(?:8|16|32|64)_t|size_t|ptrdiff_t)|bits|bool|u(?:8|16|32|64|128)|char|int|unsigned|unsignedint|unsignedlong|unsignedlonglong)$");
    }

    public static bool Mentions(CppStatement s,string name) {
        bool Text(string text)=>Tokenize(text).Tokens.Any(t=>t.Identifier&&t.Text==name);
        return s switch {
            CppLine q=>Text(q.Text),CppSequence q=>q.Body.Any(c=>Mentions(c,name)),CppScope q=>Mentions(q.Body,name),
            CppIf q=>Text(q.Condition.Expression)||Mentions(q.Then,name)||Mentions(q.Else,name),CppWhile q=>Text(q.Condition)||Mentions(q.Body,name),
            CppDoWhile q=>Text(q.Condition.Expression)||Mentions(q.Body,name),CppFor q=>Text(q.Initializer)||Text(q.Condition)||Text(q.Increment)||Mentions(q.Body,name),_=>false
        };
    }

    // This exact prelude binding only computes the address of real frame storage.
    public static string FrameAlias(string function) {
        var lexed=Tokenize(function);if(!lexed.Valid)return function;
        int uses=lexed.Tokens.Count(t=>t.Identifier&&t.Text=="frame_bytes");
        return Regex.Replace(function,@"(?m)^\s*\[\[maybe_unused\]\]\s*auto\* frame_bytes=reinterpret_cast<std::byte\*>\(&frame\);\r?\n",m=>uses==1?"":Regex.Replace(m.Value,@"\[\[maybe_unused\]\]\s*",""));
    }
}
