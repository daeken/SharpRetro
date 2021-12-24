namespace LibSharpRetro; 

public interface IAudioBackend {
	int SampleRate { get; set; }
	int Channels { get; set; }

	void AddSamples(IEnumerable<short> data);
}