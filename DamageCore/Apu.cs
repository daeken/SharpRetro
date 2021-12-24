using LibSharpRetro;

namespace DamageCore; 

public class Apu {
	readonly Core Core;
	readonly IAudioBackend AudioBackend;
	
	public Apu(Core core) {
		Core = core;
		AudioBackend = Core.AudioBackend;
		core.Timing.TimeSync(Run());
	}

	IEnumerable<ulong> Run() {
		var i = 0;
		while(true) {
			AudioBackend.AddSamples(Enumerable.Range(0, 739).Select(_ => (short) (MathF.Sin(i++ / 44100f * 440 * 2 * MathF.PI) * 16384)).Select(x => new[] { x, x }).SelectMany(x => x));
			yield return 70224;
		}
	}
}