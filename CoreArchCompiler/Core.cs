using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MoreLinq;
using PrettyPrinter;

namespace CoreArchCompiler; 

public enum ContextTypes {
	Disassembler, 
	Interpreter, 
	Recompiler
}
	
public class Capture {
	public static readonly List<Capture> UnassignedCaptures = new();
	public readonly string Name, RawName;
	readonly Action<Func<PList, ExecutionState, dynamic>> Assign;

	public Capture(string type, string name, Action<Func<PList, ExecutionState, dynamic>> assign) {
		var st = new StackTrace();
		var frame = st.GetFrame(2);
		var method = frame?.GetMethod() ?? throw new Exception();
		RawName = name;
		Name = type + " " + name + " in " + method.DeclaringType.Name;
		Assign = assign;
		UnassignedCaptures.Add(this);
	}
		
	public void Interpret(Func<PList, ExecutionState, dynamic> func) {
		UnassignedCaptures.Remove(this);
		Assign(func);
	}

	public void NoInterpret() {
		UnassignedCaptures.Remove(this);
		Assign((_, __) => throw new BailoutException());
	}
}

public abstract class Builtin {
	public static EType TypeFromName(PTree expr) {
		if(expr is not PName name) throw new NotSupportedException($"Attempted to make type from expr {expr.ToPrettyString()}");

		var ns = name.Name;
		if(ns[0] == 'f') return new EFloat(int.Parse(ns[1..]));
		if(ns == "vec") return EType.Vector;
		return ns[0] == 'i' ? new EInt(true, int.Parse(ns[1..])) : new EInt(false, int.Parse(ns[1..]));
	}
		
	public static Capture Statement(string name, Func<PList, EType> signature, Action<CodeBuilder, PList> compiletime, Action<CodeBuilder, PList> runtime = null) {
		if(Core.Statements.ContainsKey(name)) throw new Exception();
		return new Capture("Statement", name, func => Core.Statements[name] = (signature, compiletime, runtime ?? compiletime, func));
	}
	public static Capture Expression(string name, Func<PList, EType> signature, Func<PList, string> compiletime, Func<PList, string> runtime = null) {
		if(Core.Expressions.ContainsKey(name)) throw new Exception();
		return new Capture("Expression", name, func => Core.Expressions[name] = (signature, compiletime, runtime ?? compiletime, func));
	}
	public static Capture BranchExpression(string name, Func<PList, EType> signature, Func<PList, string> compiletime, Func<PList, string> runtime = null) {
		if(Core.Expressions.ContainsKey(name)) throw new Exception();
		var oc = compiletime;
		compiletime = list => {
			Core.HasBranch = true;
			return oc(list);
		};
		var or = runtime ?? oc;
		runtime = list => {
			Core.HasBranch = true;
			return or(list);
		};
		return new Capture("Expression", name, func => Core.Expressions[name] = (signature, compiletime, runtime, func));
	}
	public static Capture Expression(IEnumerable<string> names, Func<PList, EType> signature, Func<PList, string> compiletime, Func<PList, string> runtime = null) {
		var nameList = names.ToList();
		return new Capture("Expressions", $"[ {string.Join(" ", nameList)} ]",
			func => MoreEnumerable.ForEach(nameList, name => Expression(name, signature, compiletime, runtime).Interpret(func)));
	}

	public static void Interpret(string name, Func<PList, ExecutionState, dynamic> func) {
		var captures = Capture.UnassignedCaptures.Where(x => x.RawName == name).ToList();
		Debug.Assert(captures.Count != 0);
		foreach(var capture in captures)
			capture.Interpret(func);
	}

	public static string TempName() => Core.TempName();

	public static string GenerateType(EType type) => Core.GenerateType(type);
	public static string GenerateExpression(PTree expr, bool lhs = false) => Core.GenerateExpression(expr, lhs);

	public static void GenerateStatement(CodeBuilder c, PList list) => Core.GenerateStatement(c, list);
		
	public abstract void Define();
}

public class Core {
	public static string NextLabel;
	public static bool HasBranch;

	public static readonly Dictionary<string, (Func<PList, EType> Signature, Action<CodeBuilder, PList> CompileTime,
			Action<CodeBuilder, PList> RunTime, Func<PList, ExecutionState, dynamic> Execute)>
		Statements = new();

	public static readonly Dictionary<string, (Func<PList, EType> Signature, Func<PList, string> CompileTime,
			Func<PList, string> RunTime, Func<PList, ExecutionState, dynamic> Execute)>
		Expressions =
			new Dictionary<string, (Func<PList, EType> Signature, Func<PList, string> CompileTime,
				Func<PList, string> RunTime, Func<PList, ExecutionState, dynamic>)>();

	public static void InferList(PList list) {
		switch(list[0]) {
			case PName(var name) when Statements.ContainsKey(name):
				foreach(var elem in list.Skip(1))
					if(elem is PList sublist)
						InferList(sublist);
				list.Type = Statements[name].Signature(list);
				break;
			default:
				InferExpression(list);
				break;
		}
	}

	public static EType InferExpression(PTree tree) {
		if(tree.Type.Runtime) return tree.Type;
		switch(tree) {
			case PList list:
				var set = false;
				foreach(var elem in list)
					if(InferExpression(elem).Runtime)
						set = true;
				return list.Type = set ? list.Type.AsRuntime() : list.Type;
			default:
				return tree.Type;
		}
	}
		
	public static Def InferRuntime(Def def) {
		InferList(def.Decode);
		InferList(def.Eval);
		return def;
	}

	static Core() {
		MoreEnumerable.ForEach(AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
			.Where(x => x.IsSubclassOf(typeof(Builtin))), x => ((Builtin) Activator.CreateInstance(x)).Define());

		if(Capture.UnassignedCaptures.Count != 0) {
			Console.WriteLine("The following captures have not been assigned:");
			foreach(var capture in Capture.UnassignedCaptures)
				Console.WriteLine($"- {capture.Name}");
			Console.WriteLine("---Press Enter To Continue---");
			Console.ReadLine();
			//return;
			while(Capture.UnassignedCaptures.Count != 0)
				foreach(var capture in Capture.UnassignedCaptures) {
					capture.Interpret((_, __) => throw new NotImplementedException(capture.Name));
					break;
				}
		}
	}

	public static List<Def> ParseSpec(string spec, ExecutionState es, Func<PList, Def> defCtor) {
		var ptree = ListParser.Parse(spec);
		ptree = MacroProcessor.Rewrite(ptree);
		foreach(var tle in ptree) {
			try {
				es.Evaluate(tle);
			} catch(BailoutException) {}
		}
		return Def.ParseAll(ptree, defCtor).Select(InferRuntime).ToList();
	}
		
	public static ContextTypes Context;
	static int TempI;

	public static string TempName() => $"temp_{TempI++}";

	public static void GenerateStatement(CodeBuilder c, PList list) {
		if(Context == ContextTypes.Recompiler && list.Type.Runtime) {
			GenerateRuntimeStatement(c, list);
			return;
		}
		switch(list[0]) {
			case PName(var name) when Statements.ContainsKey(name):
				Statements[name].CompileTime(c, list);
				break;
			case PName name:
				c += $"{GenerateExpression(list)};";
				break;
			default:
				throw new NotSupportedException($"Non-name for first element of list {list.ToPrettyString()}");
		}
	}

	public static void GenerateRuntimeStatement(CodeBuilder c, PList list) {
		Debug.Assert(Context == ContextTypes.Recompiler);
		switch(list[0]) {
			case PName(var name) when Statements.ContainsKey(name):
				Statements[name].RunTime(c, list);
				break;
			case PName name:
				c += $"{GenerateExpression(list)};";
				break;
			default:
				throw new NotSupportedException($"Non-name for first element of list {list.ToPrettyString()}");
		}
	}

	public static string ToHex(long value) => value < 0 ? $"-0x{-value:X}" : $"0x{value:X}";

	public static string GenerateExpression(PTree v, bool lhs = false) {
		return v switch {
			PName name => name.Name,
			PInt value => value.Type switch {
				EInt(false, 8) => $"(byte) {ToHex(value.Value)}",
				EInt(true, 8) => $"(sbyte) {ToHex(value.Value)}",
				EInt(false, 16) => $"(ushort) {ToHex(value.Value)}",
				EInt(true, 16) => $"(short) {ToHex(value.Value)}",
				EInt(false, 32) => $"{ToHex(value.Value)}U",
				EInt(true, 32) => $"{ToHex(value.Value)}",
				EInt(false, 64) => ToHex(value.Value) + "UL", 
				EInt(true, 64) => ToHex(value.Value) + "L", 
				_ => throw new NotImplementedException()
			}, 
			PString str => str.String.ToPrettyString(),
			PList list => GenerateListExpression(list, lhs: lhs),
			_ => throw new NotImplementedException()
		};
	}

	public static string GenerateType(EType type) {
		string __GenerateType() {
			switch(type) {
				case null: return "void";
				case EUnit: return "void";
				case EString: return "string";
				case EInt i:
					switch(i.Width) {
						case 1: return "uint"; // TODO: Figure out how to use real bools here!
						case > 64: return i.Signed ? "Int128" : "UInt128";
						case > 32: return i.Signed ? "long" : "ulong";
						case > 16: return i.Signed ? "int" : "uint";
						case > 8: return i.Signed ? "short" : "ushort";
						default: return i.Signed ? "sbyte" : "byte";
					}
				case EFloat f:
					switch(f.Width) {
						case > 64: return "Vector128<float>";
						case > 32: return "double";
						default: return "float";
					}
				case EVector: return "Vector128<float>";
				default: throw new NotImplementedException($"Type {type}");
			}
		}

		return Context == ContextTypes.Recompiler && type.Runtime
			? $"LlvmRuntimeValue<{__GenerateType()}>"
			: __GenerateType();
	}

	public static string GenerateListExpression(PList list, bool lhs = false) {
		if(Context == ContextTypes.Recompiler && list.Type.Runtime) {
			var expr = GenerateBaseListRuntimeExpression(list);
			return lhs || list.Type is EUnit ? expr : $"({GenerateType(list.Type)}) ({expr})";
		} else {
			var expr = GenerateBaseListExpression(list);
			return lhs || list.Type is EUnit ? expr : $"({GenerateType(list.Type)}) ({expr})";
		}
	}

	public static string GenerateBaseListExpression(PList list) {
		switch(list[0]) {
			case PName name when Expressions.ContainsKey(name): return Expressions[name].CompileTime(list);
			case PName name: throw new NotImplementedException($"Unknown name for GenerateListExpression: {name}");
			default: throw new NotSupportedException($"Non-name for first element of list {list.ToPrettyString()}");
		}
	}

	public static string GenerateBaseListRuntimeExpression(PList list) {
		Debug.Assert(Context == ContextTypes.Recompiler);
		switch(list[0]) {
			case PName name when Expressions.ContainsKey(name): return Expressions[name].RunTime(list);
			case PName name:
				throw new NotImplementedException($"Unknown name for GenerateRuntimeListExpression: {name}");
			default: throw new NotSupportedException($"Non-name for first element of list {list.ToPrettyString()}");
		}
	}

	public static void CleanupCode(string genpath) {
		if(!RunCommand("jb", "cleanupcode", "-v").Contains("JetBrains")) {
			Console.WriteLine("WARNING: JetBrains code cleanup utility not installed. Run `dotnet tool install -g --add-source . JetBrains.ReSharper.GlobalTools`");
			return;
		}

		var slnDir = Path.GetFullPath("../");
		var sln = Directory.GetFiles(slnDir, "*.sln")[0];
		var relGenPath = Path.GetFullPath(genpath)[slnDir.Length..];
		while(true) {
			Console.WriteLine("Running code cleanup...");
			var output = RunCommand("jb", "cleanupcode", sln, "--profile=Autogen Cleanup", "--include=" + relGenPath + "/*.cs");
			if(!output.Contains("Code cleanup hanged up"))
				break;
		}
	}

	static string RunCommand(string command, params string[] args) {
		var process = new Process {
			StartInfo = {
				FileName = command, 
				RedirectStandardError = true,
				RedirectStandardOutput = true
			}
		};
		args.ForEach(process.StartInfo.ArgumentList.Add);

		var ret = "";
		void Capture(object sender, DataReceivedEventArgs evt) => ret += evt.Data + "\n";

		process.OutputDataReceived += Capture;
		process.ErrorDataReceived += Capture;

		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		process.WaitForExit();

		return ret;
	}
}