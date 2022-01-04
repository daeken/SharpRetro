using System.Diagnostics;
using CoreArchCompiler;
using MoreLinq.Extensions;

namespace SharpStationGenerator; 

public class MipsDef : Def {
	public readonly uint Mask, Match;
	public readonly IReadOnlyDictionary<string, (int Bits, int Shift)> Fields;

	MipsDef(string name, uint mask, uint match, 
		Dictionary<string, (int Size, int Shift)> fields, 
		PTree dasm, PList decode, PList eval,
		IReadOnlyDictionary<string, EType> _locals
	) : base(name, dasm, decode, eval, _locals) {
		Mask = mask;
		Match = match;
		Fields = fields;
	}

	public static MipsDef Parse(PList def) {
		if(def[0] is not PName("def")) throw new Exception();
		if(def[1] is not PTree _name) throw new Exception();
		if(def[2] is not PTree _bitstr) throw new Exception();
		if(def[3] is not PTree disasm) throw new Exception();
		if(def[4] is not PList names) throw new Exception();
		if(def[5] is not PList decode) throw new Exception();
		if(def[6] is not PList eval) throw new Exception();

		var name = _name switch {
			PName(var x) => x, 
			var x => (string) new ExecutionState().Evaluate(x)
		};
		Console.WriteLine(name);

		var fieldNames = names.Skip(1).Select(x => (PList) x)
			.Select(x => (((PName) x[1]).Name, ((PName) x[0]).Name)).ToDictionary();
		
		var fields = new Dictionary<string, (int, int)>();
		var mask = 0U;
		var match = 0U;
		var bitstring = ((string) new ExecutionState().Evaluate(_bitstr)).Replace(" ", "");
		Debug.Assert(bitstring.Length == 32);
		for(var i = 0; i < 32; ++i) {
			var bit = 31 - i;
			mask <<= 1;
			match <<= 1;
			switch(bitstring[i]) {
				case '0':
					mask |= 1;
					break;
				case '1':
					mask |= 1;
					match |= 1;
					break;
				case '#': break;
				case var x:
					var field = fieldNames[x.ToString()];
					fields[field] = fields.ContainsKey(field) ? (fields[field].Item1 + 1, bit) : (1, bit);
					break;
			}
		}
		
		var locals = new Dictionary<string, EType>();
		foreach(var (fname, (bits, _)) in fields)
			locals[fname] = new EInt(false, bits).AsCompiletime();
		
		return new MipsDef(name, mask, match, fields, disasm, decode, RewriteEval(eval), locals);
	}

	static PList RewriteEval(PList eval) {
		var regRefs = new HashSet<PTree>();
		var hasDoLoad = false;
		eval.WalkLeaves(x => {
			if(x is not PList pl) return;
			if(pl[0] is PName("reg"))
				regRefs.Add(pl[1]);
			else if(pl[0] is PName("do-load"))
				hasDoLoad = true;
		});

		var neval = new PList { new PName("block") };
		foreach(var elem in regRefs)
			neval.Add(new PList { new PName("read-absorb"), elem });

		var tregs = new Dictionary<PTree, string>();
		PTree RewriteRegs(PTree tl) {
			if(tl is not PList list) return tl;

			switch(list[0]) {
				case PName("=") or PName("defer="):
					return new PList { list[0], list[1], RewriteRegs(list[2]) };
				case PName("reg"):
					if(!tregs.TryGetValue(list, out var tn))
						tregs[list] = tn = Core.TempName();
					return new PName(tn);
				case PName("do-load"):
					return new PList { new PName("do-load"), list[1], RewriteRegs(new PList { new PName("reg"), list[1] })};
				default:
					return new PList(list.Select(RewriteRegs));
			}
		}
		
		eval = RewriteRegs(eval) as PList;
		if(!hasDoLoad)
			eval = new PList { new PName("block"), new PList { new PName("do-lds") }, eval };
		if(tregs.Count != 0)
			eval = new PList { new PName("mlet"), new PList(
					tregs.Select(x => new[] { new PName(x.Value), x.Key }).SelectMany(x => x)
				), eval };
		neval.Add(eval);
		return neval;
	}
}