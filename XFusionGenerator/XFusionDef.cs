using CoreArchCompiler;

namespace XFusionGenerator;

/// Mirror of XFusionCpu.OpcodeMap (same member names — generated code references the
/// runtime one by name). No project ref: a broken Generated/ must never prevent the
/// generator from rebuilding to fix it (Damage precedent).
public enum OpcodeMap { OneByte, TwoByte0F, ThreeByte0F38, ThreeByte0F3A }

/// <summary>
/// One x86 encoding = one XFusionDef. Produced by joining an `encoding` form to its
/// `instruction` semantics template by mnemonic (parse-level, not defm — MacroProcessor
/// collects defms pre-expansion with literal names, so defm-emitting-defm can't fire).
///
/// (instruction ADD (lval rval) "add $lval, $rval" (...eval))
/// (encoding ADD (Eb Gb) (0x00))
/// (encoding ADD (Ev Iz) (0x81 /0))
/// </summary>
public class XFusionDef : Def {
	public OpcodeMap Map;              // one-byte, 0F, 0F38, 0F3A
	public byte Opcode;
	public int RegExtension = -1;      // /0../7 constraint on ModRM.reg, or -1
	public bool D64;                   // SDM D64 attribute: default operand 64 in long mode, 32 unavailable (PUSH/POP/CALL/JMP class)
	public string MandatoryPrefix;     // null | "rep" (F3) | "repnz" (F2) | "opsize" (66) — the SSE/PAUSE discriminator; prefix-matched rows dispatch before the bare row
	public bool PlusR;                 // +r form: opcode occupies [Opcode, Opcode+7], reg = (op&7)|REX.B<<3
	public List<OperandSpec> Operands; // positional, matches template params
	public List<string> ParamNames;    // template params ("lval", "rval")
	public string Mnemonic;
	public PTree Dasm;                 // the REAL dasm tree (base.Disassembly gets a placeholder — Def's
	                                   // InferType can't type x86 dasm vocab like (wname …); same deferral as SemanticsEval)
	public PList SemanticsEval;        // template eval body — typed+compiled at the interpreter
	                                   // milestone (needs XFusion Builtins for Core.Statements/Expressions);
	                                   // base.Eval gets an empty block until then so Def's InferType passes.

	public bool NeedsModRm => RegExtension >= 0 || Operands.Any(o => o.NeedsModRm);

	XFusionDef(string name, PTree dasm, PList decode, PList eval,
		IReadOnlyDictionary<string, EType> locals)
		: base(name, dasm, decode, eval, locals) { }

	/// Not used — XFusion collects via CollectAll (instruction/encoding forms), but
	/// Core.ParseSpecFile's Def-ctor hook needs a signature match for plain (def ...).
	public static XFusionDef Parse(PList def) =>
		throw new NotSupportedException("XFusion uses (instruction ...) + (encoding ...), not bare (def ...)");

	public record Template(string Mnemonic, List<string> Params, PTree Dasm, PList Eval);

	public static (List<Template> Templates, List<XFusionDef> Defs) CollectAll(PList top) {
		var templates = new Dictionary<string, Template>();
		var defs = new List<XFusionDef>();

		foreach(var elem in top) {
			if(elem is not PList pl || pl.Count == 0) continue;
			switch(pl[0]) {
				case PName("instruction"): {
					// (instruction MNEM (params...) "dasm" ...eval-forms)
					if(pl[1] is not PName(var mnem)) throw new NotSupportedException($"instruction name: {pl[1]}");
					if(pl[2] is not PList plist) throw new NotSupportedException($"instruction params: {pl}");
					var ps = plist.Select(x => ((PName) x).Name).ToList();
					var dasm = pl[3];
					var eval = new PList(new PTree[] { new PName("block") }.Concat(pl.Skip(4)));
					if(templates.ContainsKey(mnem))
						throw new NotSupportedException($"duplicate instruction template {mnem}");
					templates[mnem] = new(mnem, ps, dasm, eval);
					break;
				}
				case PName("encoding"): {
					// (encoding MNEM (specs...) (opcode-bytes... [/N]))
					if(pl[1] is not PName(var mnem)) throw new NotSupportedException($"encoding name: {pl[1]}");
					if(pl[2] is not PList specList) throw new NotSupportedException($"encoding operands: {pl}");
					if(pl[3] is not PList opcList) throw new NotSupportedException($"encoding opcodes: {pl}");
					if(!templates.TryGetValue(mnem, out var tmpl))
						throw new NotSupportedException($"encoding {mnem} has no instruction template (define instruction before encoding)");

					var specs = specList.Select(OperandSpec.Parse).ToList();
					// Operand count must match template params (FixedInt/FixedReg still bind a param).
					if(specs.Count != tmpl.Params.Count)
						throw new NotSupportedException(
							$"encoding {mnem} ({string.Join(" ", specs.Select(x => x.Text))}) has {specs.Count} operands; template takes {tmpl.Params.Count}");

					var (map, opcode, regExt, d64, mprefix, plusR) = ParseOpcodes(opcList, mnem);
					var def = Build(tmpl, specs, map, opcode, regExt, d64, mprefix, plusR);
					defs.Add(def);
					break;
				}
			}
		}
		return (templates.Values.ToList(), defs);
	}

	static (OpcodeMap, byte, int, bool, string, bool) ParseOpcodes(PList opcList, string mnem) {
		var bytes = new List<byte>();
		var regExt = -1;
		var d64 = false;
		var plusR = false;
		string mprefix = null;
		foreach(var item in opcList)
			switch(item) {
				case PInt(var v):
					bytes.Add((byte) v);
					break;
				case PName(var n) when n.StartsWith("/") && n.Length == 2 && n[1] is >= '0' and <= '7':
					regExt = n[1] - '0';
					break;
				case PName("d64"):
					d64 = true;
					break;
				case PName("+r"):
					plusR = true;
					break;
				case PName("rep") or PName("repnz") or PName("opsize") when bytes.Count == 0:
					mprefix = ((PName) item).Name;
					break;
				default:
					throw new NotSupportedException($"opcode element {item} in encoding {mnem}");
			}

		return bytes switch {
			[var one] => (OpcodeMap.OneByte, one, regExt, d64, mprefix, plusR),
			[0x0F, var two] => (OpcodeMap.TwoByte0F, two, regExt, d64, mprefix, plusR),
			[0x0F, 0x38, var three] => (OpcodeMap.ThreeByte0F38, three, regExt, d64, mprefix, plusR),
			[0x0F, 0x3A, var three] => (OpcodeMap.ThreeByte0F3A, three, regExt, d64, mprefix, plusR),
			_ => throw new NotSupportedException($"opcode byte pattern in encoding {mnem}: [{string.Join(", ", bytes.Select(b => $"0x{b:X2}"))}]")
		};
	}

	static XFusionDef Build(Template tmpl, List<OperandSpec> specs, OpcodeMap map, byte opcode, int regExt, bool d64, string mprefix, bool plusR) {
		// Locals: template params are bound at decode time; typing is resolved during
		// generation (width expansion). For now register params as compile-time-unknown ints.
		var locals = new Dictionary<string, EType>();
		foreach(var p in tmpl.Params)
			locals[p] = new EInt(false, 64).AsCompiletime();

		var name = $"{tmpl.Mnemonic}-{string.Join("-", specs.Select(s => s.Text.Replace("/", "x")))}";
		return new XFusionDef(name, new PString(name), new PList { new PName("block") }, new PList { new PName("block") }, locals) {
			Dasm = tmpl.Dasm,
			SemanticsEval = tmpl.Eval,
			Map = map,
			Opcode = opcode,
			RegExtension = regExt,
			D64 = d64,
			MandatoryPrefix = mprefix,
			PlusR = plusR,
			Operands = specs,
			ParamNames = tmpl.Params,
			Mnemonic = tmpl.Mnemonic,
		};
	}
}
