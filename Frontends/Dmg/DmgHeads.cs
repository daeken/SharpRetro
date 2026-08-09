using ArchCompilerCore;
namespace Frontends.Dmg;

// Per-ISA heads {sig only} extracted from legacy DamageGenerator/Expressions.cs.
public class DmgHeads : Builtin {
    public override void Define() {
        Expr("pc", _ => new EInt(false, 16));
        Expr("reg", _ => new EInt(false, 8).AsRuntime());
        Expr("reg-ime", _ => new EInt(false, 1).AsRuntime());
        Expr("reg-ime-schedule", _ => new EInt(false, 1).AsRuntime());
        Expr("reg-flags", _ => new EInt(false, 8).AsRuntime());
        Expr("reg-bc", _ => new EInt(false, 16).AsRuntime());
        Expr("reg-de", _ => new EInt(false, 16).AsRuntime());
        Expr("reg-hl", _ => new EInt(false, 16).AsRuntime());
        Expr("reg-af", _ => new EInt(false, 16).AsRuntime());
        Expr("reg-sp", _ => new EInt(false, 16).AsRuntime());
        Stmt("=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException());
        Expr("load", list => TypeFromName(list[2]).AsRuntime());
        Expr("store", _ => EType.Unit.AsRuntime());
        Stmt("branch-default", _ => EType.Unit.AsRuntime());
        Stmt("cycles", _ => EType.Unit.AsRuntime());
        Expr("halt", _ => EType.Unit.AsRuntime());
    }
}
