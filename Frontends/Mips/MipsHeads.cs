using ArchCompilerCore;
namespace Frontends.Mips;

// Per-ISA heads {sig only} extracted from legacy SharpStationGenerator/Expressions.cs.
public class MipsHeads : Builtin {
    public override void Define() {
        Expr("pc", _ => new EInt(false, 32));
        Expr("pcd", _ => new EInt(false, 32));
        Expr("reg", _ => new EInt(false, 32).AsRuntime());
        Expr("reg-hi", _ => new EInt(false, 32).AsRuntime());
        Expr("reg-lo", _ => new EInt(false, 32).AsRuntime());
        Expr("absorb-muldiv-delay", _ => EUnit.RuntimeType);
        Expr("copfun", _ => EUnit.RuntimeType);
        Expr("exception", _ => EUnit.RuntimeType);
        Expr("copreg", _ => new EInt(false, 32).AsRuntime());
        Expr("copcreg", _ => EUnit.RuntimeType);
        Expr("mul-delay", _ => EUnit.RuntimeType);
        Expr("div-delay", _ => EUnit.RuntimeType);
        Stmt("=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException());
        Stmt("defer=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException());
        Expr("load", list => TypeFromName(list[2]).AsRuntime());
        Expr("store", _ => EType.Unit.AsRuntime());
        Expr("do-load", _ => EType.Unit.AsRuntime());
        Stmt("do-lds", _ => EType.Unit.AsRuntime());
        Stmt("read-absorb", _ => EType.Unit.AsRuntime());
    }
}
