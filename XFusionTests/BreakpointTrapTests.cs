using LiftIl;
using XFusionCpu;
using XFusionJit;
namespace XFusionTests;
[TestFixture]
public class BreakpointTrapTests {
    sealed class TrapBoundary : Exception;
    [TestCase(XMode.Bits16, 0x1234UL, 0x1235UL)]
    [TestCase(XMode.Bits16, 0xffffUL, 0UL)]
    [TestCase(XMode.Bits32, 0x4013e0UL, 0x4013e1UL)]
    [TestCase(XMode.Bits32, 0xffffffffUL, 0UL)]
    [TestCase(XMode.Bits64, 0x100001234UL, 0x100001235UL)]
    [TestCase(XMode.Bits64, ulong.MaxValue, 0UL)]
    public void TypedVectorOriginAndContinuation(XMode mode, ulong pc, ulong expected) {
        var block = X86Lifter.Lift(new byte[] { 0xcc }, pc, mode);
        Assert.That(block.Body.Single(), Is.TypeOf<IlTrap>());
        var trap = (IlTrap)block.Body.Single();
        Assert.That(trap.Architecture, Is.EqualTo("x86")); Assert.That(trap.Vector, Is.EqualTo(3));
        Assert.That(((IlConst)trap.InstructionPc).Bits, Is.EqualTo((UInt128)pc));
        Assert.That(((IlConst)trap.ContinuationPc).Bits, Is.EqualTo((UInt128)expected));
        Assert.That(block.ToString(), Does.Contain("trap x86 vector 3"));
        Assert.That(block.Body.OfType<IlFault>(), Is.Empty);
        var m = ImulOneTests.Machine("cc", mode); m.Ip = pc;
        m.OnTrap = (_, _, vector, origin, continuation) => {
            Assert.That(vector, Is.EqualTo(3)); Assert.That(origin, Is.EqualTo(pc));
            Assert.That(continuation, Is.EqualTo(expected)); throw new TrapBoundary();
        };
        Assert.Throws<TrapBoundary>(() => m.Step()); Assert.That(m.Ip, Is.EqualTo(pc));
    }
    [TestCase(XMode.Bits16)]
    [TestCase(XMode.Bits32)]
    [TestCase(XMode.Bits64)]
    public void TrapEventPreservesPreDeliveryStateAndCannotSilentlyContinue(XMode mode) {
        var m = ImulOneTests.Machine("cc40", mode); m.Flags = 0xed7;
        for(int i = 0; i < m.Gpr.Length; i++) m.Gpr[i] = (ulong)(i * 17 + 0x100);
        var gpr = m.Gpr.ToArray(); var flags = m.Flags; var calls = 0;
        m.LoadHook = (_, _) => throw new AssertionException("trap boundary must not load data");
        m.StoreHook = (_, _, _) => throw new AssertionException("trap boundary must not build a guest stack");
        m.OnIntrin = (_, _, _) => throw new AssertionException("typed trap must not fall back to intrinsics");
        m.OnTrap = (machine, architecture, vector, origin, continuation) => {
            calls++; Assert.That(machine, Is.SameAs(m)); Assert.That(architecture, Is.EqualTo("x86"));
            Assert.That(vector, Is.EqualTo(3)); Assert.That(origin, Is.EqualTo(0x1000));
            Assert.That(continuation, Is.EqualTo(0x1001)); Assert.That(m.Ip, Is.EqualTo(0x1000));
            Assert.That(m.Gpr, Is.EqualTo(gpr)); Assert.That(m.Flags, Is.EqualTo(flags));
            throw new TrapBoundary();
        };
        Assert.Throws<TrapBoundary>(() => m.Step()); Assert.That(calls, Is.EqualTo(1));
        Assert.That(m.Gpr, Is.EqualTo(gpr)); Assert.That(m.Flags, Is.EqualTo(flags));
        Assert.That(m.Ip, Is.EqualTo(0x1000)); Assert.That(m.Halted, Is.False);
    }
    [Test]
    public void CompletedPriorInstructionRetainedAndFollowingInstructionNotRun() {
        var bytes = Convert.FromHexString("b878563412ccbbefbeadde");
        var m = new X86Machine { Mode = XMode.Bits32, Ip = 0x1000, Flags = 0xed7 };
        m.FetchHook = (ulong address, Span<byte> into) => { bytes.AsSpan((int)(address - 0x1000)).CopyTo(into); return true; };
        m.Gpr[3] = 0x1111; m.Gpr[4] = 0x8000;
        Assert.That(m.Step(), Is.True); Assert.That(m.Gpr[0], Is.EqualTo(0x12345678));
        m.OnTrap = (_, _, _, origin, continuation) => {
            Assert.That(origin, Is.EqualTo(0x1005)); Assert.That(continuation, Is.EqualTo(0x1006));
            Assert.That(m.Gpr[0], Is.EqualTo(0x12345678)); throw new TrapBoundary();
        };
        Assert.Throws<TrapBoundary>(() => m.Step());
        Assert.That(m.Gpr[3], Is.EqualTo(0x1111)); Assert.That(m.Gpr[4], Is.EqualTo(0x8000));
        Assert.That(m.Flags, Is.EqualTo(0xed7)); Assert.That(m.Ip, Is.EqualTo(0x1005));
    }
    [Test]
    public void CompletedPushMemoryAndStackAreRetainedAtTrapBoundary() {
        var m = new X86Machine { Mode = XMode.Bits32, Ip = 0x1000, Mem = new byte[0x10000], Flags = 0xed7 };
        m.Mem[0x1000] = 0x50; m.Mem[0x1001] = 0xcc;
        m.Gpr[0] = 0xdeadbeef; m.Gpr[4] = 0x8000;
        Assert.That(m.Step(), Is.True); Assert.That(m.Gpr[4], Is.EqualTo(0x7ffc));
        Assert.That(m.Load(0x7ffc, 32), Is.EqualTo(0xdeadbeef));
        var memory = m.Mem.ToArray();
        m.OnTrap = (_, _, _, origin, continuation) => {
            Assert.That(origin, Is.EqualTo(0x1001)); Assert.That(continuation, Is.EqualTo(0x1002));
            throw new TrapBoundary();
        };
        Assert.Throws<TrapBoundary>(() => m.Step());
        Assert.That(m.Mem, Is.EqualTo(memory)); Assert.That(m.Gpr[4], Is.EqualTo(0x7ffc));
        Assert.That(m.Gpr[0], Is.EqualTo(0xdeadbeef)); Assert.That(m.Flags, Is.EqualTo(0xed7));
    }
    [Test]
    public void MissingOrReturnedDeliveryIsExplicitlyUnsupported() {
        var m = ImulOneTests.Machine("cc", XMode.Bits32); m.Gpr[0] = 77; m.Flags = 0xed7;
        Assert.Throws<NotSupportedException>(() => m.Step());
        Assert.That(m.Ip, Is.EqualTo(0x1000)); Assert.That(m.Gpr[0], Is.EqualTo(77));
        var calls = 0; m.OnTrap = (_, _, _, _, _) => { calls++; };
        var e = Assert.Throws<NotSupportedException>(() => m.Step());
        Assert.That(e!.Message, Does.Contain("hook returned")); Assert.That(calls, Is.EqualTo(1));
        Assert.That(m.Ip, Is.EqualTo(0x1000)); Assert.That(m.Flags, Is.EqualTo(0xed7));
    }
    [Test]
    public void DivideFaultRemainsRestartableAndDoesNotBecomeATrap() {
        var block = X86Lifter.Lift(Convert.FromHexString("f7f1"), 0x1000, XMode.Bits32);
        Assert.That(block.Body.OfType<IlTrap>(), Is.Empty);
        var m = ImulOneTests.Machine("f7f1", XMode.Bits32); m.Gpr[1] = 0;
        m.OnTrap = (_, _, _, _, _) => throw new AssertionException("#DE is not #BP");
        Assert.Throws<DivideByZeroException>(() => m.Step()); Assert.That(m.Ip, Is.EqualTo(0x1000));
    }
    [TestCase("66cc")]
    [TestCase("67cc")]
    [TestCase("f2cc")]
    [TestCase("f3cc")]
    [TestCase("64cc")]
    [TestCase("40cc")]
    public void UnmodeledPrefixesCannotBecomePlainTrap(string hex) {
        Assert.Throws<NotSupportedException>(() => X86Lifter.Lift(Convert.FromHexString(hex), 0x1000, XMode.Bits64));
    }
    [Test]
    public void UnsupportedEncodingsAndModesRemainExplicit() {
        Assert.That(ImulOneTests.Machine("f0cc", XMode.Bits32).Step(), Is.False);
        var interrupt = X86Lifter.Lift(Convert.FromHexString("cd03"), 0x1000, XMode.Bits32);
        Assert.That(interrupt.Body.OfType<IlTrap>(), Is.Empty);
        Assert.That(interrupt.Body.Single(), Is.TypeOf<IlIntrin>());
        Assert.That(Disassembler.DecodeInsn(new byte[] { 0xcc }, XMode.Bits32, out var decoded), Is.True);
        Assert.Throws<NotSupportedException>(() => X86Lifter.Lift(in decoded, 0x1000, (XMode)999));
        decoded.P.VexValid = true;
        Assert.Throws<NotSupportedException>(() => X86Lifter.Lift(in decoded, 0x1000, XMode.Bits32));
    }
    [TestCase(XMode.Bits16)]
    [TestCase(XMode.Bits32)]
    [TestCase(XMode.Bits64)]
    public void JitDeliveryRemainsExplicitlyUnsupported(XMode mode) {
        Assert.Throws<NotSupportedException>(() => TestRecompiler.Compile(new byte[] { 0xcc }, 0x1000, mode, new byte[0x20000]));
    }
}
