using System.Text.RegularExpressions;
using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

public sealed record CppFor(string Initializer,string Condition,string Increment,CppSequence Body,string? Declares=null) : CppStatement;

/// Recognize headers without changing which edge executes an update. This late
/// pass runs after declaration placement and before closed-region factoring.
public static class CppLoopHeaders {
    public static CppSequence Rewrite(CppSequence source) {
        CppStatement Child(CppStatement s)=>s switch {
            CppSequence q=>Rewrite(q),CppScope q=>q with {Body=Rewrite(q.Body)},
            CppIf q=>Guard(q with {Then=Rewrite(q.Then),Else=Rewrite(q.Else)}),
            CppWhile q=>Top(q with {Body=Rewrite(q.Body)}),
            CppDoWhile q=>q with {Body=Rewrite(q.Body)},_=>s
        };
        var result=new List<CppStatement>();
        for(int i=0;i<source.Body.Count;i++) {
            var current=Child(source.Body[i]);
            CppWhile? loop=current as CppWhile;
            // A post-test loop needs a separate first-iteration proof. Only
            // literal scalar initialization plus a literal comparison qualifies.
            if(current is CppDoWhile post && result.LastOrDefault() is CppLine previous &&
               Initial(previous) is { } initial && EntryTrue(post.Condition,initial))
                loop=new(post.Condition.ToString(),post.Body);
            if(loop!=null && Step(loop.Body) is { } step && !HasContinue(new CppSequence(loop.Body.Body.SkipLast(1).ToArray()))) {
                var body=new CppSequence(loop.Body.Body.SkipLast(1).ToArray());
                // Moving the update out of the body must preserve every binding.
                var names=Names(step.Text).ToHashSet();
                if(!Declarations(body).Any(names.Contains) && TrivialLocals(body)) {
                    string initializer="";string? declaration=null;
                    if(result.LastOrDefault() is CppLine before && Initial(before) is { } init && init.Name==step.Name &&
                       (before.Declares==null || !source.Body.Skip(i+1).SelectMany(Fragments).SelectMany(Names).Contains(init.Name) &&
                        !Fragments(source).Any(text=>MayExpose(text,init.Name)))) {
                        initializer=before.Text.Trim().TrimEnd(';');declaration=before.Declares;result.RemoveAt(result.Count-1);
                    }
                    current=new CppFor(initializer,loop.Condition,step.Text.Trim().TrimEnd(';'),body,declaration);
                } else current=loop;
            } else if(loop!=null) current=loop;
            result.Add(current);
        }
        return new(result);
    }

    static CppWhile Top(CppWhile loop) {
        if(loop.Condition=="true" && loop.Body.Body.FirstOrDefault() is CppIf {Then.Body.Count:1,Else.Body.Count:0} guard &&
           guard.Then.Body[0] is CppLine {Text:"break;"})
            return new(CppBooleanExpressions.Format(guard.Condition.Expression,!guard.Condition.Negated,true),new(loop.Body.Body.Skip(1).ToArray()));
        return loop;
    }
    static CppStatement Guard(CppIf conditional) {
        // This exact guard already supplies the post-test loop's missing entry
        // test. Removing the wrapper preserves even the number of evaluations.
        if(conditional.Else.Body.Count==0 && conditional.Then.Body.Count==1 && conditional.Then.Body[0] is CppDoWhile post &&
           CppBooleanExpressions.Format(conditional.Condition.Expression,conditional.Condition.Negated,true)==
           CppBooleanExpressions.Format(post.Condition.Expression,post.Condition.Negated,true))
            return new CppWhile(CppBooleanExpressions.Format(post.Condition.Expression,post.Condition.Negated,true),post.Body);
        return conditional;
    }
    sealed record Init(string Name,string? Type,string Value);
    static Init? Initial(CppLine line) {
        var lexed=Tokenize(line.Text);if(!lexed.Valid || lexed.Comments is {Count:>0})return null;
        var t=lexed.Tokens;
        int eq=-1;for(int i=0;i<t.Count;i++)if(t[i].Text=="="){eq=i;break;}
        if(eq<1 || t[^1].Text!=";" || t.Count(x=>x.Text==";")!=1)return null;
        string name=t[eq-1].Text;
        if(!t[eq-1].Identifier || line.Declares!=null && line.Declares!=name || line.Declares==null && eq!=1)return null;
        string? type=line.Declares==null?null:line.Text[..t[eq-1].Start].Trim();
        if(type!=null && !Regex.IsMatch(Regex.Replace(type,@"\[\[[^\]]*\]\]",""),@"^\s*(?:(?:const\s+)?(?:std::)?(?:u?int(?:8|16|32|64)_t|int|unsigned|bits|u(?:8|16|32|64|128))|(?:const\s+)?u8\s*\*(?:\s*const)?)\s*$"))return null;
        string value=line.Text[(t[eq].Start+1)..t[^1].Start].Trim();
        return new(name,type,value);
    }
    static (string Name,string Text)? Step(CppSequence body) {
        if(body.Body.LastOrDefault() is not CppLine {Declares:null} line)return null;
        var lexed=Tokenize(line.Text);if(!lexed.Valid || lexed.Comments is {Count:>0})return null;
        string text=string.Concat(lexed.Tokens.Select(t=>t.Text));
        var update=Regex.Match(text,@"^(?:(?<n>[A-Za-z_]\w*)(?:\+\+|--)|(?:\+\+|--)(?<n>[A-Za-z_]\w*)|(?<n>[A-Za-z_]\w*)[+\-]=(?<c>(?:0x[0-9a-fA-F]+|[0-9]+)[uUlL]*)|(?<n>[A-Za-z_]\w*)=\k<n>[+\-](?<c>(?:0x[0-9a-fA-F]+|[0-9]+)[uUlL]*)|(?<n>[A-Za-z_]\w*)=byte_offset\(\k<n>,(?<c>(?:0x[0-9a-fA-F]+|[0-9]+)[uUlL]*)\));$");
        return update.Success?(update.Groups["n"].Value,line.Text):null;
    }
    static bool EntryTrue(CppCondition condition,Init initial) {
        // Restrict to positive small literals representable in every admitted
        // unsigned carrier. No pointer ordering, memory access or faulting test.
        if(initial.Type==null || initial.Type.Contains('*') || !Regex.IsMatch(initial.Value,@"^(?:0x[0-9a-fA-F]+|[0-9]+)[uUlL]*$"))return false;
        var match=Regex.Match(condition.Expression,@"^\s*"+Regex.Escape(initial.Name)+@"\s*(?<op>==|!=|<=|>=|<|>)\s*(?<v>(?:0x[0-9a-fA-F]+|[0-9]+)[uUlL]*)\s*$");
        if(!match.Success)return false;
        ulong? Literal(string s) {
            s=Regex.Replace(s,@"[uUlL]+$","");
            if(s.StartsWith("0x",StringComparison.OrdinalIgnoreCase))return ulong.TryParse(s[2..],System.Globalization.NumberStyles.HexNumber,null,out var h)?h:null;
            if(s.Length>1&&s[0]=='0')return null;
            return ulong.TryParse(s,out var d)?d:null;
        }
        var left=Literal(initial.Value);var right=Literal(match.Groups["v"].Value);
        if(left==null || right==null || left>127 || right>127)return false;
        bool test=match.Groups["op"].Value switch {"=="=>left==right,"!="=>left!=right,"<="=>left<=right,">="=>left>=right,"<"=>left<right,">"=>left>right,_=>false};
        return condition.Negated?!test:test;
    }
    static IEnumerable<string> Names(string text)=>Tokenize(text).Tokens.Where(t=>t.Identifier).Select(t=>t.Text);
    static bool TrivialLocals(CppStatement statement) {
        if(statement is CppLine {Declares:{ } name} line) {
            var lexed=Tokenize(Regex.Replace(line.Text,@"\[\[[^\]]*\]\]",""));if(!lexed.Valid)return false;
            var prefix=lexed.Tokens.TakeWhile(t=>t.Text!=name).Select(t=>t.Text).ToArray();
            string type=string.Concat(prefix.Where(t=>t is not "const" and not "*"));
            return Regex.IsMatch(type,@"^(?:std::)?(?:u?int(?:8|16|32|64)_t|u(?:8|16|32|64|128)|bits|bool|char|int|unsigned|unsignedint|unsignedlong|unsignedlonglong)$");
        }
        return statement switch {
            CppSequence q=>q.Body.All(TrivialLocals),CppScope q=>TrivialLocals(q.Body),CppIf q=>TrivialLocals(q.Then)&&TrivialLocals(q.Else),
            CppWhile q=>TrivialLocals(q.Body),CppDoWhile q=>TrivialLocals(q.Body),CppFor q=>TrivialLocals(q.Body),_=>true
        };
    }
    static bool MayExpose(string text,string name) {
        var lexed=Tokenize(Regex.Replace(text,@"\[\[[^\]]*\]\]",""));if(!lexed.Valid)return true;
        var t=lexed.Tokens;
        if(!t.Any(x=>x.Identifier&&x.Text==name))return false;
        if(t.Any(x=>x.Text is "&" or "addressof" or "ref" or "cref" or "decltype"))return true;
        // Unknown calls can bind a primitive lvalue by reference. Keeping its
        // declaration outside the loop preserves storage for such observers.
        var byValue=new HashSet<string>{"address","byte_offset","byte_pointer","const_byte_pointer","static_cast","reinterpret_cast","const_cast","mask","narrow","sext","shift_left","shift_right","shift_arithmetic"};
        for(int i=0;i+1<t.Count;i++)if(t[i].Identifier && t[i+1].Text=="(" && !byValue.Contains(t[i].Text))return true;
        return false;
    }
    static IEnumerable<string> Declarations(CppStatement s)=>s switch {
        CppLine {Declares:{ } name}=>[name],CppSequence q=>q.Body.SelectMany(Declarations),CppScope q=>Declarations(q.Body),
        CppIf q=>Declarations(q.Then).Concat(Declarations(q.Else)),CppWhile q=>Declarations(q.Body),CppDoWhile q=>Declarations(q.Body),CppFor q=>Declarations(q.Body),_=>[]
    };
    static IEnumerable<string> Fragments(CppStatement s)=>s switch {
        CppLine q=>[q.Text],CppSequence q=>q.Body.SelectMany(Fragments),CppScope q=>Fragments(q.Body),
        CppIf q=>new[]{q.Condition.Expression}.Concat(Fragments(q.Then)).Concat(Fragments(q.Else)),
        CppWhile q=>new[]{q.Condition}.Concat(Fragments(q.Body)),CppDoWhile q=>new[]{q.Condition.Expression}.Concat(Fragments(q.Body)),
        CppFor q=>new[]{q.Initializer,q.Condition,q.Increment}.Concat(Fragments(q.Body)),_=>[]
    };
    static bool HasContinue(CppStatement s)=>s switch {
        CppLine q=>Tokenize(q.Text).Tokens.Any(t=>t.Text=="continue"),CppSequence q=>q.Body.Any(HasContinue),CppScope q=>HasContinue(q.Body),
        CppIf q=>HasContinue(q.Then)||HasContinue(q.Else),_=>false
    };
}
