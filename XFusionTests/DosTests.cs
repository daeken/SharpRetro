using XFusionCpu;

namespace XFusionTests;

/// The DOS vehicle (sera ·77). Programs are hand-assembled .COMs — byte-exact
/// classics, verifiable against any DOS reference (or DOSBox).
[TestFixture]
public class DosTests {
	[Test]
	public void HelloWorld() {
		// org 100h: mov dx, 0x109; mov ah, 9; int 21h; int 20h; db "Hello, DOS!$"
		var com = Convert.FromHexString("BA0901" + "B409" + "CD21" + "CD20")
			.Concat("Hello, DOS!$"u8.ToArray()).ToArray();
		var dos = new DosMachine(com);
		Assert.That(dos.Run(), Is.True, "clean exit");
		Assert.That(dos.Output.ToString(), Is.EqualTo("Hello, DOS!"));
		Assert.That(dos.ExitCode, Is.EqualTo(0));
	}

	[Test]
	public void CharLoop() {
		// print 'A'..'E' via a dec/jnz loop:
		// mov cx, 5; mov dl, 'A'
		// L: mov ah, 2; int 21h; inc dl; dec cx; jnz L
		// mov ax, 0x4C07; int 21h        (exit code 7)
		var com = Convert.FromHexString(
			"B90500" + "B241" +
			"B402" + "CD21" + "FEC2" + "49" + "75F7" +
			"B8074C" + "CD21");
		var dos = new DosMachine(com);
		Assert.That(dos.Run(), Is.True);
		Assert.That(dos.Output.ToString(), Is.EqualTo("ABCDE"));
		Assert.That(dos.ExitCode, Is.EqualTo(7));
	}

	[Test]
	public void CallRetRealMode() {
		// call a print-char sub twice:
		// mov dl,'X'; call sub; mov dl,'Y'; call sub; int 20h
		// sub: mov ah,2; int 21h; ret
		var com = Convert.FromHexString(
			"B258" + "E80700" + "B259" + "E80200" + "CD20" +
			"B402" + "CD21" + "C3");
		var dos = new DosMachine(com);
		Assert.That(dos.Run(), Is.True);
		Assert.That(dos.Output.ToString(), Is.EqualTo("XY"));
	}

	[Test]
	public void RetToPspExit() {
		// the .COM idiom: plain RET exits via the pushed 0x0000 → PSP:0 = int 20h
		var com = Convert.FromHexString("B241" + "B402" + "CD21" + "C3");  // print 'A'; ret
		var dos = new DosMachine(com);
		Assert.That(dos.Run(), Is.True, "ret-to-PSP exit");
		Assert.That(dos.Output.ToString(), Is.EqualTo("A"));
	}
}
