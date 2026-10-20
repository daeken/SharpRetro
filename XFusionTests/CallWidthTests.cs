using XFusionCpu;

namespace XFusionTests;

[TestFixture]
public class CallWidthTests {
    static X86Machine Machine(string hex, XMode mode, ulong pc=0x1000, ulong sp=0x8000) {
        var code=Convert.FromHexString(hex);
        var m=new X86Machine { Mode=mode, Ip=pc };
        m.Gpr[4]=sp;
        m.FetchHook=(ulong a, Span<byte> dst)=>{code.CopyTo(dst);return true;};
        return m;
    }

    [TestCase("e888d00b00",XMode.Bits32,0x401003UL,32,0x4be090UL)]
    [TestCase("e81000",XMode.Bits16,0x1000UL,16,0x1013UL)]
    [TestCase("e8f0ff",XMode.Bits16,0x1000UL,16,0xff3UL)]
    [TestCase("e8f0ffffff",XMode.Bits32,0x1000UL,32,0xff5UL)]
    [TestCase("e8f0ffffff",XMode.Bits64,0x100001000UL,64,0x100000ff5UL)]
    [TestCase("6648e810000000",XMode.Bits64,0x1000UL,64,0x1017UL)]
    [TestCase("66e810000100",XMode.Bits16,0x1000UL,32,0x11016UL)]
    [TestCase("66e810000000",XMode.Bits16,0x1000UL,32,0x1016UL)]
    [TestCase("66e81000",XMode.Bits32,0x12341000UL,16,0x1014UL)]
    [TestCase("e810000000",XMode.Bits64,0x100001000UL,64,0x100001015UL)]
    [TestCase("66e810000000",XMode.Bits64,0x100001000UL,64,0x100001016UL)]
    [TestCase("67e810000000",XMode.Bits32,0x1000UL,32,0x1016UL)]
    [TestCase("e810000000",XMode.Bits32,0xfffffffcUL,32,0x11UL)]
    public void DirectCallPushesOperandSizedReturn(string hex,XMode mode,ulong pc,int width,ulong target) {
        var m=Machine(hex,mode,pc);m.Flags=0xAD7;
        var stores=0;
        m.StoreHook=(a,v,w)=>{stores++;Assert.That(a,Is.EqualTo(0x8000UL-(ulong)(width/8)));Assert.That(w,Is.EqualTo(width));Assert.That(v,Is.EqualTo((pc+(ulong)(hex.Length/2)) & (width==64?ulong.MaxValue:(1UL<<width)-1)));return true;};
        Assert.That(m.Step(),Is.True);Assert.That(stores,Is.EqualTo(1));Assert.That(m.Ip,Is.EqualTo(target));Assert.That(m.Flags,Is.EqualTo(0xAD7UL));
    }

    [TestCase("ffd4",XMode.Bits32,32)]
    [TestCase("66ffd4",XMode.Bits32,16)]
    [TestCase("ffd4",XMode.Bits16,16)]
    [TestCase("66ffd4",XMode.Bits16,32)]
    [TestCase("ffd4",XMode.Bits64,64)]
    [TestCase("66ffd4",XMode.Bits64,64)]
    public void CallStackRegisterCapturesTargetBeforePush(string hex,XMode mode,int width) {
        var m=Machine(hex,mode);m.StoreHook=(a,v,w)=>{Assert.That(w,Is.EqualTo(width));return true;};
        Assert.That(m.Step(),Is.True);Assert.That(m.Ip,Is.EqualTo(0x8000UL));Assert.That(m.Gpr[4],Is.EqualTo(0x8000UL-(ulong)(width/8)));
    }

    [TestCase("ff1424",XMode.Bits32,32)]
    [TestCase("66ff1424",XMode.Bits32,16)]
    [TestCase("ff1424",XMode.Bits64,64)]
    [TestCase("66ff1424",XMode.Bits64,64)]
    public void CallMemoryReadsTargetBeforeOverlappingPush(string hex,XMode mode,int width) {
        var m=Machine(hex,mode);var stored=false;var reads=0;
        m.LoadHook=(a,w)=>{reads++;Assert.That(stored,Is.False);Assert.That(a,Is.EqualTo(0x8000UL));Assert.That(w,Is.EqualTo(width));return 0x3456;};
        m.StoreHook=(a,v,w)=>{stored=true;return true;};
        Assert.That(m.Step(),Is.True);Assert.That(reads,Is.EqualTo(1));Assert.That(m.Ip,Is.EqualTo(0x3456UL));
    }

    [TestCase("e80000",XMode.Bits16,0x12340001UL,0x1234ffffUL,0xffffUL)]
    [TestCase("66e800000000",XMode.Bits16,0x12340001UL,0x1234fffdUL,0xfffdUL)]
    [TestCase("e800000000",XMode.Bits32,1UL,0xfffffffdUL,0xfffffffdUL)]
    [TestCase("66e80000",XMode.Bits32,1UL,0xffffffffUL,0xffffffffUL)]
    public void StackAddressWrapUsesModeNotOperandOrAddressPrefix(string hex,XMode mode,ulong sp,ulong final,ulong address) {
        var m=Machine(hex,mode,sp:sp);m.StoreHook=(a,v,w)=>{Assert.That(a,Is.EqualTo(address));return true;};
        Assert.That(m.Step(),Is.True);Assert.That(m.Gpr[4],Is.EqualTo(final));
    }

    [TestCase("f2e800000000")]
    public void UnsupportedLockAndBoundsPrefixesFail(string hex) {
        Assert.Throws<NotSupportedException>(()=>Machine(hex,XMode.Bits32).Step());
    }

    [Test]
    public void SegmentBaseIsAddedAfterStackOffsetWrap() {
        var m=Machine("e800000000",XMode.Bits32,sp:1);m.SegBase[2]=0x100000000;
        m.StoreHook=(a,v,w)=>{Assert.That(a,Is.EqualTo(0x1fffffffdUL));return true;};
        Assert.That(m.Step(),Is.True);
    }
    [Test]
    public void LockCallIsRejectedByDecoder() {
        Assert.That(Machine("f0e800000000",XMode.Bits32).Step(),Is.False);
    }

    [TestCase(XMode.Bits16,"e80d00","c3",0x12340001UL,0UL,0)]
    [TestCase(XMode.Bits16,"66e80a000000","66c3",0x12340001UL,0x10000UL,0)]
    [TestCase(XMode.Bits32,"e80b000000","c3",1UL,0x100000000UL,0)]
    [TestCase(XMode.Bits32,"66e80c00","66c3",1UL,0UL,0)]
    [TestCase(XMode.Bits64,"e80b000000","c3",0x8000UL,0UL,0)]
    [TestCase(XMode.Bits64,"66e80a000000","c3",0x8000UL,0UL,0)]
    [TestCase(XMode.Bits32,"e80b000000","c20600",0x8000UL,0x10000UL,6)]
    [TestCase(XMode.Bits32,"66e80c00","66c20600",0x8000UL,0x10000UL,6)]
    public void CallReturnRoundTrip(XMode mode,string call,string ret,ulong sp,ulong ss,int cleanup) {
        var m=Machine(call,mode,sp:sp);m.SegBase[2]=ss;
        var bytes=new Dictionary<ulong,byte>();var callBytes=Convert.FromHexString(call);var retBytes=Convert.FromHexString(ret);
        m.FetchHook=(ulong a,Span<byte> dst)=>{dst.Clear();(a==0x1000?callBytes:retBytes).CopyTo(dst);return true;};
        m.StoreHook=(a,v,w)=>{for(int n=0;n<w/8;n++)bytes[a+(ulong)n]=(byte)(v>>(8*n));return true;};
        m.LoadHook=(a,w)=>{ulong v=0;for(int n=0;n<w/8;n++)v|=(ulong)bytes[a+(ulong)n]<<(8*n);return v;};
        Assert.That(m.Step(),Is.True);Assert.That(m.Ip,Is.EqualTo(0x1010UL));
        Assert.That(m.Step(),Is.True);Assert.That(m.Ip,Is.EqualTo(0x1000UL+(ulong)callBytes.Length));
        Assert.That(m.Gpr[4],Is.EqualTo(sp+(ulong)cleanup));
    }

    [TestCase(XMode.Bits16,"50","58",0x12340001UL,16)]
    [TestCase(XMode.Bits16,"6650","6658",0x12340001UL,32)]
    [TestCase(XMode.Bits32,"50","58",1UL,32)]
    [TestCase(XMode.Bits32,"6650","6658",1UL,16)]
    [TestCase(XMode.Bits64,"50","58",0x8000UL,64)]
    [TestCase(XMode.Bits64,"6650","6658",0x8000UL,16)]
    public void PushPopUseSameStackContext(XMode mode,string push,string pop,ulong sp,int width) {
        var m=Machine(push,mode,sp:sp);m.Gpr[0]=0x1234;m.SegBase[2]=mode==XMode.Bits64?0UL:0x10000UL;
        ulong address=0,value=0;int storedWidth=0;
        m.StoreHook=(a,v,w)=>{address=a;value=v;storedWidth=w;return true;};
        m.LoadHook=(a,w)=>{Assert.That(a,Is.EqualTo(address));Assert.That(w,Is.EqualTo(storedWidth));return value;};
        Assert.That(m.Step(),Is.True);Assert.That(storedWidth,Is.EqualTo(width));
        m.Gpr[0]=0;m.FetchHook=(ulong a,Span<byte> d)=>{Convert.FromHexString(pop).CopyTo(d);return true;};
        Assert.That(m.Step(),Is.True);Assert.That(m.Gpr[4],Is.EqualTo(sp));Assert.That(m.Gpr[0],Is.EqualTo(0x1234UL));
    }

    [TestCase("66c3",0x8000UL,0UL,0x8004UL)]
    [TestCase("66c20600",0x8000UL,0UL,0x800aUL)]
    [TestCase("66c20600",0xfffcUL,0x10000UL,6UL)]
    public void Operand32ReturnInBits16PreservesHighTarget(string hex,ulong sp,ulong ss,ulong finalSp) {
        var m=Machine(hex,XMode.Bits16,sp:sp);m.SegBase[2]=ss;
        var reads=0;m.LoadHook=(a,w)=>{reads++;Assert.That(a,Is.EqualTo(ss+sp));Assert.That(w,Is.EqualTo(32));return 0x11016;};
        Assert.That(m.Step(),Is.True);Assert.That(m.Ip,Is.EqualTo(0x11016UL));
        Assert.That(m.Gpr[4],Is.EqualTo(finalSp));Assert.That(reads,Is.EqualTo(1));
    }

    [TestCase("c3",0x8002UL)]
    [TestCase("c20600",0x8008UL)]
    public void Operand16ReturnStillTruncatesTarget(string hex,ulong finalSp) {
        var m=Machine(hex,XMode.Bits16);m.LoadHook=(a,w)=>{Assert.That(w,Is.EqualTo(16));return 0x1016;};
        Assert.That(m.Step(),Is.True);Assert.That(m.Ip,Is.EqualTo(0x1016UL));Assert.That(m.Gpr[4],Is.EqualTo(finalSp));
    }

    [TestCase("66c3",0x8000UL)]
    [TestCase("66c20600",0x8006UL)]
    public void Operand32CallReturnFromHighBits16Pc(string ret,ulong finalSp) {
        const ulong pc=0x11000;
        var m=Machine("66e80a000000",XMode.Bits16,pc:pc);var callBytes=Convert.FromHexString("66e80a000000");var retBytes=Convert.FromHexString(ret);
        m.FetchHook=(ulong a,Span<byte> d)=>{(a==pc?callBytes:retBytes).CopyTo(d);return true;};
        ulong saved=0;m.StoreHook=(a,v,w)=>{Assert.That(w,Is.EqualTo(32));saved=v;return true;};m.LoadHook=(a,w)=>saved;
        Assert.That(m.Step(),Is.True);Assert.That(m.Ip,Is.EqualTo(pc+16));
        Assert.That(m.Step(),Is.True);Assert.That(m.Ip,Is.EqualTo(pc+6));Assert.That(m.Gpr[4],Is.EqualTo(finalSp));
    }
}
