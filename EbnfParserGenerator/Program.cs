using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ParserCombinators;
using PrettyPrinter;

namespace EbnfParserGenerator;

enum PatternType {
	ByValue, 
	Named, 
	ValueOverride, 
	ValueList
}

static class Program {
	static string CurrentRuleName;
	static string ClassName = "Ast";
	static string StartRuleName;
	static readonly Dictionary<string, string> RuleTypes = [];
	
	static void Main(string[] args) {
		var ast = EbnfParser.Parse(File.ReadAllText(args[0]));
		if(ast == null)
			return;

		var ns = "Generated";
		if(ast.TypeDecl != null) {
			var td = ast.TypeDecl.Name;
			var i = td.LastIndexOf('.');
			ClassName = i == -1 ? td : td[(i + 1)..];
			if(i != -1) ns = td[..i];
		}

		Console.WriteLine("using ParserCombinators;");
		Console.WriteLine("using System.Collections.Generic;");
		Console.WriteLine($"namespace {ns};");
		Console.WriteLine($"public abstract partial class {ClassName} {{");
		var patternNameDeps = new Dictionary<string, List<string>>();
		var patternTypes = new Dictionary<string, PatternType>();
		ast.Rules.ForEach(rule => RuleTypes[rule.Name] = ClassName);
		ast.Rules.ForEach(rule => {
			var ruleName = rule.Name;
			var names = GetNamedElements(rule.Expression).Distinct().ToList();
			if(names.Contains("@") || rule.Operator == ":=")
				patternTypes[ruleName] = PatternType.ValueOverride;
			else if(names.Contains("@+"))
				patternTypes[ruleName] = PatternType.ValueList;
			else
				patternTypes[ruleName] = names.Count == 0 ? PatternType.ByValue : PatternType.Named;
			patternNameDeps[ruleName] = names;

			switch(patternTypes[ruleName]) {
				case PatternType.Named:
				case PatternType.ByValue:
					RuleTypes[ruleName] = $"{ClassName}.{ruleName}";
					break;
				case PatternType.ValueOverride:
					if(rule.Operator == ":=")
						RuleTypes[ruleName] = BuildType(rule.Expression);
					else
						RuleTypes[ruleName] = BuildType(FindElement("@", rule.Expression));
					break;
				case PatternType.ValueList:
					RuleTypes[ruleName] = $"List<{BuildType(FindElement("@+", rule.Expression))}>";
					break;
			}
		});

		string NewIf(string name) => ast.Rules.Any(x =>
                                         x.Name == name && patternTypes[name] != PatternType.ValueList &&
                                         patternTypes[name] != PatternType.ValueOverride)
            ? "new "
			: "";
		ast.Rules.ForEach(rule => {
			var ruleName = rule.Name;
			if(patternTypes[ruleName] == PatternType.ValueList ||
			   patternTypes[ruleName] == PatternType.ValueOverride)
				return;
			var names = patternNameDeps[ruleName];
			Console.WriteLine($"\tpublic partial class {rule.Name} : {ClassName} {{");
			if(names.Count == 0)
				Console.WriteLine($"\t\tpublic {NewIf("Value")}{BuildType(rule.Expression)} Value;");
			else
				foreach(var _name in names) {
					var name = _name.EndsWith("+") ? _name.Substring(0, _name.Length - 1) : _name;
					var type = BuildType(FindElement(_name, rule.Expression));
					if(_name.EndsWith("+"))
						type = $"List<{type}>";
					Console.WriteLine($"\t\tpublic {NewIf(name)}{type} {name};");
				}

			Console.WriteLine("\t}");
		});
		
		var deps = ast.Rules.Select(x => (x.Name, FindDeps(x.Expression).Distinct().ToList())).ToDictionary(x => x.Item1, x => x.Item2);
		var order = new List<string>();
		var forward = new List<string>();

		void BuildOrder(string name, List<string> ancestors) {
			foreach(var dep in deps[name]) {
				if(order.Contains(dep) || forward.Contains(dep)) continue;
				if(ancestors.Contains(dep))
					forward.Add(dep);
				else
					BuildOrder(dep, ancestors.Concat([name]).ToList());
			}
			order.Add(name);
		}

		StartRuleName = ast.Rules[0].Name;
		BuildOrder(StartRuleName, new List<string>());
		Console.WriteLine("\tstatic readonly Grammar Grammar;");
		Console.WriteLine($"\tstatic {ClassName}() {{");
		foreach(var elem in forward)
			Console.WriteLine($"\t\tvar (_{elem}, __{elem}_body) = Patterns.Forward();");
		foreach(var elem in order) {
			CurrentRuleName = elem;
			var rule = ast.Rules.First(x => x.Name == elem);
			string body = Generate(rule.Expression);
			var ptype = patternTypes[elem];
			switch(ptype) {
				case PatternType.Named:
					body = $"Patterns.Bind<{ClassName}.{elem}>({body})";
					break;
				case PatternType.ByValue:
					body = $"Patterns.Bind<{ClassName}.{elem}>(Patterns.With<{ClassName}.{elem}>((x, d) => x.Value = d, {body}))";
					break;
				case PatternType.ValueOverride:
					if(body.StartsWith("Patterns.PushValue("))
						body = body.Substring(19, body.Length - 19 - 1);
					else if(rule.Operator != ":=")
						body = $"Patterns.PopValue({body})";
					break;
				case PatternType.ValueList:
					body = $"Patterns.ValueList({body})";
					break;
				default: throw new NotImplementedException();
			}
			body = $"Patterns.Memoize({body})";
			if(forward.Contains(elem))
				Console.WriteLine($"\t\t__{elem}_body.Value = {body};");
			else
				Console.WriteLine($"\t\tvar _{elem} = {body};");
		}
		Console.WriteLine($"\t\tGrammar = new Grammar(_{StartRuleName});");
		Console.WriteLine("\t}");
		Console.WriteLine($"\tpublic static {ClassName}.{StartRuleName} Parse(string input) => ({ClassName}.{StartRuleName}) Grammar.Parse(input);");
		Console.WriteLine("}");
	}

	static EbnfParser.Element FindElement(string name, EbnfParser expr) {
		switch(expr) {
			case EbnfParser.Choice c: return c.Choices.Select(x => FindElement(name, x)).FirstOrDefault(x => x != null);
			case EbnfParser.Sequence s: return s.Items.Select(x => FindElement(name, x)).FirstOrDefault(x => x != null);
			case EbnfParser.Element e: return e.Name == name ? e : FindElement(name, e.Body);
			case EbnfParser.Optional o: return FindElement(name, o.Expression);
			case EbnfParser.RuleReference _:
			case EbnfParser.RegexLiteral _:
			case EbnfParser.StringLiteral _:
			case EbnfParser.End _:
				return null;
			default: throw new NotImplementedException(expr.ToPrettyString());
		}
	}

	static string Generate(EbnfParser expr) {
		switch(expr) {
			case EbnfParser.Choice c: return $"Patterns.IgnoreLeadingWhitespace(Patterns.Choice({string.Join(", ", c.Choices.Select(Generate))}))";
			case EbnfParser.Sequence s: return s.Items.Count == 1 ? Generate(s.Items[0]) : $"Patterns.TupleLooseSequence([{string.Join(", ", s.Items.Select(x => $"typeof({BuildType(x)})"))}], {string.Join(", ", s.Items.Select(Generate))})";
			case EbnfParser.Element e:
				var type = BuildType(e.Body);
				var body = Generate(e.Body);
				if(e.Modifiers == "*")
					body = $"Patterns.ZeroOrMore<{type}>(Patterns.IgnoreLeadingWhitespace({body}))";
				else if(e.Modifiers == "+")
					body = $"Patterns.OneOrMore<{type}>(Patterns.IgnoreLeadingWhitespace({body}))";
				var name = e.Name;
				switch(name) {
					case "@":
						return $"Patterns.PushValue({body})";
					case "@+":
						return $"Patterns.AddValue({body})";
					case null:
						return body;
					case {} when name.EndsWith('+'):
						name = name.Substring(0, name.Length - 1);
						return $"Patterns.With<{ClassName}.{CurrentRuleName}>((x, d) => (x.{name} = x.{name} ?? new List<{type}>()).Add(d), {body})";
					default:
						return
							$"Patterns.With<{ClassName}.{CurrentRuleName}>((x, d) => x.{name} = d, {body})";
				}
			case EbnfParser.RuleReference r: return "_" + r.Name;
			case EbnfParser.Optional o: return $"Patterns.IgnoreLeadingWhitespace(Patterns.Optional({Generate(o.Expression)}))";
			case EbnfParser.RegexLiteral r:
				var regex = r.Value[1..];
				var mpos = regex.LastIndexOf('/');
				// TODO: Process the modifiers and put them into the regex pattern
				regex = regex[..mpos];
				return $"Patterns.IgnoreLeadingWhitespace(Patterns.Regex({UnescapeRegex(regex)}))";
			case EbnfParser.StringLiteral s: return $"Patterns.IgnoreLeadingWhitespace(Patterns.Literal({UnescapeString(s.Value)}))";
			case EbnfParser.End: return "Patterns.IgnoreLeadingWhitespace(Patterns.End)";
			default: throw new NotImplementedException(expr.ToPrettyString());
		}
	}

	static string UnescapeRegex(string inp) {
		var ret = "^";
		for(var i = 0; i < inp.Length;) {
			switch(inp[i++]) {
				case '\\':
					switch(inp[i++]) {
						case '/': ret += "/"; break;
						case 'n': ret += "\n"; break;
						case 'r': ret += "\r"; break;
						case 't': ret += "\t"; break;
						case '\\': ret += "\\\\"; break;
						case var x: ret += "\\" + x; break;
					}
					break;
				case var x:
					ret += x;
					break;
			}
		}
		return ret.ToPrettyString();
	}

	static string UnescapeString(string inp) {
		inp = inp.Substring(1, inp.Length - 2);

		var ret = "";
		for(var i = 0; i < inp.Length;) {
			switch(inp[i++]) {
				case '\\':
					switch(inp[i++]) {
						case '"': ret += "\""; break;
						case '\'': ret += "'"; break;
						case 'n': ret += "\n"; break;
						case 'r': ret += "\r"; break;
						case 't': ret += "\t"; break;
						case '\\': ret += "\\"; break;
						case var x: throw new NotImplementedException($"Unsupported escape: '{x}'");
					}
					break;
				case var x:
					ret += x;
					break;
			}
		}
		return ret.ToPrettyString();
	}

	static string BuildType(EbnfParser expr) {
		switch(expr) {
			case EbnfParser.Choice c: {
				var choices = c.Choices.Select(BuildType).ToList();
				var first = choices.First();
				if(choices.Any(x => x != first)) {
					if(first == "string" || choices.Any(x => x == "string"))
						throw new NotImplementedException();
					return ClassName;
				}
				return first;
			}
			case EbnfParser.Sequence s:
				if(s.Items.Count == 1) return BuildType(s.Items[0]);
				return $"({string.Join(", ", s.Items.Select(BuildType))})";
			case EbnfParser.Element e:
				var type = BuildType(e.Body);
				if(e.Modifiers == "*" || e.Modifiers == "+")
					type = $"List<{type}>";
				return type;
			case EbnfParser.RuleReference r: return RuleTypes[r.Name];
			case EbnfParser.Optional o: return BuildType(o.Expression);
			case EbnfParser.RegexLiteral: case EbnfParser.StringLiteral: return "string";
			case EbnfParser.End: return "object";
			default: throw new NotImplementedException(expr.ToPrettyString());
		}
	}

	static IEnumerable<string> FindDeps(EbnfParser expr) =>
		expr switch {
			EbnfParser.Choice c => c.Choices.Select(FindDeps).SelectMany(x => x),
			EbnfParser.Sequence s => s.Items.Select(FindDeps).SelectMany(x => x),
			EbnfParser.Element e => FindDeps(e.Body),
			EbnfParser.RuleReference r => [r.Name],
			EbnfParser.Optional o => FindDeps(o.Expression),
			EbnfParser.RegexLiteral => [],
			EbnfParser.StringLiteral => [],
			EbnfParser.End => [],
			_ => throw new NotImplementedException(expr.ToPrettyString()),
		};

	static IEnumerable<string> GetNamedElements(EbnfParser expr) {
		return expr switch {
			EbnfParser.Choice c => c.Choices.Select(GetNamedElements).SelectMany(x => x),
			EbnfParser.Sequence s => s.Items.Select(GetNamedElements).SelectMany(x => x),
			EbnfParser.Element { Name: not null } e => new[] { e.Name }.Concat(GetNamedElements(e.Body)),
			EbnfParser.Element e => GetNamedElements(e.Body),
			EbnfParser.Optional o => GetNamedElements(o.Expression),
			EbnfParser.RuleReference => [],
			EbnfParser.RegexLiteral => [],
			EbnfParser.StringLiteral => [],
			EbnfParser.End e => [],
			_ => throw new NotImplementedException(expr.ToPrettyString()),
		};
	}
}