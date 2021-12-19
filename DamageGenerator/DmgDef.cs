using System.Diagnostics;
using CoreArchCompiler;
using MoreLinq.Extensions;

namespace DamageGenerator; 

public class DmgDef : Def {
	public readonly int Size;
	public readonly Dictionary<int, (byte Mask, byte Match)> MatchBytes;
	public readonly Dictionary<string, (int Byte, int Size, int Shift)> Fields;

	DmgDef(string name, int size, Dictionary<int, (byte Mask, byte Match)> matchBytes, 
		Dictionary<string, (int Byte, int Size, int Shift)> fields, 
		PTree dasm, PList decode, PList eval,
		IReadOnlyDictionary<string, EType> _locals
	) : base(name, dasm, decode, eval, _locals) {
		Size = size;
		MatchBytes = matchBytes;
		Fields = fields;
	}

	public static DmgDef Parse(PList def) {
		if(def[0] is not PName("def")) throw new Exception();
		if(def[1] is not PTree _name) throw new Exception();
		if(def[2] is not PTree _bitstr) throw new Exception();
		if(def[3] is not PTree disasm) throw new Exception();
		if(def[4] is not PList names) throw new Exception();
		if(def[5] is not PList cycles || cycles[0] is not PName("cycles")) throw new Exception();
		if(def[6] is not PList decode) throw new Exception();
		if(def[7] is not PList eval) throw new Exception();

		var name = _name switch {
			PName(var x) => x, 
			var x => (string) new ExecutionState().Evaluate(x)
		};
		Console.WriteLine(name);

		var fields = new Dictionary<string, (int Byte, int Size, int Shift)>();
		
		var fieldNames = names.Skip(1).Select(x => (PList) x)
			.Select(x => (((PName) x[1]).Name, ((PName) x[0]).Name)).ToDictionary();

		var bitstr = ((string) new ExecutionState().Evaluate(_bitstr)).Replace(" ", "");
		Debug.Assert(bitstr.Length % 8 == 0);
		var matchBytes = new Dictionary<int, (byte Mask, byte Match)>();
		for(var bi = 0; bi < bitstr.Length / 8; ++bi) {
			var mask = (byte) 0;
			var match = (byte) 0;
			var bbits = bitstr[(bi * 8)..(bi * 8 + 8)];
			for(var i = 0; i < 8; ++i) {
				var bit = 7 - i;
				mask <<= 1;
				match <<= 1;
				switch(bbits[i]) {
					case '0':
						mask |= 1;
						break;
					case '1':
						mask |= 1;
						match |= 1;
						break;
					case var x:
						var field = fieldNames[x.ToString()];
						if(fields.TryGetValue(field, out var fld)) {
							Debug.Assert(fld.Byte == bi); // TODO: Support fields spanning bytes
							fields[field] = (bi, fld.Size + 1, bit);
						} else
							fields[field] = (bi, 1, bit);
						break;
				}
			}

			if(mask != 0) matchBytes[bi] = (mask, match);
		}
		
		var locals = new Dictionary<string, EType>();
		foreach(var (fname, (_, bits, _)) in fields)
			locals[fname] = new EInt(false, bits);
		
		eval = new PList(new PTree[] { new PName("block"), eval, cycles });

		return new DmgDef(name, bitstr.Length / 8, matchBytes, fields, disasm, decode, eval, locals);
	}
}