namespace ArchCompilerCore;

// Extracted {sig, exec} from legacy: CoreArchCompiler/ControlFlow.cs
// Emit-lambdas → Backends/CSharp (rung-2). Local helpers lifted VERBATIM from legacy.
public class ControlFlow : Builtin {
    public override void Define() {
        Stmt("requires", list => EType.Unit,
            (list, state) =>
				list.Skip(1).Select(x => Extensions.AsBool(state.Evaluate(x))).Aggregate((a, b) => a && b)
					? true
					: throw new BailoutException());
        // block/if/match: legacy dual-registers (Stmt+Expr sigs). InferType dispatches
        // Stmt-first (Def.cs:64-69), so Stmt-sig is canonical here; the Expr-sig only
        // fired at emit-time (GenerateExpression path) → lives in Backend at rung-2.
        Stmt("block", list => list.Last().Type,
            (list, state) => state.Evaluate(list.Skip(1)));
        Stmt("if", list => list[2].Type.AsRuntime(list[1].Type.Runtime ||
			                               list[2].Type is not EUnit && list[2].Type.Runtime ||
			                               list[3].Type is not EUnit && list[3].Type.Runtime));
        Stmt("for", _ => EType.Unit,
            (list, state) => {
			var rlist = (PList) list[1];
			var varName = ((PName) rlist[0]).Name;
			var range = rlist.Skip(1).Select(state.Evaluate).ToList();
			int start = 0, end = 0, step = 1;
			if(range.Count == 1)
				end = (int) range[0];
			else if(range.Count == 2)
				(start, end) = ((int) range[0], (int) range[1]);
			else if(range.Count == 3)
				(start, end, step) = ((int) range[0], (int) range[1], (int) range[2]);
			else
				throw new NotSupportedException();
			var hasPrevious = state.Locals.ContainsKey(varName);
			var preValue = hasPrevious ? state.Locals[varName] : null;
			for(var i = start; i < end; i += step) {
				state.Locals[varName] = i;
				state.Evaluate(list.Skip(2));
			}
			if(hasPrevious)
				state.Locals[varName] = preValue;
			else
				state.Locals.Remove(varName);
			return null;
		});
        Stmt("when", list => EType.Unit.AsRuntime(list[1].Type.Runtime),
            (list, state) => Extensions.AsBool(state.Evaluate(list[1])) ? list.Skip(1).Select(x => state.Evaluate(x)).ToList() : null);
        Stmt("match", list => list.Count == 3 ? list[2].Type : list[3].Type);
        Stmt("assert", _ => EType.Unit,
            (list, state) => {
				if(!state.Evaluate(list[1])) {
					Console.WriteLine($"Assertion failed {list[1]}: {state.Evaluate(list[2])}");
					Environment.Exit(1);
				}
				return null;
			});
        Expr("unimplemented", _ => EType.Unit);
        // legacy ControlFlow.cs:348 uses BranchExpression() (a variant registration
        // form the mechanical extraction missed — regex only matched Expression|Statement).
        Stmt("branch", _ => EType.Unit.AsRuntime(),
            (list, state) => state.Registers["PC"] = state.Evaluate(list[1]));
    }
}
