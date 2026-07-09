using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoreArchCompiler;

/// <summary>
/// Pre-macro source pass. Runs after ListParser.Parse and before MacroProcessor.Rewrite,
/// so that included files can contribute their own defms/defs and feature-gated regions
/// are pruned before any macro or def processing sees them.
///
/// Forms handled (at any list depth; splicing forms splice into their parent list):
///   (define-feature name)        -- registers a known feature flag; removed from output
///   (featuring flag ...body)     -- if flag enabled: body forms spliced in place; else dropped.
///                                   flag must have been define-feature'd (typo protection).
///   (include "file.isa")         -- file parsed relative to the including file's directory,
///                                   recursively preprocessed, top-level forms spliced in place.
///   (has-feature flag)           -- rewritten to 1/0 (for use inside macros/exprs)
/// </summary>
public class Preprocessor {
	readonly HashSet<string> Known = [], Enabled = [];
	readonly HashSet<string> IncludeStack = [];  // absolute paths, cycle guard

	public IReadOnlySet<string> KnownFeatures => Known;
	public IReadOnlySet<string> EnabledFeatures => Enabled;

	public Preprocessor(IEnumerable<string> enabledFeatures = null) {
		if(enabledFeatures != null)
			Enabled.UnionWith(enabledFeatures);
	}

	public static PList ProcessFile(string path, IEnumerable<string> enabledFeatures = null) {
		var pp = new Preprocessor(enabledFeatures);
		return pp.Include(path);
	}

	public PList Process(PList tree, string baseDir) {
		var outList = new PList { Type = tree.Type };
		foreach(var child in tree)
			foreach(var form in ProcessForm(child, baseDir))
				outList.Add(form);
		return outList;
	}

	IEnumerable<PTree> ProcessForm(PTree form, string baseDir) {
		if(form is not PList list) {
			yield return form;
			yield break;
		}

		switch(list.Count == 0 ? null : list[0]) {
			case PName("define-feature"): {
				if(list.Count != 2 || list[1] is not PName(var fname))
					throw new PreprocessorException($"define-feature expects one name: {list}");
				Known.Add(fname);
				// Enabled set may contain features declared later in the file; validate at the end.
				yield break;
			}
			case PName("featuring"): {
				if(list.Count < 2 || list[1] is not PName(var flag))
					throw new PreprocessorException($"featuring expects a flag name: {list}");
				if(!Known.Contains(flag))
					throw new PreprocessorException($"featuring references undeclared feature '{flag}' (declare with define-feature first)");
				if(!Enabled.Contains(flag))
					yield break;
				foreach(var body in list.Skip(2))
					foreach(var sub in ProcessForm(body, baseDir))
						yield return sub;
				yield break;
			}
			case PName("include"): {
				if(list.Count != 2 || list[1] is not PString ps)
					throw new PreprocessorException($"include expects one plain string literal: {list}");
				foreach(var sub in Include(Path.Combine(baseDir, ps.String)))
					yield return sub;
				yield break;
			}
			case PName("has-feature"): {
				if(list.Count != 2 || list[1] is not PName(var flag2))
					throw new PreprocessorException($"has-feature expects one name: {list}");
				if(!Known.Contains(flag2))
					throw new PreprocessorException($"has-feature references undeclared feature '{flag2}'");
				yield return new PInt(Enabled.Contains(flag2) ? 1 : 0);
				yield break;
			}
			default: {
				// Recurse: featuring/include/has-feature may appear nested (e.g. inside a def body).
				var outList = new PList { Type = list.Type };
				foreach(var child in list)
					foreach(var sub in ProcessForm(child, baseDir))
						outList.Add(sub);
				yield return outList;
				yield break;
			}
		}
	}

	public PList Include(string path) {
		var abs = Path.GetFullPath(path);
		if(!IncludeStack.Add(abs))
			throw new PreprocessorException($"include cycle detected at '{abs}'");
		try {
			if(!File.Exists(abs))
				throw new PreprocessorException($"include file not found: '{abs}'");
			var tree = ListParser.Parse(File.ReadAllText(abs));
			return Process(tree, Path.GetDirectoryName(abs) ?? ".");
		} finally {
			IncludeStack.Remove(abs);
		}
	}

	/// Call after processing the root file: any enabled feature that was never declared is a typo.
	public void ValidateEnabled() {
		var unknown = Enabled.Except(Known).ToList();
		if(unknown.Count != 0)
			throw new PreprocessorException($"enabled feature(s) never declared by any define-feature: {string.Join(", ", unknown)}");
	}
}

public class PreprocessorException(string message) : Exception(message);
