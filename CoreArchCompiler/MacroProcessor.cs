using System;
using System.Collections.Generic;
using System.Linq;
using PrettyPrinter;

namespace CoreArchCompiler; 

public static class MacroProcessor {
	public static PList Rewrite(PList top) {
		var tempI = 0;
		var macros = new Dictionary<string, List<(List<string>, PTree)>>();
		foreach(var elem in top) {
			if(elem is not PList list || list[0] is not PName("defm")) continue;
			if(list[1] is not PName(var name)) throw new();
			var varnames = ((PList) list[2]).Select(x => ((PName) x).Name).ToList();
			if(varnames.SkipLast(1).Any(x => x.StartsWith("...")))
				throw new NotSupportedException($"Variadic macro arg must be last: {name}");
			if(!macros.TryGetValue(name, out var mlist))
				mlist = macros[name] = [];
			var body = list.Count == 4 ? list[3] : new PList(new PTree[] { new PName("inline-block") }.Concat(list.Skip(3)));
			mlist.Add((varnames, body));
		}

		PTree Repl(string macroName, PTree elem, Dictionary<string, PTree> replacements, Dictionary<string, int> uids) {
			switch(elem) {
				case PList list: return new PList(list.Select(x => Repl(macroName, x, replacements, uids)));
				case PName(var name):
					if(replacements.TryGetValue(name, out var repl))
						return repl;
					if(name[0] == '$') {
						var tname = name[1..];
						var uid = uids.TryGetValue(tname, out var tuid) ? tuid : uids[tname] = tempI++;
						return new PName($"__macro_{macroName.Replace("-", "_")}_{tname}_{uid}");
					}
					return elem;
				default: return elem;
			}
		}

		PTree Sub(PTree elem) {
			if(elem is PName(var pname)) return new PName(pname);
			if(elem is not PList list) return elem;
			if(list.Count == 0 || list[0] is not PName(var name)) return new PList(list.Select(Sub));
			if(name == "defm") return elem;
			if(name == "mfor") {
				var nlist = new PList(list.Select(Sub));
				var dlist = (PList) nlist[1];
				var (iname, start, end, step) = dlist switch {
					[PName(var vname), PInt(var fend)] =>
						(vname, 0, (int) fend, 1),
					[PName(var vname), PInt(var fstart), PInt(var fend)] =>
						(vname, (int) fstart, (int) fend, 1),
					[PName(var vname), PInt(var fstart), PInt(var fend), PInt(var fstep)] =>
						(vname, (int) fstart, (int) fend, (int) fstep),
					_ => throw new NotSupportedException(dlist.ToString()),
				};
				var ret = new List<PTree> { new PName("inline-block") };
				for(var i = start; i < end; i += step)
					ret.Add(Sub(Repl(name, nlist[2], new() { [iname] = new PInt(i) }, [])));
				return new PList(ret);
			}
			if(!macros.TryGetValue(name, out var mlist)) return new PList(list.Select(Sub));
			foreach(var (args, block) in mlist) {
				var variadic = args.Count > 0 && args[^1].StartsWith("...");
				if(variadic ? list.Count - 1 < args.Count - 1 : args.Count != list.Count - 1) continue;
				var repls = args.Take(variadic ? args.Count - 1 : args.Count)
					.Select((vn, i) => (vn, list[i + 1])).ToDictionary(x => x.vn, x => x.Item2);
				if(variadic)
					repls[args[^1]] = new PList(new PTree[] { new PName("inline-block") }.Concat(list.Skip(args.Count)));
				return Sub(Repl(name, block, repls, []));
			}

			throw new NotSupportedException($"No overload of macro {name.ToPrettyString()} takes {list.Count - 1} arguments");
		}

		PList StripInlineBlocks(PList list) {
			var nlist = new List<PTree>();
			foreach(var elem in list) {
				if(elem is not PList elist || elist.Count == 0) {
					nlist.Add(elem);
					continue;
				}
				if(elist[0] is PName("inline-block"))
					nlist.AddRange(StripInlineBlocks(new(elist.Skip(1))));
				else
					nlist.Add(StripInlineBlocks(elist));
			}
			return new(nlist);
		}

		return StripInlineBlocks((PList) Sub(top));
	}
}