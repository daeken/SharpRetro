using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DoubleSharp.Linq;
using PrettyPrinter;

namespace ArchCompilerCore; 

public class Def {
	public static List<Def> ParseAll(PList top, Func<PList, Def> transform) {
		var ret = new List<Def>();
		foreach(var elem in top)
			if(elem is PList pl && pl.Count != 0)
				switch(pl[0]) {
					case PName("def"):
						ret.Add(transform(pl));
						break;
					case PName("block"):
						ret.AddRange(ParseAll(pl, transform));
						break;
				}
		return ret;
	}

	public readonly string Name;
	public readonly PTree Disassembly;
	public readonly IReadOnlyDictionary<string, EType> Locals;
	public readonly PList Decode, Eval;

	protected Def(string name, PTree dasm, PList decode, PList eval, IReadOnlyDictionary<string, EType> _locals) {
		Name = name;
		Disassembly = dasm;
		Decode = decode;
		Eval = eval;

		var locals = new Dictionary<string, EType>(_locals);

		void InferList(PList list) => list.Skip(1).ForEach(x => InferType(x));

		EType InferType(PTree tree) => tree.Type is EUndef ? tree.Type = _InferType(tree) : tree.Type;
		EType _InferType(PTree tree) {
			switch(tree) {
				case PList list:
					switch(((PName) list[0]).Name) {
						case "block":
							InferList(list);
							return list.Last().Type;
						case "for":
							locals[((PName) ((PList) list[1])[0]).Name] = new EInt(true, 32);
							list.Skip(2).ForEach(x => InferType(x));
							return EType.Unit;
						case "let":
							locals[((PName) list[1]).Name] = InferType(list[2]);
							list.Skip(3).ForEach(x => InferType(x));
							return list.Last().Type;
						case "mlet":
							if(list[1] is not PList dlist) throw new NotSupportedException();
							Debug.Assert(dlist.Count % 2 == 0);
							for(var i = 0; i < dlist.Count; i += 2)
								locals[((PName) dlist[i]).Name] = InferType(dlist[i + 1]);
							list.Skip(2).ForEach(x => InferType(x));
							return list.Last().Type;
						case { } fname when Heads.All.TryGetValue(fname, out var _h) && _h.IsStatement:
							InferList(list);
							return Heads.All[fname].Signature(list);
						case { } fname when Heads.All.ContainsKey(fname):
							InferList(list);
							return Heads.All[fname].Signature(list);
						default:
							throw new NotImplementedException($"Unhandled function: {list[0]}");
					}
				case PString:
					return EType.String;
				case PInt(var value):
					if(value >= 0)
						return value switch {
							<= byte.MaxValue => new(false, 8), 
							<= ushort.MaxValue => new(false, 16), 
							<= uint.MaxValue => new(false, 32), 
							_ => new EInt(false, 64), 
						};
					else
						return value switch {
							>= sbyte.MinValue => new(true, 8), 
							>= short.MinValue => new(true, 16), 
							>= int.MinValue => new(true, 32), 
							_ => new EInt(true, 64), 
						};
				case PName pname:
					return locals.ContainsKey(pname.Name) ? locals[pname.Name] : EType.Unit;
				default:
					throw new NotImplementedException($"Unknown type for inference: {tree.ToPrettyString()}");
			}
		}
		InferType(Decode);
		InferType(Disassembly);
		InferType(Eval);
		Locals = locals;
	}
}


// Post-pass runtime-propagation (legacy Core.cs:141 InferRuntime → InferList → InferExpression).
// Def.ctor's InferType computes each node's type via Signature; this pass then propagates
// .Runtime UPWARD: if any child is runtime, the parent becomes runtime (regardless of what
// Signature returned). This is why e.g. (vector-insert rd i (gpr64 rn)) — sig=EType.Unit —
// ends up .Runtime=true at recompile-context: gpr64 is .AsRuntime(), InferExpression lifts it.
// Rung-1b matched WITHOUT this because the tree-dump showed sig-computed types; rung-2's
// Recompiler.cs diverged on exactly the heads whose sigs don't AsRuntime(list.AnyRuntime).
public static class RuntimeInference {
    public static Def InferRuntime(Def def) {
        InferList(def.Decode);
        InferList(def.Eval);
        return def;
    }
    static void InferList(PList list) {
        // Statement-heads: recurse children only, then re-eval Signature (matches legacy Core.cs:115-119).
        // Expression-heads: InferExpression (bottom-up runtime propagation).
        if(list[0] is PName(var name) && Heads.All.TryGetValue(name, out var h) && h.IsStatement) {
            foreach(var elem in list.Skip(1))
                if(elem is PList sub) InferList(sub);
            list.Type = h.Signature(list);
        } else {
            InferExpression(list);
        }
    }
    static EType InferExpression(PTree tree) {
        if(tree.Type.Runtime) return tree.Type;
        if(tree is PList list) {
            var set = false;
            foreach(var elem in list)
                if(InferExpression(elem).Runtime) set = true;
            return list.Type = set ? list.Type.AsRuntime() : list.Type;
        }
        return tree.Type;
    }
}


// Fresh-identifier generator (legacy Core.cs:195). Used by frontend tree-rewriting
// (MipsDef's branch-slot reg-defer pass) AND by backend emit (mlet lowering) — both
// call sites share ONE counter in the legacy compiler, so temps never collide.
public static class Temp {
    static int I;
    public static string Name() => $"temp_{I++}";
}
