namespace SharpRetro.Decompiler;

public abstract record CppStatement;
// IsConditionDefinition marks an emitter-created boolean snapshot assignment;
// predicate forwarding independently checks purity, totality and availability.
public sealed record CppLine(string Text,string? Declares = null,bool IsEdgeTransfer = false,bool IsConditionDefinition = false) : CppStatement;
public sealed record CppSequence(IReadOnlyList<CppStatement> Body) : CppStatement;
// Only compiler-created scopes containing trivial temporaries are flattenable.
public sealed record CppScope(CppSequence Body) : CppStatement;
// Conditions are pure expressions: instruction/memory/flag effects have already
// been emitted as preceding statements, even when both branches become empty.
public sealed record CppCondition(string Expression,bool Negated = false) {
    public CppCondition Not() => this with { Negated=!Negated };
    public override string ToString() => !Negated ? Expression : Expression=="branch_condition" ? "!branch_condition" : $"!({Expression})";
}
public sealed record CppIf(CppCondition Condition,CppSequence Then,CppSequence Else) : CppStatement;
public sealed record CppWhile(string Condition,CppSequence Body) : CppStatement;
public sealed record CppDoWhile(CppCondition Condition,CppSequence Body) : CppStatement;

/// Local source rewrites after CFG validation. Declarations are explicit metadata,
/// not guessed by parsing C++ text. Statement/effect order never changes. This
/// tree contains only compiler-created trivial temporaries, never RAII objects,
/// and their addresses cannot escape their scopes (memcpy/context references
/// are noncapturing). Scope flattening and shared-tail extraction rely on this;
/// promoted source buffers or address-escaping locals require a separate proof.
public static class CppSourceTree {
    static CppSequence Sequence(params CppStatement[] body) => new(body);
    static bool Empty(CppSequence sequence) => sequence.Body.Count==0;

    public static CppSequence Clean(CppSequence source) => Clean(source,null);

    static CppSequence Clean(CppSequence source,CppSequence? continuation) {
        // The continuation is exact, ordered emitter-owned edge copies followed
        // by an abrupt statement in the same lexical loop/function. Arbitrary
        // effects and cycle-breaking declarations cannot join this bundle.
        // A loop body supplies its own continue, never an outer
        // break/continue. Value-bearing exits cannot cross a local declaration
        // that could change the binding of an identifier in their expression.
        if(continuation!=null) {
            var localNames=Declarations(source).ToHashSet();
            if(Identifiers(continuation).Any(localNames.Contains))
                continuation=null;
        }
        CppSequence? Exit(CppStatement statement) => statement switch {
            CppLine line when Terminates(line) => Sequence(line),
            // Block emission wraps even a lone return in a trivial scope. It
            // contributes no declarations or effects to the continuation.
            CppScope { Body.Body.Count:1 } scope => Exit(scope.Body.Body[0]),
            _ => null
        };
        // Region construction nests sequences; expose sibling order before
        // deciding whether the next action is a bare exit or an effect.
        var input=new List<CppStatement>();
        void Flatten(CppStatement statement) {
            if(statement is CppSequence sequence) foreach(var child in sequence.Body) Flatten(child);
            else input.Add(statement);
        }
        Flatten(source);
        CppSequence? Following(IReadOnlyList<CppStatement> statements,int start,CppSequence? inherited) {
            var body=new List<CppStatement>();
            for(int i=start;i<statements.Count;i++) {
                if(Exit(statements[i]) is { } exit) return new([..body,..exit.Body]);
                if(statements[i] is not CppLine { IsEdgeTransfer:true,Declares:null } transfer) return null;
                body.Add(transfer);
            }
            return inherited==null ? null : new([..body,..inherited.Body]);
        }
        CppStatement Rewrite(CppStatement statement,CppSequence? next) => statement switch {
            CppScope s => new CppScope(Clean(s.Body,next)),
            CppWhile w => Loop(w),
            CppDoWhile w => w with { Body=Clean(w.Body,Sequence(new CppLine("continue;"))) },
            CppIf i => Conditional(i with { Then=Clean(i.Then,next),Else=Clean(i.Else,next) },next),
            _ => statement
        };
        CppStatement Loop(CppWhile loop) {
            var body=Clean(loop.Body,Sequence(new CppLine("continue;")));
            // A direct final continue reaches the same next condition test as
            // falling off this body. Never reach through a nested loop.
            if(body.Body.LastOrDefault() is CppLine { Text:"continue;" })
                body=new(body.Body.Take(body.Body.Count-1).ToArray());
            // A post-test loop evaluates the condition only on its tail edge.
            // Earlier continues would start the original while(true) again
            // without evaluating that tail test, so they prevent this rewrite.
            if(loop.Condition=="true") {
                CppCondition? test=null;
                CppIf? transfers=null;
                int removed=0;
                if(body.Body.LastOrDefault() is CppIf { Then.Body.Count:1,Else.Body.Count:0 } guard &&
                   guard.Then.Body[0] is CppLine { Text:"break;" }) {
                    test=guard.Condition.Not(); removed=1;
                } else if(body.Body.Count>=2 && body.Body[^1] is CppLine { Text:"break;" } &&
                          body.Body[^2] is CppIf { Else.Body.Count:0 } back &&
                          back.Then.Body.LastOrDefault() is CppLine { Text:"continue;" } &&
                          back.Then.Body.SkipLast(1).All(s=>s is CppLine { IsEdgeTransfer:true,Declares:null })) {
                    test=back.Condition; removed=2;
                    if(back.Then.Body.Count>1) transfers=back with { Then=new(back.Then.Body.SkipLast(1).ToArray()) };
                }
                if(test!=null) {
                    var prefix=new CppSequence(body.Body.Take(body.Body.Count-removed).ToArray());
                    var names=Declarations(prefix).ToHashSet();
                    // Copies must not alter the saved test before do/while
                    // reads it. Only emitter-owned pure transfers qualify.
                    var assigned=transfers?.Then.Body.OfType<CppLine>().Select(l=>l.Text.Split('=')[0].Trim()).ToHashSet() ?? [];
                    if(!HasContinue(prefix) && !Identifiers(new CppLine(test.Expression)).Any(n=>names.Contains(n) || assigned.Contains(n)))
                        return new CppDoWhile(test,transfers==null ? prefix : new([..prefix.Body,transfers]));
                }
            }
            return loop with { Body=body };
        }
        CppStatement Conditional(CppIf conditional,CppSequence? next) {
            // Share only byte-for-byte identical, declaration-free trailing
            // statements whose names are not local to either branch. Moving
            // across this if leaves their effects and loop targets unchanged.
            var localNames=Declarations(conditional.Then).Concat(Declarations(conditional.Else)).ToHashSet();
            var yes=conditional.Then.Body; var no=conditional.Else.Body;
            int shared=0;
            while(shared<yes.Count && shared<no.Count &&
                  yes[^(shared+1)] is CppLine { Declares:null } line && line==no[^(shared+1)] &&
                  !System.Text.RegularExpressions.Regex.Matches(line.Text,@"[A-Za-z_]\w*").Any(m=>localNames.Contains(m.Value))) shared++;
            if(shared!=0) {
                var prefix=conditional with { Then=new(yes.Take(yes.Count-shared).ToArray()),Else=new(no.Take(no.Count-shared).ToArray()) };
                return new CppSequence([Conditional(prefix,Following(yes,yes.Count-shared,next)),..yes.Skip(yes.Count-shared)]);
            }
            if(Empty(conditional.Then)) {
                if(Empty(conditional.Else)) return Sequence();
                conditional=new(conditional.Condition.Not(),conditional.Else,Sequence());
            }
            // Turn a small exceptional arm into a guard when both arms would
            // otherwise reach exactly this continuation. Only the tagged copy/
            // exit bundle is duplicated; the large arm retains ordered effects.
            if(next!=null && !Empty(conditional.Else)) {
                int yesCost=Cost(conditional.Then), noCost=Cost(conditional.Else);
                bool CanFinish(CppSequence arm) => !Terminates(arm) &&
                    !Identifiers(next).Any(Declarations(arm).ToHashSet().Contains);
                if(yesCost<=16 && noCost>Math.Max(8,yesCost*2) && CanFinish(conditional.Then))
                    conditional=conditional with { Then=new([..conditional.Then.Body,..next.Body]) };
                else if(noCost<=16 && yesCost>Math.Max(8,noCost*2) && CanFinish(conditional.Else))
                    conditional=conditional with { Else=new([..conditional.Else.Body,..next.Body]) };
            }
            if(!Empty(conditional.Else) && Terminates(conditional.Then))
                return new CppSequence([conditional with { Else=Sequence() },new CppScope(conditional.Else)]);
            if(!Empty(conditional.Else) && Terminates(conditional.Else))
                return new CppSequence([new CppIf(conditional.Condition.Not(),conditional.Else,Sequence()),new CppScope(conditional.Then)]);
            if(Empty(conditional.Else) && next!=null && HasConditional(conditional.Then))
                return new CppSequence([new CppIf(conditional.Condition.Not(),next,Sequence()),new CppScope(conditional.Then)]);
            // With no intervening statements, short circuit evaluation has the
            // same order and path domain as these nested pure conditions.
            while(Empty(conditional.Else) && conditional.Then.Body.Count==1 &&
                  conditional.Then.Body[0] is CppIf inner && Empty(inner.Else))
                conditional=new(new($"({conditional.Condition}) && ({inner.Condition})"),inner.Then,Sequence());
            return conditional;
        }
        var flattened=new List<CppStatement>();
        void Append(CppStatement statement) {
            if(statement is CppSequence sequence) foreach(var child in sequence.Body) Append(child);
            else if(statement is not CppScope { Body.Body.Count:0 }) flattened.Add(statement);
        }
        for(int i=0;i<input.Count;i++)
            Append(Rewrite(input[i],Following(input,i+1,continuation)));

        // Reserve declarations already in this lexical scope, including ones
        // later in the sequence. Flattening must not create a redeclaration or
        // shadow a name in a subsequent sibling scope.
        var names=flattened.OfType<CppLine>().Where(l=>l.Declares!=null).Select(l=>l.Declares!).ToHashSet();
        var result=new List<CppStatement>();
        for(int index=0;index<flattened.Count;index++) {
            var statement=flattened[index];
            if(statement is CppScope scope) {
                var declarations=scope.Body.Body.OfType<CppLine>().Where(l=>l.Declares!=null).Select(l=>l.Declares!).ToArray();
                // A later use can refer to an ancestor's declaration even when
                // that name is not declared in this sequence. Hoisting must not
                // capture it with a newly flattened child declaration.
                var laterNames=flattened.Skip(index+1).SelectMany(Identifiers).ToHashSet();
                if(!declarations.Any(n=>names.Contains(n) || laterNames.Contains(n))) {
                    result.AddRange(scope.Body.Body); names.UnionWith(declarations);
                } else result.Add(scope);
            } else result.Add(statement);
            if(Terminates(statement)) break;
        }
        return new(result);
    }

    static bool HasConditional(CppStatement statement) => statement switch {
        CppIf => true,
        CppScope s => HasConditional(s.Body),
        CppSequence s => s.Body.Any(HasConditional),
        // Do not flatten a small outer guard merely because it contains a loop.
        _ => false
    };

    static int Cost(CppStatement statement) => statement switch {
        CppLine => 1,
        CppSequence s => s.Body.Sum(Cost),
        CppScope s => Cost(s.Body),
        CppIf i => 1+Cost(i.Then)+Cost(i.Else),
        CppWhile w => 1+Cost(w.Body),
        CppDoWhile w => 1+Cost(w.Body),
        _ => throw new NotSupportedException("Unknown C++ source statement")
    };

    static IEnumerable<string> Identifiers(CppStatement statement) {
        IEnumerable<string> Tokens(string text) => System.Text.RegularExpressions.Regex.Matches(text,@"[A-Za-z_]\w*").Select(m=>m.Value);
        return statement switch {
            CppLine l => Tokens(l.Text),
            CppSequence s => s.Body.SelectMany(Identifiers),
            CppScope s => Identifiers(s.Body),
            CppIf i => Tokens(i.Condition.Expression).Concat(Identifiers(i.Then)).Concat(Identifiers(i.Else)),
            CppWhile w => Tokens(w.Condition).Concat(Identifiers(w.Body)),
            CppDoWhile w => Tokens(w.Condition.Expression).Concat(Identifiers(w.Body)),
            _ => []
        };
    }

    static bool HasContinue(CppStatement statement) => statement switch {
        CppLine { Text:"continue;" } => true,
        CppSequence s => s.Body.Any(HasContinue),
        CppScope s => HasContinue(s.Body),
        CppIf i => HasContinue(i.Then) || HasContinue(i.Else),
        // These loops own their continues; they do not target this loop.
        _ => false
    };

    static IEnumerable<string> Declarations(CppStatement statement) => statement switch {
        CppLine { Declares:{ } name } => [name],
        CppSequence s => s.Body.SelectMany(Declarations),
        CppScope s => Declarations(s.Body),
        CppIf i => Declarations(i.Then).Concat(Declarations(i.Else)),
        CppWhile w => Declarations(w.Body),
        CppDoWhile w => Declarations(w.Body),
        _ => []
    };

    public static bool Terminates(CppStatement statement) => statement switch {
        CppLine l => l.Text is "return;" or "break;" or "continue;" || l.Text.StartsWith("return ") || l.Text.StartsWith("throw "),
        CppSequence s => s.Body.Count>0 && Terminates(s.Body[^1]),
        CppScope s => Terminates(s.Body),
        CppIf i => Terminates(i.Then) && Terminates(i.Else),
        _ => false
    };

    public static void Emit(CppSequence source,Action<string,int> line) {
        void Write(CppStatement statement,int depth) {
            switch(statement) {
                case CppLine l: line(l.Text,depth); break;
                case CppSequence s: foreach(var child in s.Body) Write(child,depth); break;
                case CppScope s:
                    line("{",depth); Write(s.Body,depth+1); line("}",depth); break;
                case CppWhile w:
                    line($"while({w.Condition}) {{",depth); Write(w.Body,depth+1); line("}",depth); break;
                case CppFor w:
                    line($"for({w.Initializer}; {w.Condition}; {w.Increment}) {{",depth); Write(w.Body,depth+1); line("}",depth); break;
                case CppDoWhile w:
                    line("do {",depth); Write(w.Body,depth+1); line($"}} while({w.Condition});",depth); break;
                case CppIf i: WriteIf(i,depth,"if"); break;
            }
        }
        void WriteIf(CppIf conditional,int depth,string prefix) {
            line($"{prefix}({conditional.Condition}) {{",depth);
            Write(conditional.Then,depth+1);
            if(conditional.Else.Body.Count==1 && conditional.Else.Body[0] is CppIf next) WriteIf(next,depth,"} else if");
            else if(!Empty(conditional.Else)) {
                line("} else {",depth); Write(conditional.Else,depth+1); line("}",depth);
            } else line("}",depth);
        }
        Write(source,0);
    }
}
