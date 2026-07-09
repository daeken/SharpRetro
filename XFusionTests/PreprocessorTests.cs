using CoreArchCompiler;

namespace XFusionTests;

/// XF-0 compiler-layer tests (layer 1 of 3 per #pagentry-dev ·26 plan).
/// Feed spec strings, assert on the preprocessed/rewritten tree — no generator run needed.
public class PreprocessorTests {
	static PList Pre(string src, params string[] features) {
		var pp = new Preprocessor(features);
		var tree = pp.Process(ListParser.Parse(src), ".");
		pp.ValidateEnabled();
		return tree;
	}

	static string Flat(PTree t) => t switch {
		PList l => "(" + string.Join(" ", l.Select(Flat)) + ")",
		_ => t.ToString() ?? ""
	};

	[Test]
	public void DefineFeatureIsRemoved() {
		var tree = Pre("(define-feature ia32) (foo)");
		Assert.That(Flat(tree), Is.EqualTo("((foo))"));
	}

	[Test]
	public void FeaturingDropsWhenDisabled() {
		var tree = Pre("(define-feature avx) (featuring avx (vaddps) (vsubps)) (after)");
		Assert.That(Flat(tree), Is.EqualTo("((after))"));
	}

	[Test]
	public void FeaturingSplicesWhenEnabled() {
		var tree = Pre("(define-feature avx) (featuring avx (vaddps) (vsubps)) (after)", "avx");
		Assert.That(Flat(tree), Is.EqualTo("((vaddps) (vsubps) (after))"));
	}

	[Test]
	public void FeaturingNestedInsideForm() {
		var tree = Pre("(define-feature sse) (outer (featuring sse (inner)))", "sse");
		Assert.That(Flat(tree), Is.EqualTo("((outer (inner)))"));
	}

	[Test]
	public void FeaturingUndeclaredThrows() =>
		Assert.Throws<PreprocessorException>(() => Pre("(featuring typo (x))"));

	[Test]
	public void EnabledButNeverDeclaredThrows() =>
		Assert.Throws<PreprocessorException>(() => Pre("(define-feature real) (x)", "tyop"));

	[Test]
	public void HasFeatureRewritesToInt() {
		var tree = Pre("(define-feature mmx) (if (has-feature mmx) (a) (b))", "mmx");
		Assert.That(Flat(tree), Is.EqualTo("((if 1 (a) (b)))"));
		tree = Pre("(define-feature mmx) (if (has-feature mmx) (a) (b))");
		Assert.That(Flat(tree), Is.EqualTo("((if 0 (a) (b)))"));
	}

	[Test]
	public void IncludeSplicesFile() {
		var dir = Path.Combine(Path.GetTempPath(), "xf0-inc-" + Path.GetRandomFileName());
		Directory.CreateDirectory(dir);
		try {
			File.WriteAllText(Path.Combine(dir, "sub.isa"), "(from-sub 1)");
			File.WriteAllText(Path.Combine(dir, "root.isa"), "(before) (include \"sub.isa\") (after)");
			var tree = Preprocessor.ProcessFile(Path.Combine(dir, "root.isa"));
			Assert.That(Flat(tree), Is.EqualTo("((before) (from-sub 1) (after))"));
		} finally { Directory.Delete(dir, true); }
	}

	[Test]
	public void IncludeIsRelativeToIncludingFile() {
		var dir = Path.Combine(Path.GetTempPath(), "xf0-rel-" + Path.GetRandomFileName());
		Directory.CreateDirectory(Path.Combine(dir, "nested"));
		try {
			File.WriteAllText(Path.Combine(dir, "nested", "leaf.isa"), "(leaf)");
			File.WriteAllText(Path.Combine(dir, "nested", "mid.isa"), "(include \"leaf.isa\")");
			File.WriteAllText(Path.Combine(dir, "root.isa"), "(include \"nested/mid.isa\")");
			var tree = Preprocessor.ProcessFile(Path.Combine(dir, "root.isa"));
			Assert.That(Flat(tree), Is.EqualTo("((leaf))"));
		} finally { Directory.Delete(dir, true); }
	}

	[Test]
	public void IncludeInsideFeaturingOnlyLoadsWhenEnabled() {
		var dir = Path.Combine(Path.GetTempPath(), "xf0-feat-" + Path.GetRandomFileName());
		Directory.CreateDirectory(dir);
		try {
			// deliberately do NOT create gated.isa — proves the include never fires when disabled
			File.WriteAllText(Path.Combine(dir, "root.isa"),
				"(define-feature avx512) (featuring avx512 (include \"gated.isa\")) (tail)");
			var tree = Preprocessor.ProcessFile(Path.Combine(dir, "root.isa"));
			Assert.That(Flat(tree), Is.EqualTo("((tail))"));
		} finally { Directory.Delete(dir, true); }
	}

	[Test]
	public void IncludeCycleThrows() {
		var dir = Path.Combine(Path.GetTempPath(), "xf0-cyc-" + Path.GetRandomFileName());
		Directory.CreateDirectory(dir);
		try {
			File.WriteAllText(Path.Combine(dir, "a.isa"), "(include \"b.isa\")");
			File.WriteAllText(Path.Combine(dir, "b.isa"), "(include \"a.isa\")");
			Assert.Throws<PreprocessorException>(() => Preprocessor.ProcessFile(Path.Combine(dir, "a.isa")));
		} finally { Directory.Delete(dir, true); }
	}

	[Test]
	public void LineCommentsAreSkipped() {
		var tree = ListParser.Parse("; leading comment\n(a b) ; trailing\n; (commented-form)\n(c)");
		Assert.That(Flat(tree), Is.EqualTo("((a b) (c))"));
	}

	[Test]
	public void CommentTerminatesName() {
		var tree = ListParser.Parse("(ab;comment\ncd)");
		Assert.That(Flat(tree), Is.EqualTo("((ab cd))"));
	}
}

public class MacroTests {
	static string Flat(PTree t) => t switch {
		PList l => "(" + string.Join(" ", l.Select(Flat)) + ")",
		_ => t.ToString() ?? ""
	};

	static string Rewrite(string src) => Flat(MacroProcessor.Rewrite(ListParser.Parse(src)));

	[Test]
	public void FixedArityStillWorks() =>
		Assert.That(Rewrite("(defm twice (x) (+ x x)) (twice 3)"),
			Is.EqualTo("((defm twice (x) (+ x x)) (+ 3 3))"));

	[Test]
	public void VariadicCollectsRest() =>
		// ...body splices as inline-block, which StripInlineBlocks flattens into the parent
		Assert.That(Rewrite("(defm wrap (name ...body) (block name ...body)) (wrap W (a) (b) (c))"),
			Is.EqualTo("((defm wrap (name ...body) (block name ...body)) (block W (a) (b) (c)))"));

	[Test]
	public void VariadicEmptyRest() =>
		Assert.That(Rewrite("(defm wrap (name ...body) (block name ...body)) (wrap W)"),
			Is.EqualTo("((defm wrap (name ...body) (block name ...body)) (block W))"));

	[Test]
	public void MultiFormBody() =>
		// defm with >1 body form emits all forms (spliced at call site)
		Assert.That(Rewrite("(defm two () (a) (b)) (outer (two))"),
			Is.EqualTo("((defm two () (a) (b)) (outer (a) (b)))"));

	[Test]
	public void VariadicMustBeLast() =>
		Assert.Throws<NotSupportedException>(() => Rewrite("(defm bad (...rest x) (x)) (bad 1 2)"));
}
