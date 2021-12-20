namespace LibSharpRetro; 

public static class UsefulExtensions {
	public static bool HasBit(this byte v, int bit) => ((v >> bit) & 1) != 0;
}