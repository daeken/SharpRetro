namespace LibSharpRetro; 

public static class FunctionalHelpers {
	public static Action<T> Funcify<T>(Action<T> func) => func;
	public static Func<T> Funcify<T>(Func<T> func) => func;
	public static Func<TIn, TOut> Funcify<TIn, TOut>(Func<TIn, TOut> func) => func;
}