using PrettyPrinter;

namespace SharpStationCore; 

public static class Extensions {
	public static T Debug<T>(this T value) {
		Console.WriteLine(value);
		return value;
	}
}