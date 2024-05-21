using System;
using System.Collections.Generic;
using System.Linq;
using PrettyPrinter;

namespace CoreArchCompiler; 

public class MacroProcessor {
	public static PList Rewrite(PList top) {
		var macros = new Dictionary<string, List<(List<string>, PTree)>>();
		foreach(var elem in top) {
			if(elem is not PList list || list[0] is not PName("defm")) continue;
			if(list[1] is not PName(var name)) throw new();
			var varnames = ((PList) list[2]).Select(x => ((PName) x).Name).ToList();
			if(!macros.TryGetValue(name, out var mlist))
				mlist = macros[name] = [];
			mlist.Add((varnames, list[3]));
		}

		PTree Repl(string macroName, PTree elem, Dictionary<string, PTree> replacements) {
			switch(elem) {
				case PList list: return new PList(list.Select(x => Repl(macroName, x, replacements)));
				case PName(var name):
					if(replacements.TryGetValue(name, out var repl))
						return repl;
					if(name[0] == '$')
						return new PName($"__macro_{macroName.Replace("-", "_")}_{name[1..]}");
					return elem;
				default: return elem;
			}
		}

		PTree Sub(PTree elem) {
			if(elem is PName(var pname)) return new PName(pname);
			if(elem is not PList list) return elem;
			if(list.Count == 0 || list[0] is not PName(var name) || !macros.TryGetValue(name, out var mlist)) return new PList(list.Select(Sub));
			foreach(var (args, block) in mlist) {
				if(args.Count != list.Count - 1) continue;
				return Sub(Repl(name, block, args.Select((vn, i) => (vn, list[i + 1])).ToDictionary()));
			}

			throw new NotSupportedException($"No overload of macro {name.ToPrettyString()} takes {list.Count - 1} arguments");
		}
			
		return (PList) Sub(top);
	}
}