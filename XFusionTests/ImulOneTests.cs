using LiftIl;
using XFusionCpu;
using XFusionJit;
namespace XFusionTests;
[TestFixture]
public class ImulOneTests {
    public static X86Machine Machine(string hex, XMode mode) {
        var bytes=Convert.FromHexString(hex);
        var m=new X86Machine {Mode=mode,Ip=0x1000};
        m.FetchHook=(ulong _,Span<byte> d)=>{bytes.CopyTo(d);return true;};
        return m;
    }
    [TestCase("f7e9",XMode.Bits32)]
    [TestCase("f6ec",XMode.Bits32)]
    [TestCase("66f7e9",XMode.Bits32)]
    [TestCase("48f7e9",XMode.Bits64)]
    public void TypedEffectsRatherThanOpaqueMultiply(string hex,XMode mode) {
        var block=X86Lifter.Lift(Convert.FromHexString(hex),0x1000,mode);
        Assert.That(block.Body.OfType<IlIntrin>(),Is.Empty);
        Assert.That(block.ToString(),Does.Contain("mul"));
        Assert.That(block.Body.OfType<IlWriteReg>().Count(x=>x.Kind==RegKind.Eflags),Is.EqualTo(6));
    }
    [TestCase("f7e9",XMode.Bits32,32)]
    [TestCase("66f7e9",XMode.Bits32,16)]
    [TestCase("f7e9",XMode.Bits16,16)]
    [TestCase("66f7e9",XMode.Bits16,32)]
    [TestCase("f7e9",XMode.Bits64,32)]
    [TestCase("66f7e9",XMode.Bits64,16)]
    [TestCase("48f7e9",XMode.Bits64,64)]
    [TestCase("6648f7e9",XMode.Bits64,64)]
    public void BoundaryProducts(string hex,XMode mode,int width) {
        ulong mask=width==64?ulong.MaxValue:(1UL<<width)-1;
        foreach(var a in new[]{0UL,1UL,mask,1UL<<(width-1),(1UL<<(width-1))-1})
        foreach(var b in new[]{0UL,1UL,2UL,mask,1UL<<(width-1)}) {
            var m=Machine(hex,mode);m.Gpr[0]=a;m.Gpr[1]=b;m.Gpr[2]=0xabcddcba00000000;m.Flags=0x602;
            Assert.That(m.Step(),Is.True);
            Int128 Signed(ulong x)=>(Int128)x<<(128-width)>>(128-width);
            var product=Signed(a)*Signed(b);var bits=(UInt128)product;
            var low=(ulong)bits&mask;var high=(ulong)(bits>>width)&mask;
            Assert.That(m.Gpr[0]&mask,Is.EqualTo(low));Assert.That(m.Gpr[2]&mask,Is.EqualTo(high));
            if(width==16)Assert.That(m.Gpr[2]>>16,Is.EqualTo(0xabcddcba00000000UL>>16));
            if(width==32)Assert.That(m.Gpr[2]>>32,Is.Zero);
            var ov=product!=Signed(low);
            Assert.That(m.Flags&0x801,Is.EqualTo(ov?0x801UL:0UL));Assert.That(m.Flags&0x602,Is.EqualTo(0x602UL));
        }
    }
    [TestCase("f6ec",0x123456789abcfe03UL,0x123456789abcfffaUL)] // AL3 * AH-2
    [TestCase("f6e8",0x123456789abc0080UL,0x123456789abc4000UL)] // AL-128 squared
    public void ByteProductWritesOnlyAxAndCapturesAliasedInput(string hex,ulong before,ulong after) {
        var m=Machine(hex,XMode.Bits32);m.Gpr[0]=before;m.Gpr[2]=0x1234;
        Assert.That(m.Step(),Is.True);Assert.That(m.Gpr[0],Is.EqualTo(after));Assert.That(m.Gpr[2],Is.EqualTo(0x1234));
    }
    [Test]
    public void SourceReadBeforeImplicitWrites() {
        var m=Machine("f728",XMode.Bits32);m.Gpr[0]=0x9000;m.Gpr[2]=77;int reads=0;
        m.LoadHook=(a,w)=>{reads++;Assert.That(a,Is.EqualTo(0x9000));Assert.That(w,Is.EqualTo(32));Assert.That(m.Gpr[0],Is.EqualTo(0x9000));Assert.That(m.Gpr[2],Is.EqualTo(77));return 0xfffffffe;};
        Assert.That(m.Step(),Is.True);Assert.That(reads,Is.EqualTo(1));Assert.That(m.Gpr[0],Is.EqualTo(0xfffee000UL));Assert.That(m.Gpr[2],Is.EqualTo(0xffffffffUL));
    }
    [Test]
    public void LockPrefixRejected() { Assert.That(Machine("f0f7e9",XMode.Bits32).Step(),Is.False); }

    [Test]
    public void UndefinedFlagsAreValuesNotPreservedInputs() {
        var block=X86Lifter.Lift(Convert.FromHexString("f7e9"),0x1000,XMode.Bits32);
        Assert.That(block.Body.OfType<IlWriteReg>().Where(x=>x.Val is IlUndefined).Select(x=>x.Idx),
            Is.EquivalentTo(new[]{2,4,6,7}));
    }
    [TestCase("f2f7e9")]
    [TestCase("f3f7e9")]
    public void UnsupportedRepPrefixFailsExplicitly(string hex) {
        Assert.Throws<NotSupportedException>(()=>X86Lifter.Lift(Convert.FromHexString(hex),0x1000,XMode.Bits32));
    }
    [Test]
    public void ScalarUlongJitRejects128BitProduct() {
        Assert.Throws<NotSupportedException>(()=>TestRecompiler.Compile(Convert.FromHexString("48f7e9"),0x1000,XMode.Bits64,new byte[0x20000]));
    }
}
