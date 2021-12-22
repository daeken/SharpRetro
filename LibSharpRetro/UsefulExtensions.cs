namespace LibSharpRetro; 

public static class UsefulExtensions {
	public static bool HasBit(this byte v, int bit) => ((v >> bit) & 1) != 0;

	public static void ForEach<T>(this IEnumerable<T> seq, Action<T> func) {
		foreach(var elem in seq)
			func(elem);
	}

	public static void ForEach<T>(this IEnumerable<T> seq, Action<T, int> func) {
		var i = 0;
		foreach(var elem in seq)
			func(elem, i++);
	}
}