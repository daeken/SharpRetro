namespace JitBase; 

public static class Helpers {
	public static bool IsSigned<U>() => default(U) is sbyte or short or int or long;
	public static void IsSigned<U>(Action if_, Action else_) {
		if(default(U) is sbyte or short or int or long)
			if_();
		else
			else_();
	}
}