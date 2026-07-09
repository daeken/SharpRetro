using CoreArchCompiler;

namespace XFusionGenerator;

/// The census move applied to lowering: walk EVERY template's eval body through
/// IlLower with synthetic operand binds; tally what lowers vs what throws, by
/// missing construct. Converts "what's left for M1" from guesswork to a list.
/// Run: dotnet run --project XFusionGenerator -- --lower-census <features...>
public static class LowerCensus {
	public static void Run(List<XFusionDef.Template> templates) {
		var ok = new List<string>();
		var fails = new Dictionary<string, List<string>>();  // reason -> mnemonics

		foreach(var t in templates) {
			// synthetic binds: every param a 32-bit reg (widest structural coverage
			// without operand-class knowledge; mem/imm variants exercise the same body)
			var binds = new Dictionary<string, OperandBind>();
			var regs = new[] { "X86_RAX", "X86_RBX", "X86_RCX", "X86_RDX" };
			for(var i = 0; i < t.Params.Count; i++)
				binds[t.Params[i]] = new OperandBind.Reg(regs[i % regs.Length], 32);
			try {
				IlLower.Lower(t.Params, t.Eval.Skip(1), binds, 32);  // Eval = (block ...)
				ok.Add(t.Mnemonic);  // empty body (NOP) is a valid lowering
			} catch(NotSupportedException e1) when (e1.Message.Contains("addr-of")) {
				// LEA-shaped: the operand is an M-class — retry with a mem bind
				try {
					var membinds = new Dictionary<string, OperandBind>(binds);
					membinds[t.Params[^1]] = new OperandBind.Mem(new Ilx.ReadReg("X86_RCX"), 32);
					IlLower.Lower(t.Params, t.Eval.Skip(1), membinds, 32);
					ok.Add(t.Mnemonic);
				} catch(Exception e2) { Tally(fails, e2, t.Mnemonic); }
			} catch(Exception e) {
				Tally(fails, e, t.Mnemonic);
			}
		}

		static void Tally(Dictionary<string, List<string>> fails, Exception e, string mnem) {
			var key = e.Message.Length > 60 ? e.Message[..60] : e.Message;
			if(!fails.TryGetValue(key, out var list)) fails[key] = list = [];
			list.Add(mnem);
		}

		Console.WriteLine($"\nlowering census: {ok.Count}/{templates.Count} templates lower");
		Console.WriteLine("\nblockers by construct:");
		foreach(var (reason, mnems) in fails.OrderByDescending(x => x.Value.Count))
			Console.WriteLine($"  {mnems.Count,4}  {reason}   e.g. {string.Join(" ", mnems.Take(6))}");
	}
}
