using CoreArchCompiler;

namespace XFusionGenerator;

/// <summary>
/// x86-family instruction definition. XF-1 (in design) gives this the real shape:
/// prefix* + opcode[1-3] + ModRM? + SIB? + disp? + imm?, with ModRM/imm presence
/// derived from Intel-SDM operand notation (Eb Gb Ev Gv Ib Iz ...).
/// The variable-length ancestor is DmgDef (byte-oriented), not Aarch64Def (fixed-32).
/// </summary>
public class XFusionDef : Def {
	XFusionDef(string name, PTree dasm, PList decode, PList eval,
		IReadOnlyDictionary<string, EType> locals)
		: base(name, dasm, decode, eval, locals) { }

	public static XFusionDef Parse(PList def) {
		if(def[0] is not PName("def")) throw new();
		if(def[1] is not PName(var name)) throw new($"def name must be a plain name, got {def[1]}");
		throw new NotImplementedException($"XFusionDef.Parse ({name}): XF-1 lands the variable-length x86 def shape");
	}
}
