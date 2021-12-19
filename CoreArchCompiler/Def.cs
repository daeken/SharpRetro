using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MoreLinq.Extensions;
using PrettyPrinter;

namespace CoreArchCompiler; 

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
						case { } fname when Core.Statements.ContainsKey(fname):
							InferList(list);
							return Core.Statements[fname].Signature(list);
						case { } fname when Core.Expressions.ContainsKey(fname):
							InferList(list);
							return Core.Expressions[fname].Signature(list);
						default:
							throw new NotImplementedException($"Unhandled function: {list[0]}");
					}
				case PString:
					return EType.String;
				case PInt(var value):
					if(value >= 0)
						return value switch {
							<= byte.MaxValue => new EInt(false, 8), 
							<= ushort.MaxValue => new EInt(false, 16), 
							<= uint.MaxValue => new EInt(false, 16), 
							_ => new EInt(false, 64), 
						};
					else
						return value switch {
							>= sbyte.MinValue => new EInt(true, 8), 
							>= short.MinValue => new EInt(true, 16), 
							>= int.MinValue => new EInt(true, 16), 
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