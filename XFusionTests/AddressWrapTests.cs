using LiftIl;
using XFusionCpu;

namespace XFusionTests;

/// Effective offsets wrap at address size before segment-base addition. These
/// synthetic probes use hooks so no allocation at guest boundary addresses is needed.
[TestFixture]
public class AddressWrapTests {
	static X86Machine Machine(string hex, XMode mode, Action<X86Machine>? init = null) {
		var code=Convert.FromHexString(hex);
		var m=new X86Machine{Mode=mode,Ip=0x1000};
		m.FetchHook=(ulong address,Span<byte> dst)=>{code.CopyTo(dst);return true;};
		init?.Invoke(m);return m;
	}

	[TestCase("8b4308",XMode.Bits32,0xFFFFFFFCUL,0UL,4UL)] // [ebx+8]
	[TestCase("8b4380",XMode.Bits32,0x10UL,0UL,0xFFFFFF90UL)] // negative disp8
	[TestCase("8b448b08",XMode.Bits32,0xFFFFFFFCUL,0x40000000UL,4UL)] // [ebx+ecx*4+8]
	[TestCase("8b0500000080",XMode.Bits32,0UL,0UL,0x80000000UL)] // absolute disp32, sign bit
	[TestCase("678b4008",XMode.Bits32,0xFFFCUL,0UL,4UL)] // 16-bit [bx+si+8]
	[TestCase("678b4080",XMode.Bits32,0x10UL,0UL,0xFF90UL)] // 16-bit negative disp8
	[TestCase("678b060080",XMode.Bits32,0UL,0UL,0x8000UL)] // absolute disp16
	[TestCase("8b4008",XMode.Bits16,0xFFFCUL,0UL,4UL)] // default16 [bx+si+8]
	[TestCase("678b4308",XMode.Bits16,0xFFFFFFFCUL,0UL,4UL)] // override32
	[TestCase("678b4308",XMode.Bits64,0xFFFFFFFFFFFFFFFCUL,0UL,4UL)] // addr32 ignores upper RBX
	[TestCase("8b4308",XMode.Bits64,0x00000000FFFFFFFCUL,0UL,0x100000004UL)] // default64 no 32 wrap
	[TestCase("8b4380",XMode.Bits64,0x10UL,0UL,0xFFFFFFFFFFFFFF90UL)] // 64-bit signed displacement
	public void LoadOffsetUsesAddressWidth(string hex,XMode mode,ulong b,ulong index,ulong expected) {
		var m=Machine(hex,mode,x=>{x.Gpr[3]=b;x.Gpr[1]=index;x.Gpr[6]=index;x.Flags=0xAD7;});
		ulong? address=null;var reads=0;
		m.LoadHook=(a,w)=>{address=a;reads++;return 0x1234;};
		Assert.That(m.Step(),Is.True);Assert.That(address,Is.EqualTo(expected));
		Assert.That(reads,Is.EqualTo(1));Assert.That(m.Gpr[0],Is.EqualTo(0x1234UL));
		Assert.That(m.Gpr[3],Is.EqualTo(b));Assert.That(m.Flags,Is.EqualTo(0xAD7UL));
	}

	[TestCase("648b4308",XMode.Bits32,0xFFFFFFFCUL,0x2000UL,0x2004UL)]
	[TestCase("64678b4008",XMode.Bits32,0xFFFCUL,0x100000UL,0x100004UL)]
	[TestCase("64678b4308",XMode.Bits64,0xFFFFFFFCUL,0x100000000UL,0x100000004UL)]
	public void SegmentBaseIsAddedAfterOffsetWrap(string hex,XMode mode,ulong b,ulong segment,ulong expected) {
		var m=Machine(hex,mode,x=>{x.Gpr[3]=b;x.SegBase[4]=segment;});
		ulong? address=null;m.LoadHook=(a,w)=>{address=a;return 1;};
		Assert.That(m.Step(),Is.True);Assert.That(address,Is.EqualTo(expected));
	}

	[TestCase("894308",XMode.Bits32,0xFFFFFFFCUL,4UL)]
	[TestCase("67894008",XMode.Bits32,0xFFFCUL,4UL)]
	[TestCase("67894308",XMode.Bits64,0xFFFFFFFCUL,4UL)]
	public void StoreUsesTheSameWrappedOffset(string hex,XMode mode,ulong b,ulong expected) {
		var m=Machine(hex,mode,x=>{x.Gpr[3]=b;x.Gpr[0]=0x12345678;});
		var writes=0;m.StoreHook=(a,v,w)=>{Assert.That(a,Is.EqualTo(expected));Assert.That(v,Is.EqualTo(0x12345678UL));Assert.That(w,Is.EqualTo(32));writes++;return true;};
		Assert.That(m.Step(),Is.True);Assert.That(writes,Is.EqualTo(1));
	}

	[TestCase("678d4008",XMode.Bits32,0xFFFCUL,4UL)]
	[TestCase("678d4080",XMode.Bits32,0x10UL,0xFF90UL)]
	[TestCase("488d4308",XMode.Bits64,0xFFFFFFFCUL,0x100000004UL)]
	[TestCase("67488d4308",XMode.Bits64,0xFFFFFFFCUL,4UL)]
	public void LeaReturnsOffsetWithoutMemoryEffects(string hex,XMode mode,ulong b,ulong expected) {
		var m=Machine(hex,mode,x=>{x.Gpr[3]=b;x.Flags=0xAD7;});
		m.LoadHook=(a,w)=>throw new Exception("LEA must not load");
		m.StoreHook=(a,v,w)=>throw new Exception("LEA must not store");
		Assert.That(m.Step(),Is.True);Assert.That(m.Gpr[0],Is.EqualTo(expected));Assert.That(m.Flags,Is.EqualTo(0xAD7UL));
	}

	[TestCase("678b0500000000",XMode.Bits64,0x100001000UL,0x1007UL)] // EIP-relative
	[TestCase("8b0500000000",XMode.Bits64,0x100001000UL,0x100001006UL)] // RIP-relative control
	public void PcRelativeOffsetUsesEffectiveAddressWidth(string hex,XMode mode,ulong pc,ulong expected) {
		var m=Machine(hex,mode);m.Ip=pc;ulong? address=null;m.LoadHook=(a,w)=>{address=a;return 1;};
		Assert.That(m.Step(),Is.True);Assert.That(address,Is.EqualTo(expected));
	}
}
