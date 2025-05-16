namespace Aarch64Generator;

using System.Diagnostics;
using CoreArchCompiler;

public class Aarch64Def : Def {
	public readonly uint Mask, Match;
	public readonly IReadOnlyDictionary<string, (int Bits, int Shift)> Fields;

	Aarch64Def(string name, uint mask, uint match, 
		Dictionary<string, (int Size, int Shift)> fields, 
		PTree dasm, PList decode, PList eval,
		IReadOnlyDictionary<string, EType> _locals
	) : base(name, dasm, decode, eval, _locals) {
		Mask = mask;
		Match = match;
		Fields = fields;
	}

	public static Aarch64Def Parse(PList def) {
		if(def[0] is not PName("def")) throw new();
		if(def[1] is not PTree _name) throw new();
		if(def[2] is not PTree _bitstr) throw new();
		if(def[3] is not PTree disasm) throw new();
		if(def[4] is not PList names) throw new();
		if(def[5] is not PList decode) throw new();
		if(def[6] is not PList eval) throw new();

		var name = _name switch {
			PName(var x) => x, 
			_ => (string) new ExecutionState().Evaluate(_name)
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
		
		return new(name, mask, match, fields, disasm, decode, eval, locals);
	}
}