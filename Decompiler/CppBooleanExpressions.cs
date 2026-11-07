using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

/// Render recovered primitive predicates using C++ precedence. Ordered comparison
/// complements require the caller's integer profile: !(a<=b) is NOT a>b for NaN.
/// Unknown syntax remains opaque; expression order and short-circuiting stay fixed.
public static class CppBooleanExpressions {
    abstract record Expr;
    sealed record Atom(string Text) : Expr;
    sealed record Unary(string Op,Expr Value) : Expr;
    sealed record Binary(string Op,Expr Left,Expr Right) : Expr;
    static int Precedence(string op)=>op switch {
        "||"=>1,"&&"=>2,"|"=>3,"^"=>4,"&"=>5,"==" or "!="=>6,
        "<" or "<=" or ">" or ">="=>7,"<<" or ">>"=>8,"+" or "-"=>9,"*" or "/" or "%"=>10,_=>0
    };
    static bool Boolean(Expr e)=>e is Unary {Op:"!"} or Binary {Op:"||" or "&&" or "==" or "!=" or "<" or "<=" or ">" or ">="} or Atom {Text:"true" or "false"};

    public static string Format(string expression,bool negated,bool integerComparisons,bool truthContext=true,Func<string,string>? formatAtom=null) {
        // The shared scanner omits quoted literals. Keep them opaque rather
        // than let adjacent remaining operators form a different expression.
        if(expression.Contains('"') || expression.Contains('\'')) return Original();
        var lexed=Tokenize(expression);
        if(!lexed.Valid || lexed.Tokens.Count==0 || lexed.Tokens.Count>4096) return Original();
        var tokens=lexed.Tokens;int at=0;bool valid=true;
        Expr? Parse(int minimum=1,int depth=0) {
            if(depth>128 || at>=tokens.Count) { valid=false;return null; }
            Expr? left;
            if(tokens[at].Text is "!" or "~" or "+" or "-" or "*" or "&") {
                string op=tokens[at++].Text;var value=Parse(11,depth+1);
                left=value==null ? null : new Unary(op,value);
            } else if(tokens[at].Text=="(") {
                at++;left=Parse(1,depth+1);
                if(at>=tokens.Count || tokens[at++].Text!=")") valid=false;
            } else {
                int start=at;
                if(!tokens[at].Identifier && !char.IsDigit(tokens[at].Text[0])) { valid=false;return null; }
                at++;
                // Qualified names and explicit C++ casts are opaque operands.
                while(at+1<tokens.Count && tokens[at].Text=="::" && tokens[at+1].Identifier) at+=2;
                if(tokens[start].Text is not "static_cast" and not "reinterpret_cast" and not "const_cast" and not "dynamic_cast" && at<tokens.Count && tokens[at].Text=="<") {
                    // `helper<T>(x)` is ambiguous with chained comparisons
                    // without C++ name lookup. Preserve the whole predicate.
                    int angles=0;
                    for(int cursor=at;cursor<tokens.Count;cursor++) {
                        string t=tokens[cursor].Text;
                        if(t=="<") angles++;else if(t==">") angles--;else if(t==">>") angles-=2;
                        if(angles<=0) {
                            if(angles==0 && cursor+1<tokens.Count && tokens[cursor+1].Text=="(") { valid=false;return null; }
                            break;
                        }
                    }
                }
                if(tokens[start].Text is "static_cast" or "reinterpret_cast" or "const_cast" or "dynamic_cast") {
                    if(at>=tokens.Count || tokens[at++].Text!="<") { valid=false;return null; }
                    int angles=1;
                    while(at<tokens.Count && angles>0) {
                        string t=tokens[at++].Text;
                        if(t=="<") angles++;else if(t==">") angles--;else if(t==">>") angles-=2;
                    }
                    if(angles!=0 || at>=tokens.Count || tokens[at].Text!="(") { valid=false;return null; }
                }
                // Function/cast arguments retain exact original source. Do not
                // interpret comma arguments, literals or nested template syntax.
                if(at<tokens.Count && tokens[at].Text=="(") {
                    int parentheses=1;at++;
                    while(at<tokens.Count && parentheses>0) {
                        string t=tokens[at++].Text;
                        if(t=="(") parentheses++;else if(t==")") parentheses--;
                    }
                    if(parentheses!=0) valid=false;
                }
                int end=tokens[at-1].Start+tokens[at-1].Length;
                left=new Atom(expression[tokens[start].Start..end]);
            }
            if(left==null) return null;
            while(valid && at<tokens.Count && Precedence(tokens[at].Text) is var precedence && precedence>=minimum) {
                string op=tokens[at++].Text;var right=Parse(precedence+1,depth+1);
                if(right==null) return null;
                left=new Binary(op,left,right);
            }
            return left;
        }
        var parsed=Parse();
        if(!valid || parsed==null || at!=tokens.Count) return Original();
        // Parenthesis removal would otherwise lose comments between operands.
        // Keep these rare fragments verbatim rather than relocate their text.
        if(lexed.Comments is {Count:>0}) return Original();
        Expr Simplify(Expr e)=>e switch {
            Unary {Op:"!"} u=>Not(Simplify(u.Value)),
            Unary u=>u with {Value=Simplify(u.Value)},
            Binary b=>b with {Left=Simplify(b.Left),Right=Simplify(b.Right)},
            _=>e
        };
        Expr Not(Expr e) {
            if(e is Atom {Text:"true"}) return new Atom("false");
            if(e is Atom {Text:"false"}) return new Atom("true");
            if(e is Unary {Op:"!"} u && Boolean(u.Value)) return u.Value;
            if(e is Binary {Op:"&&" or "||"} logical)
                return new Binary(logical.Op=="&&" ? "||" : "&&",Not(logical.Left),Not(logical.Right));
            if(integerComparisons && e is Binary comparison) {
                string? inverse=comparison.Op switch {"=="=>"!=","!="=>"==","<"=>">=","<="=>">",">"=>"<=",">="=>"<",_=>null};
                if(inverse!=null) return comparison with {Op=inverse};
            }
            return new Unary("!",e);
        }
        string Render(Expr e,int parent=0) {
            int precedence=e switch {Binary b=>Precedence(b.Op),Unary=>11,_=>12};
            string Operand(Expr child,string op,bool right) {
                int required=precedence+(right && op is not "&&" and not "||" ? 1 : 0);
                string value=Render(child,required);
                // Keep mixed boolean/bitwise groups explicit, including the
                // groupings Clang diagnoses with -Wparentheses under -Wall.
                if(child is Binary nested && Precedence(nested.Op)>=required &&
                   (op=="||" && nested.Op=="&&" || op is "|" or "^" or "&" && nested.Op!=op ||
                    op is "<<" or ">>" && nested.Op is "+" or "-")) value="("+value+")";
                return value;
            }
            string text=e switch {
                Atom a=>formatAtom?.Invoke(a.Text) ?? a.Text,
                // Keep separate unary tokens from becoming ++, -- or &&.
                Unary u when u.Value is Unary v && u.Op==v.Op && u.Op is "+" or "-" or "&"=>u.Op+"("+Render(u.Value)+")",
                Unary u=>u.Op+Render(u.Value,11),
                Binary b=>Operand(b.Left,b.Op,false)+" "+b.Op+" "+Operand(b.Right,b.Op,true),
                _=>throw new NotSupportedException("Unknown predicate expression")
            };
            return precedence<parent ? "("+text+")" : text;
        }
        Expr TruthContext(Expr e) {
            // if/while and logical operands perform their own bool conversion.
            // Keep !!x inside comparisons/arithmetic, where its 0/1 value matters.
            while(e is Unary {Op:"!",Value:Unary {Op:"!"} inner}) e=inner.Value;
            return e is Binary {Op:"&&" or "||"} logical
                ? logical with {Left=TruthContext(logical.Left),Right=TruthContext(logical.Right)} : e;
        }
        var result=Simplify(parsed);
        result=negated ? Not(result) : result;
        return Render(truthContext ? TruthContext(result) : result);
        string Original()=>negated ? "!("+expression+")" : expression;
    }

    public static CppSequence Clean(CppSequence body,bool integerComparisons) {
        CppCondition Condition(CppCondition c)=>new(Format(c.Expression,c.Negated,integerComparisons));
        CppStatement Rewrite(CppStatement s)=>s switch {
            CppSequence q=>new CppSequence(q.Body.Select(Rewrite).ToArray()),
            CppScope q=>q with {Body=(CppSequence)Rewrite(q.Body)},
            CppIf c=>c with {Condition=Condition(c.Condition),Then=(CppSequence)Rewrite(c.Then),Else=(CppSequence)Rewrite(c.Else)},
            CppWhile w=>w with {Condition=Format(w.Condition,false,integerComparisons),Body=(CppSequence)Rewrite(w.Body)},
            CppDoWhile w=>w with {Condition=Condition(w.Condition),Body=(CppSequence)Rewrite(w.Body)},
            _=>s
        };
        return (CppSequence)Rewrite(body);
    }
}
