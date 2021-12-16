using System.Diagnostics;
using CoreArchCompiler;

namespace DamageGenerator; 

public class DmgDef : Def {
	DmgDef(string name, string dasm, PList decode, PList eval, IReadOnlyDictionary<string, EType> _locals) : base(name, dasm, decode, eval, _locals) {
		
	}

	public static DmgDef Parse(PList def) {
		if(def[0] is not PName("def")) throw new Exception();
		if(def[1] is not PName name) throw new Exception();
		if(def[2] is not PString bitstr) throw new Exception();
		if(def[3] is not PString disasm) throw new Exception();
		if(def[4] is not PList names) throw new Exception();
		if(def[5] is not PList cycles || cycles[0] is not PName("cycles") || cycles[1] is not PInt(_)) throw new Exception();
		if(def[6] is not PList decode) throw new Exception();
		if(def[7] is not PList eval) throw new Exception();

		var bits = bitstr.String.Replace(" ", "");
		Console.WriteLine(bits);
		Debug.Assert(bits.Length % 8 == 0);

		var locals = new Dictionary<string, EType>();
		
		return new DmgDef(name, disasm, decode, eval, locals);
	}
}