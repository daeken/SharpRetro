using System.Numerics;
using CoreArchCompiler;
using LiftIl;
using XFusionCpu;
using XFusionGenerator;
using XFusionJit;
namespace XFusionTests;
[TestFixture]
public class MulOneTests {
    static ulong Mask(int w)=>w==64?ulong.MaxValue:(1UL<<w)-1;
    static X86Machine Machine(string hex,XMode mode)=>ImulOneTests.Machine(hex,mode);
    // BigInteger arithmetic is independent of the UInt128 lifter evaluator.
    static void Check(X86Machine m,int w,ulong a,ulong b,ulong oldAx,ulong oldDx) {
        var mask=Mask(w);var product=new BigInteger(a&mask)*new BigInteger(b&mask);
        var lo=(ulong)(product&mask);var hi=(ulong)((product>>w)&mask);
        var ax=w==8?(oldAx&~0xffffUL)|(ulong)product:w==16?(oldAx&~mask)|lo:lo;
        var dx=w==8?oldDx:w==16?(oldDx&~mask)|hi:hi;
        Assert.That(m.Gpr[0],Is.EqualTo(ax),$"AX width{w} {a:x}*{b:x}");
        Assert.That(m.Gpr[2],Is.EqualTo(dx),$"DX width{w} {a:x}*{b:x}");
        Assert.That(m.Flags&0x801,Is.EqualTo(hi!=0?0x801UL:0UL));
        Assert.That(m.Flags&0x602,Is.EqualTo(0x602));
        // Undefined PF/AF/ZF/SF checked structurally, never asserted zero.
    }
    [TestCase("f6e1",XMode.Bits32,8)]
    [TestCase("66f6e1",XMode.Bits32,8)]
    [TestCase("f7e1",XMode.Bits16,16)]
    [TestCase("66f7e1",XMode.Bits16,32)]
    [TestCase("66f7e1",XMode.Bits32,16)]
    [TestCase("f7e1",XMode.Bits32,32)]
    [TestCase("f7e1",XMode.Bits64,32)]
    [TestCase("66f7e1",XMode.Bits64,16)]
    [TestCase("48f7e1",XMode.Bits64,64)]
    [TestCase("6648f7e1",XMode.Bits64,64)]
    public void WidthsAndBoundaries(string hex,XMode mode,int w) {
        var mask=Mask(w);var values=new[]{0UL,1UL,2UL,mask,mask-1,1UL<<(w-1),(1UL<<(w-1))-1,(1UL<<(w/2))+1};
        foreach(var a in values)foreach(var b in values) {
            var m=Machine(hex,mode);var ax=(0xabcddcba87654321UL&~mask)|a;const ulong dx=0x76543210fedcba98;
            m.Gpr[0]=ax;m.Gpr[1]=b;m.Gpr[2]=dx;m.Flags=0xed7;
            Assert.That(m.Step(),Is.True);Check(m,w,a,b,ax,dx);
        }
    }
    [Test]
    public void ExhaustiveByteProducts() {
        for(ulong a=0;a<256;a++)for(ulong b=0;b<256;b++) {
            var m=Machine("f6e1",XMode.Bits32);m.Gpr[0]=0xaabbccdd00000000|a;m.Gpr[1]=b;m.Gpr[2]=77;m.Flags=0x602;
            Assert.That(m.Step(),Is.True);Check(m,8,a,b,0xaabbccdd00000000|a,77);
        }
    }
    [TestCase("f6e0",XMode.Bits32,8,0)]
    [TestCase("f6e4",XMode.Bits32,8,0)]
    [TestCase("40f6e4",XMode.Bits64,8,4)]
    [TestCase("f7e0",XMode.Bits32,32,0)]
    [TestCase("f7e2",XMode.Bits32,32,2)]
    [TestCase("66f7e2",XMode.Bits32,16,2)]
    [TestCase("48f7e0",XMode.Bits64,64,0)]
    [TestCase("48f7e2",XMode.Bits64,64,2)]
    public void RegisterAliases(string hex,XMode mode,int w,int src) {
        var m=Machine(hex,mode);m.Gpr[0]=0xaabbccdd1234fe83;m.Gpr[2]=0xfedcba9876543210;m.Gpr[4]=0x29;m.Flags=0x602;
        var ax=m.Gpr[0];var dx=m.Gpr[2];var b=hex=="f6e4"?ax>>8:m.Gpr[src];
        Assert.That(m.Step(),Is.True);Check(m,w,ax,b,ax,dx);
    }
    [TestCase("f620",XMode.Bits32,8,0)]
    [TestCase("66f720",XMode.Bits32,16,0)]
    [TestCase("f720",XMode.Bits32,32,0)]
    [TestCase("f722",XMode.Bits32,32,2)]
    [TestCase("48f720",XMode.Bits64,64,0)]
    [TestCase("48f722",XMode.Bits64,64,2)]
    public void MemoryReadAndFaultOrdering(string hex,XMode mode,int w,int addressReg) {
        foreach(var fault in new[]{false,true}) {
            var m=Machine(hex,mode);m.Gpr[0]=0x9000;m.Gpr[2]=0xa000;m.Flags=0xed7;
            var ax=m.Gpr[0];var dx=m.Gpr[2];var flags=m.Flags;var address=m.Gpr[addressReg];var reads=0;
            m.LoadHook=(a,width)=>{reads++;Assert.That(a,Is.EqualTo(address));Assert.That(width,Is.EqualTo(w));Assert.That(m.Gpr[0],Is.EqualTo(ax));Assert.That(m.Gpr[2],Is.EqualTo(dx));Assert.That(m.Flags,Is.EqualTo(flags));if(fault)throw new InvalidOperationException("sentinel fault");return Mask(w);};
            m.StoreHook=(_,_,_)=>throw new AssertionException("MUL must not store memory");
            if(fault) {Assert.Throws<InvalidOperationException>(()=>m.Step());Assert.That(m.Gpr[0],Is.EqualTo(ax));Assert.That(m.Gpr[2],Is.EqualTo(dx));Assert.That(m.Flags,Is.EqualTo(flags));Assert.That(m.Ip,Is.EqualTo(0x1000));}
            else {Assert.That(m.Step(),Is.True);Check(m,w,ax,Mask(w),ax,dx);}
            Assert.That(reads,Is.EqualTo(1));
        }
    }
    [TestCase("f7e6",XMode.Bits32)]
    [TestCase("f6e1",XMode.Bits32)]
    [TestCase("66f7e1",XMode.Bits32)]
    [TestCase("48f7e1",XMode.Bits64)]
    public void TypedUnsignedAndUndefinedFlags(string hex,XMode mode) {
        var block=X86Lifter.Lift(Convert.FromHexString(hex),0x1000,mode);
        Assert.That(block.Body.OfType<IlIntrin>(),Is.Empty);
        var products=block.Body.OfType<IlLet>().Select(l=>l.Val).OfType<IlBin>().Where(b=>b.Op==BinOp.Mul).ToArray();
        Assert.That(products,Has.Length.EqualTo(1));Assert.That(products[0].Ty,Is.TypeOf<IlType.I>());
        Assert.That(((IlType.I)products[0].Ty).Signed,Is.False);
        Assert.That(((IlCast)products[0].L).Kind,Is.EqualTo(CastKind.Zext));Assert.That(((IlCast)products[0].R).Kind,Is.EqualTo(CastKind.Zext));
        Assert.That(block.Body.OfType<IlWriteReg>().Where(w=>w.Val is IlUndefined).Select(w=>w.Idx),Is.EquivalentTo(new[]{2,4,6,7}));
        Assert.That(block.Body.OfType<IlWriteReg>().Count(w=>w.Kind==RegKind.Eflags),Is.EqualTo(6));
    }
    [Test]
    public void WrongSignednessIsDistinguishable() {
        var u=Machine("f6e1",XMode.Bits32);var s=Machine("f6e9",XMode.Bits32);
        foreach(var m in new[]{u,s}) {m.Gpr[0]=255;m.Gpr[1]=255;Assert.That(m.Step(),Is.True);}
        Assert.That(u.Gpr[0],Is.EqualTo(65025));Assert.That(s.Gpr[0],Is.EqualTo(1));Assert.That(u.Flags&0x801,Is.EqualTo(0x801));Assert.That(s.Flags&0x801,Is.Zero);
    }
    [Test]
    public void InvalidSignAndWidthRefused() {
        var binds=new Dictionary<string,OperandBind>{["src"]=new OperandBind.Reg(1,24)};
        Assert.Throws<NotSupportedException>(()=>IlLower.Lower(["src"],ListParser.Parse("(mul-wide src #f)"),binds,24));
        binds["src"]=new OperandBind.Reg(1,32);
        Assert.Throws<NotSupportedException>(()=>IlLower.Lower(["src"],ListParser.Parse("(mul-wide src unknown)"),binds,32));
    }
    [Test]
    public void UlongJitRefuses128BitProduct() {
        Assert.Throws<NotSupportedException>(()=>TestRecompiler.Compile(Convert.FromHexString("48f7e1"),0x1000,XMode.Bits64,new byte[0x20000]));
    }
}
