using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

// These two nodes exist only between region emission and early lowering. Explicit
// owner identities distinguish a nested loop exit from an enclosing transfer.
public sealed record CppExitArm(ulong Target,CppSequence Body);
public sealed record CppMultiExitLoop(ulong Header,CppSequence Body,IReadOnlyList<CppExitArm> Exits) : CppStatement;
public sealed record CppExitLoop(ulong Header,ulong Target) : CppStatement;

/// Sink closed exit continuations onto their exact loop-exit edges. A normally
/// completing continuation gets a break of its original owner; return/throw
/// paths need no dispatch. Outer break/continue paths remain outside inner loops.
/// Scope placement and all value/CFG analyses run AFTER this early lowering.
public static class CppLoopExits {
    [Flags] enum Flow { Normal=1,Return=2,Throw=4,Break=8,Continue=16,Unknown=32 }
    sealed class ExpansionLimit : Exception;
    sealed record Route(CppSequence Body,int Cost);

    public static CppSequence Lower(CppSequence source,int expansionLimit=4096) {
        CppSequence Run(bool inline) {
            int expanded=0;
            CppStatement Rewrite(CppStatement statement,IReadOnlyDictionary<(ulong,ulong),Route> routes) {
                switch(statement) {
                    case CppSequence q: {
                        var tail=new CppSequence([]);
                        // Sequence nodes group regions without opening a C++
                        // scope. Expose their real sibling continuation first.
                        IEnumerable<CppStatement> Flatten(CppStatement item)=>item is CppSequence sequence ? sequence.Body.SelectMany(Flatten) : [item];
                        foreach(var child in q.Body.SelectMany(Flatten).Reverse()) {
                            var flow=Effects(tail);
                            bool terminal=(flow&(Flow.Normal|Flow.Break|Flow.Continue|Flow.Unknown))==0;
                            if(inline && terminal && Cost(tail)<=96 && child is CppIf conditional) {
                                if((expanded+=Cost(tail))>expansionLimit)throw new ExpansionLimit();
                                CppSequence Finish(CppSequence arm)=>new([new CppScope(arm),..Clone(tail).Body]);
                                tail=new([Rewrite(conditional with {Then=Finish(conditional.Then),Else=Finish(conditional.Else)},routes)]);
                            } else if(inline && terminal && Cost(tail)<=96 && child is CppScope scope && TrivialLocals(scope.Body) &&
                                      !FreeNames(tail).Intersect(Declarations(scope.Body)).Any()) {
                                tail=new([Rewrite(scope with {Body=new([..scope.Body.Body,..tail.Body])},routes)]);
                            } else if(inline && child is CppMultiExitLoop loop && !BareBreak(loop.Body) && Cost(tail)<=96 && terminal) {
                                // The loop's common terminal suffix belongs on
                                // every selected exit, after that arm's scope.
                                // This also resolves enclosing return edges in
                                // nested loops without changing their owner.
                                if((long)Cost(tail)*loop.Exits.Count>expansionLimit-expanded)throw new ExpansionLimit();
                                var continued=loop with {Exits=loop.Exits.Select(e=>e with {Body=new CppSequence([new CppScope(e.Body),..Clone(tail).Body])}).ToArray()};
                                tail=new([Rewrite(continued,routes)]);
                            } else tail=new([Rewrite(child,routes),..tail.Body]);
                        }
                        return tail;
                    }
                    case CppScope q:return q with {Body=(CppSequence)Rewrite(q.Body,routes)};
                    case CppIf q:return q with {Then=(CppSequence)Rewrite(q.Then,routes),Else=(CppSequence)Rewrite(q.Else,routes)};
                    case CppWhile q:return q with {Body=(CppSequence)Rewrite(q.Body,routes)};
                    case CppDoWhile q:return q with {Body=(CppSequence)Rewrite(q.Body,routes)};
                    case CppExitLoop exit:
                        if(!routes.TryGetValue((exit.Header,exit.Target),out var route))throw new InvalidOperationException("Unbound recovered loop exit");
                        if(inline && (expanded+=route.Cost)>expansionLimit)throw new ExpansionLimit();
                        return new CppScope(Clone(route.Body));
                    case CppMultiExitLoop loop: {
                        // Exits execute in the enclosing lexical/loop context.
                        // Resolve enclosing terminal routes before deciding
                        // whether an inner continuation is closed as well.
                        var exits=loop.Exits.Select(e=>e with {Body=(CppSequence)Rewrite(e.Body,routes)}).ToArray();
                        var local=new Dictionary<(ulong,ulong),Route>(routes);
                        var pending=new List<CppExitArm>();
                        var bodyNames=Declarations(loop.Body).ToHashSet();
                        bool validOwner=!BareBreak(loop.Body);
                        bool safeLocals=validOwner&&TrivialLocals(loop.Body);
                        foreach(var arm in exits) {
                            var flow=Effects(arm.Body);int cost=Cost(arm.Body);
                            bool closed=(flow&(Flow.Break|Flow.Continue|Flow.Unknown))==0;
                            if(inline && safeLocals && closed && cost<=96 && !FreeNames(arm.Body).Any(bodyNames.Contains)) {
                                var body=(flow&Flow.Normal)!=0 ? new CppSequence([..arm.Body.Body,new CppLine("break;")]) : arm.Body;
                                local[(loop.Header,arm.Target)]=new(body,Cost(body));
                            } else pending.Add(arm);
                        }
                        string selector=$"loop_exit_{loop.Header:x}";
                        foreach(var arm in pending) {
                            var body=pending.Count==1&&validOwner ? new CppSequence([new CppLine("break;")])
                                : new CppSequence([new CppLine($"{selector}=0x{arm.Target:x}ULL;"),new CppLine("break;")]);
                            local[(loop.Header,arm.Target)]=new(body,0);
                        }
                        var repetition=new CppWhile("true",(CppSequence)Rewrite(loop.Body,local));
                        if(pending.Count==0)return repetition;
                        if(pending.Count==1&&validOwner)return new CppScope(new([repetition,pending[0].Body]));
                        CppSequence continuation=new([new CppLine("throw std::logic_error(\"invalid recovered loop exit\");")]);
                        foreach(var arm in pending.AsEnumerable().Reverse())
                            continuation=new([new CppIf(new($"{selector}==0x{arm.Target:x}ULL"),arm.Body,continuation)]);
                        return new CppScope(new([new CppLine($"std::uint64_t {selector}=0;",selector),repetition,continuation]));
                    }
                    default:return statement;
                }
            }
            return (CppSequence)Rewrite(source,new Dictionary<(ulong,ulong),Route>());
        }
        try{return Run(!RawControl(source));}catch(ExpansionLimit){return Run(false);}
    }

    static Flow Effects(CppStatement s) {
        switch(s) {
            case CppSequence q: {
                Flow flow=Flow.Normal;
                foreach(var child in q.Body)if((flow&Flow.Normal)!=0)flow=(flow&~Flow.Normal)|Effects(child);
                return flow;
            }
            case CppScope q:return Effects(q.Body);
            case CppIf q:return Effects(q.Then)|Effects(q.Else);
            case CppWhile q:return Flow.Normal|(Effects(q.Body)&~(Flow.Normal|Flow.Break|Flow.Continue));
            case CppDoWhile q:return Flow.Normal|(Effects(q.Body)&~(Flow.Normal|Flow.Break|Flow.Continue));
            case CppLine q: {
                var lexed=Tokenize(q.Text);if(!lexed.Valid)return Flow.Unknown;
                var t=lexed.Tokens;if(t.Count==0)return Flow.Normal;
                if(t.Any(t=>t.Text is "goto" or "if" or "while" or "do" or "for" or "switch" or "try" or "catch" or "#"))return Flow.Unknown;
                if(t.Skip(1).Any(t=>t.Text is "return" or "throw" or "break" or "continue"))return Flow.Unknown;
                return t[0].Text switch {"return"=>Flow.Return,"throw"=>Flow.Throw,"break"=>Flow.Break,"continue"=>Flow.Continue,_=>Flow.Normal};
            }
            default:return Flow.Unknown;
        }
    }
    static IEnumerable<CppStatement> Children(CppStatement s)=>s switch {
        CppSequence q=>q.Body,CppScope q=>[q.Body],CppIf q=>[q.Then,q.Else],CppWhile q=>[q.Body],CppDoWhile q=>[q.Body],
        CppMultiExitLoop q=>new[]{q.Body}.Concat(q.Exits.Select(e=>e.Body)),_=>[]
    };
    static int Cost(CppStatement s)=>1+Children(s).Sum(Cost);
    static bool RawControl(CppStatement s) {
        if(s is CppLine q) {
            var lexed=Tokenize(q.Text);var t=lexed.Tokens;
            if(!lexed.Valid || t.Any(t=>t.Text is "goto" or "switch" or "case" or "try" or "catch" or "#") ||
               t.Count>1&&t[0].Identifier&&t[1].Text==":")return true;
        }
        return Children(s).Any(RawControl);
    }
    static bool BareBreak(CppStatement s)=>s switch {
        CppLine q=>Tokenize(q.Text).Tokens.Any(t=>t.Text=="break"),
        CppWhile or CppDoWhile=>false,
        CppMultiExitLoop q=>q.Exits.Any(e=>BareBreak(e.Body)),
        _=>Children(s).Any(BareBreak)
    };
    static IEnumerable<string> Declarations(CppStatement s)=>s is CppLine {Declares:{ } name} ? [name] : Children(s).SelectMany(Declarations);
    static bool TrivialLocals(CppStatement s) {
        if(s is CppLine {Declares:{ } name} line) {
            var lexed=Tokenize(line.Text);if(!lexed.Valid)return false;
            var tokens=lexed.Tokens;int at=0;
            if(tokens.Count>3&&tokens[0].Text=="["&&tokens[1].Text=="[") {while(at+1<tokens.Count&&!(tokens[at].Text=="]"&&tokens[at+1].Text=="]"))at++;at+=2;}
            var type=tokens.Skip(at).TakeWhile(t=>t.Text!=name).Select(t=>t.Text).ToArray();
            string plain=string.Concat(type.Where(t=>t is not "const" and not "*"));
            if(!System.Text.RegularExpressions.Regex.IsMatch(plain,@"^(?:(?:std::)?(?:u?int(?:8|16|32|64)_t|size_t|ptrdiff_t)|bits|bool|u(?:8|16|32|64|128)|char|int|unsigned|unsignedint|unsignedlong|unsignedlonglong)$"))return false;
            int index=Enumerable.Range(0,tokens.Count).FirstOrDefault(i=>tokens[i].Text==name,-1);
            if(index<0||index+1>=tokens.Count||tokens[index+1].Text is not "=" and not ";")return false;
        }
        return Children(s).All(TrivialLocals);
    }
    static IEnumerable<string> FreeNames(CppSequence body) {
        var free=new HashSet<string>();
        void Text(string text,HashSet<string> bound) {
            var lexed=Tokenize(text);if(!lexed.Valid){free.Add("?");return;}
            foreach(var t in lexed.Tokens)if(t.Identifier&&!bound.Contains(t.Text))free.Add(t.Text);
        }
        void Scan(CppStatement s,HashSet<string> bound) {
            switch(s) {
                case CppLine q:if(q.Declares!=null)bound.Add(q.Declares);Text(q.Text,bound);break;
                case CppSequence q:foreach(var child in q.Body)Scan(child,bound);break;
                case CppScope q:Scan(q.Body,new(bound));break;
                case CppIf q:Text(q.Condition.Expression,bound);Scan(q.Then,new(bound));Scan(q.Else,new(bound));break;
                case CppWhile q:Text(q.Condition,bound);Scan(q.Body,new(bound));break;
                case CppDoWhile q:Scan(q.Body,new(bound));Text(q.Condition.Expression,bound);break;
            }
        }
        Scan(body,[]);return free;
    }
    static CppSequence Clone(CppSequence body) {
        CppStatement Copy(CppStatement s)=>s switch {
            CppLine q=>q with {},CppSequence q=>new CppSequence(q.Body.Select(Copy).ToArray()),CppScope q=>q with {Body=Clone(q.Body)},
            CppIf q=>q with {Then=Clone(q.Then),Else=Clone(q.Else)},CppWhile q=>q with {Body=Clone(q.Body)},CppDoWhile q=>q with {Body=Clone(q.Body)},_=>throw new InvalidOperationException("Unlowered exit continuation")
        };
        return new(body.Body.Select(Copy).ToArray());
    }
}
