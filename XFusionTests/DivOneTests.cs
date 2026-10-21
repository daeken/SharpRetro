using System.Numerics;
using CoreArchCompiler;
using LiftIl;
using XFusionCpu;
using XFusionGenerator;
using XFusionJit;
namespace XFusionTests;
[TestFixture]
public class DivOneTests {
    static ulong Mask(int w) => w == 64 ? ulong.MaxValue : (1UL << w) - 1;
    static X86Machine Machine(string hex, XMode mode) => ImulOneTests.Machine(hex, mode);
    // Independent BigInteger reference: no typed IL or UInt128 calculation.
    static void Check(X86Machine m, int w, ulong divisor) {
        var before = m.Gpr.ToArray(); var flags = m.Flags; var ip = m.Ip;
        var mask = Mask(w);
        var dividend = w == 8 ? new BigInteger(before[0] & 0xffff) :
            (new BigInteger(before[2] & mask) << w) | new BigInteger(before[0] & mask);
        var d = new BigInteger(divisor & mask);
        var fault = d.IsZero || dividend / d > mask;
        if(fault) {
            Assert.Throws<DivideByZeroException>(() => m.Step());
            Assert.That(m.Gpr, Is.EqualTo(before)); Assert.That(m.Flags, Is.EqualTo(flags));
            Assert.That(m.Ip, Is.EqualTo(ip)); return;
        }
        var q = (ulong)(dividend / d); var r = (ulong)(dividend % d);
        Assert.That(m.Step(), Is.True);
        before[0] = w == 8 ? (before[0] & ~0xffffUL) | q | (r << 8) :
            w == 16 ? (before[0] & ~mask) | q : q;
        if(w != 8) before[2] = w == 16 ? (before[2] & ~mask) | r : r;
        Assert.That(m.Gpr, Is.EqualTo(before));
        Assert.That(m.Flags & ~0x8d5UL, Is.EqualTo(flags & ~0x8d5UL));
        // All six arithmetic flags are undefined, checked structurally below.
    }
    [TestCase("f6f1", XMode.Bits16, 8)]
    [TestCase("f6f1", XMode.Bits32, 8)]
    [TestCase("66f6f1", XMode.Bits32, 8)]
    [TestCase("f7f1", XMode.Bits16, 16)]
    [TestCase("66f7f1", XMode.Bits16, 32)]
    [TestCase("66f7f1", XMode.Bits32, 16)]
    [TestCase("f7f1", XMode.Bits32, 32)]
    [TestCase("f7f1", XMode.Bits64, 32)]
    [TestCase("66f7f1", XMode.Bits64, 16)]
    [TestCase("48f7f1", XMode.Bits64, 64)]
    [TestCase("6648f7f1", XMode.Bits64, 64)]
    public void BoundaryArithmeticAndArchitecturalFaults(string hex, XMode mode, int w) {
        var mask = Mask(w);
        var values = new[] { 0UL, 1UL, 2UL, mask - 1, mask, 1UL << (w - 1) };
        foreach(var high in values) foreach(var low in values) foreach(var divisor in values) {
            var m = Machine(hex, mode); m.Flags = 0x2ed7;
            m.Gpr[0] = w == 8 ? 0xaabbccddeeff0000UL | high << 8 | low :
                (0xaabbccdd87654321UL & ~mask) | low;
            m.Gpr[2] = (0xabcdef0198765432UL & ~mask) | high;
            m.Gpr[1] = divisor; Check(m, w, divisor);
        }
    }
    [Test]
    public void ByteDividendSweepAndMaximumQuotient() {
        // All 65536 AX dividends, representative divisors incl. zero and max.
        foreach(ulong d in new[] { 0UL, 1UL, 2UL, 127UL, 128UL, 255UL })
        for(ulong ax = 0; ax <= 0xffff; ax++) {
            var m = Machine("f6f1", XMode.Bits32); m.Gpr[0] = 0xabcd000000000000UL | ax;
            m.Gpr[1] = d; m.Gpr[2] = 0xfeed; m.Flags = 0xed7; Check(m, 8, d);
        }
    }
    [TestCase("f6f0", XMode.Bits32, 8, 0)]
    [TestCase("f6f4", XMode.Bits32, 8, 0)]
    [TestCase("40f6f4", XMode.Bits64, 8, 4)]
    [TestCase("f7f0", XMode.Bits32, 32, 0)]
    [TestCase("f7f2", XMode.Bits32, 32, 2)]
    [TestCase("66f7f0", XMode.Bits32, 16, 0)]
    [TestCase("66f7f2", XMode.Bits32, 16, 2)]
    [TestCase("48f7f0", XMode.Bits64, 64, 0)]
    [TestCase("48f7f2", XMode.Bits64, 64, 2)]
    public void OverlappingSourceRegisters(string hex, XMode mode, int w, int index) {
        foreach(var ax in new[] { 0x83UL, 0x1234UL, 0x80000000000000ffUL })
        foreach(var dx in new[] { 0UL, 1UL, 0x8000000000000000UL }) {
            var m = Machine(hex, mode); m.Gpr[0] = ax; m.Gpr[2] = dx;
            m.Gpr[4] = 0x29; m.Flags = 0xed7;
            Check(m, w, hex == "f6f4" ? ax >> 8 : m.Gpr[index]);
        }
    }
    [TestCase("f630", XMode.Bits32, 8, 0)]
    [TestCase("66f730", XMode.Bits32, 16, 0)]
    [TestCase("f730", XMode.Bits32, 32, 0)]
    [TestCase("f732", XMode.Bits32, 32, 2)]
    [TestCase("48f730", XMode.Bits64, 64, 0)]
    [TestCase("48f732", XMode.Bits64, 64, 2)]
    public void SourceMemorySingleReadAndFaultPrecedence(string hex, XMode mode, int w, int index) {
        foreach(var loadFault in new[] { false, true })
        foreach(var divisor in new[] { 0UL, 1UL, Mask(w) }) {
            var m = Machine(hex, mode); m.Gpr[0] = 0x9000; m.Gpr[2] = 0xa000;
            m.Flags = 0xed7; var before = m.Gpr.ToArray(); var flags = m.Flags;
            var address = m.Gpr[index]; var reads = 0;
            m.LoadHook = (a, bits) => {
                reads++; Assert.That(a, Is.EqualTo(address)); Assert.That(bits, Is.EqualTo(w));
                Assert.That(m.Gpr, Is.EqualTo(before)); Assert.That(m.Flags, Is.EqualTo(flags));
                if(loadFault) throw new InvalidOperationException("source memory fault sentinel");
                return divisor;
            };
            m.StoreHook = (_, _, _) => throw new AssertionException("DIV must not store memory");
            if(loadFault) {
                Assert.Throws<InvalidOperationException>(() => m.Step());
                Assert.That(m.Gpr, Is.EqualTo(before)); Assert.That(m.Flags, Is.EqualTo(flags));
                Assert.That(m.Ip, Is.EqualTo(0x1000));
            } else Check(m, w, divisor);
            Assert.That(reads, Is.EqualTo(1));
        }
    }
    [TestCase("f6f1", XMode.Bits32)]
    [TestCase("66f7f1", XMode.Bits32)]
    [TestCase("f7f1", XMode.Bits32)]
    [TestCase("48f7f1", XMode.Bits64)]
    public void TypedFaultsAndUndefinedFlags(string hex, XMode mode) {
        var b = X86Lifter.Lift(Convert.FromHexString(hex), 0x1000, mode);
        Assert.That(b.Body.OfType<IlIntrin>(), Is.Empty);
        var checks = b.Body.OfType<IlIf>().ToArray(); Assert.That(checks, Has.Length.EqualTo(2));
        foreach(var c in checks) {
            Assert.That(c.Then, Is.EqualTo(new Il[] { new IlFault("x86", 0) }));
            Assert.That(c.Else, Is.Empty);
        }
        Assert.That(b.Body.OfType<IlWriteReg>().Where(x => x.Val is IlUndefined).Select(x => x.Idx),
            Is.EquivalentTo(new[] { 0, 2, 4, 6, 7, 11 }));
        Assert.That(b.ToString(), Does.Contain("fault x86 vector 0"));
        var firstWrite = b.Body.ToList().FindIndex(x => x is IlWriteReg);
        Assert.That(b.Body.ToList().FindLastIndex(x => x is IlIf), Is.LessThan(firstWrite));
    }
    [Test]
    public void SignedDivisionIsTypedAndDistinct() {
        // IDIV lowers to typed effects (SDiv/SRem plus the #DE checks), no longer the
        // opaque idiv-wide marker; it must still differ from DIV on the same bits.
        var b = X86Lifter.Lift(Convert.FromHexString("f7f9"), 0x1000, XMode.Bits32);
        Assert.That(b.Body.OfType<IlIntrin>(), Is.Empty);
        Assert.That(b.ToString(), Does.Contain("sdiv").And.Contain("srem"));
        var u = Machine("f6f1", XMode.Bits32); var s = Machine("f6f9", XMode.Bits32);
        foreach(var m in new[] { u, s }) { m.Gpr[0] = 0xff80; m.Gpr[1] = 2; }
        Assert.Throws<DivideByZeroException>(() => u.Step());
        Assert.That(s.Step(), Is.True); Assert.That(s.Gpr[0] & 0xffff, Is.EqualTo(0xc0));
    }
    [Test]
    public void UnsupportedWidthSignednessAndPrefixesAreRefused() {
        var binds = new Dictionary<string, OperandBind> { ["src"] = new OperandBind.Reg(1, 24) };
        Assert.Throws<NotSupportedException>(() => IlLower.Lower(["src"], ListParser.Parse("(div-wide src #f)"), binds, 24));
        binds["src"] = new OperandBind.Reg(1, 32);
        Assert.Throws<NotSupportedException>(() => IlLower.Lower(["src"], ListParser.Parse("(div-wide src unknown)"), binds, 32));
        foreach(var hex in new[] { "f2f7f1", "f3f7f1" })
            Assert.Throws<NotSupportedException>(() => X86Lifter.Lift(Convert.FromHexString(hex), 0x1000, XMode.Bits32));
        Assert.That(Machine("f0f7f1", XMode.Bits32).Step(), Is.False);
        Assert.That(Disassembler.DecodeInsn(Convert.FromHexString("f7f1"), XMode.Bits32, out var decoded), Is.True);
        Assert.Throws<NotSupportedException>(() => X86Lifter.Lift(in decoded, 0x1000, (XMode)999));
        decoded.P.VexValid = true;
        Assert.Throws<NotSupportedException>(() => X86Lifter.Lift(in decoded, 0x1000, XMode.Bits32));
        decoded.P.VexValid = false; decoded.P.EvexValid = true;
        Assert.Throws<NotSupportedException>(() => X86Lifter.Lift(in decoded, 0x1000, XMode.Bits32));
        decoded.P.EvexValid = false; decoded.P.Lock = true;
        Assert.Throws<NotSupportedException>(() => X86Lifter.Lift(in decoded, 0x1000, XMode.Bits32));
    }
    [Test]
    public void WideComparisonResultWidthDoesNotNarrowOperands() {
        var eval = typeof(X86Machine).GetMethod("Eval", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var values = new[] { BigInteger.Zero, BigInteger.One, BigInteger.One << 64,
            BigInteger.One << 127, (BigInteger.One << 128) - 1 };
        var wide = new IlType.I(false, 128); var m = new X86Machine();
        BigInteger Signed(BigInteger x) => x >= (BigInteger.One << 127) ? x - (BigInteger.One << 128) : x;
        foreach(var a in values) foreach(var b in values)
        foreach(var op in new[] { BinOp.Eq, BinOp.Ne, BinOp.Ult, BinOp.Ule, BinOp.Ugt, BinOp.Uge,
            BinOp.Slt, BinOp.Sle, BinOp.Sgt, BinOp.Sge }) {
            var expected = op switch {
                BinOp.Eq => a == b, BinOp.Ne => a != b,
                BinOp.Ult => a < b, BinOp.Ule => a <= b, BinOp.Ugt => a > b, BinOp.Uge => a >= b,
                BinOp.Slt => Signed(a) < Signed(b), BinOp.Sle => Signed(a) <= Signed(b),
                BinOp.Sgt => Signed(a) > Signed(b), BinOp.Sge => Signed(a) >= Signed(b),
                _ => throw new AssertionException("unknown comparison")
            };
            var expression = new IlBin(IlType.U1, op, new IlConst(wide, (UInt128)a), new IlConst(wide, (UInt128)b));
            Assert.That((UInt128)eval.Invoke(m, new object[] { expression })!, Is.EqualTo(expected ? UInt128.One : UInt128.Zero));
        }
    }
    [TestCase("f6f1", 8)]
    [TestCase("66f7f1", 16)]
    [TestCase("f7f1", 32)]
    public unsafe void TestJitHostPreservesStateOnDivideFault(string hex, int width) {
        foreach(var zero in new[] { false, true }) {
            var (rc, fn) = TestRecompiler.Compile(Convert.FromHexString(hex), 0x1000, XMode.Bits32, new byte[0x20000]);
            var st = default(X86State); st.Rip = 0x1000; st.Flags = 0xed7;
            st.Gpr[0] = 0xffff; st.Gpr[2] = 0xffff; st.Gpr[1] = zero ? 0UL : 1UL;
            rc.BindState(&st);
            // Fixed local pointer cannot be captured by an assertion lambda.
            var caught = false;
            try { fn(&st); } catch(DivideByZeroException) { caught = true; }
            Assert.That(caught, Is.True);
            Assert.That(st.Gpr[0], Is.EqualTo(0xffff)); Assert.That(st.Gpr[2], Is.EqualTo(0xffff));
            Assert.That(st.Gpr[1], Is.EqualTo(zero ? 0UL : 1UL));
            Assert.That(st.Flags, Is.EqualTo(0xed7)); Assert.That(st.Rip, Is.EqualTo(0x1000));
        }
    }
    [Test]
    public void DefaultJitHostRefusesMissingFaultDelivery() {
        var jit = new CilJit.CilJit<ulong>(); var host = new X86Recompiler();
        Assert.Throws<NotSupportedException>(() => jit.CreateFunction<TestRecompiler.Compiled>("missing_fault", builder => {
            host.Begin(builder, builder.StructRefArgument<X86State>(0));
            host.RecompileOne(Convert.FromHexString("f7f1"), 0x1000, XMode.Bits32);
        }));
    }
    [TestCase("48f7f1", XMode.Bits64)]
    public void JitWithout128BitCarrierMustRefuse(string hex, XMode mode) {
        Assert.Throws<NotSupportedException>(() => TestRecompiler.Compile(Convert.FromHexString(hex), 0x1000, mode, new byte[0x20000]));
    }
}
