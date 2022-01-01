namespace JitBase; 

public interface ILocalVar<T> where T : struct {
	IRuntimeValue<T> Value { get; set; }
}