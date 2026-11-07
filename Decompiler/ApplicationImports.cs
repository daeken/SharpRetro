namespace SharpRetro.Decompiler;

public sealed record HostImport(int Arguments,string Expression,bool Void=false,bool Variadic=false);

/// Explicit libSystem integer/pointer ABI contracts. The algorithm in the input
/// executable is never delegated to a host replacement.
public static class ApplicationImports {
    static readonly int[] Registers=[7,6,2,1,8,9];
    static readonly IReadOnlyDictionary<string,HostImport> Functions=new Dictionary<string,HostImport> {
        ["_read"]=new(3,"input_output(true,a_7,a_6,a_2)"),
        ["_write"]=new(3,"input_output(false,a_7,a_6,a_2)"),
        ["_strcmp"]=new(2,"std::uint64_t(std::uint32_t(std::strcmp(c_string(a_7),c_string(a_6))))"),
        ["_malloc"]=new(1,"host_allocate(a_7)"),
        ["_free"]=new(1,"host_release(a_7)",Void:true),
        ["_fopen"]=new(2,"address(std::fopen(c_string(a_7),c_string(a_6)))"),
        ["_fclose"]=new(1,"std::uint64_t(std::uint32_t(std::fclose(file(a_7))))"),
        ["_fgetc"]=new(1,"std::uint64_t(std::uint32_t(std::fgetc(file(a_7))))"),
        ["_fread"]=new(4,"host_fread(a_7,a_6,a_2,a_1)"),
        ["_fwrite"]=new(4,"host_fwrite(a_7,a_6,a_2,a_1)"),
        ["_perror"]=new(1,"std::perror(c_string(a_7))",Void:true),
        ["_printf"]=new(6,"format_output(stdout,a_7,{a_6,a_2,a_1,a_8,a_9},a_0)",Variadic:true),
        ["_fprintf"]=new(6,"format_output(file(a_7),a_6,{a_2,a_1,a_8,a_9},a_0)",Variadic:true),
        ["memcpy"]=new(3,"address(std::memcpy(pointer(a_1),pointer(a_2),std::size_t(a_8)))"),
        ["memmove"]=new(3,"address(std::memmove(pointer(a_1),pointer(a_2),std::size_t(a_8)))"),
        ["memset"]=new(3,"address(std::memset(pointer(a_1),int(a_2&255),std::size_t(a_8)))")
    };
    public static HostImport Get(string symbol) => Functions.TryGetValue(symbol,out var result) ? result
        : throw new NotSupportedException("No application ABI bridge for import "+symbol);
    public static IReadOnlySet<ValueSlot> Inputs(string symbol,bool windows=false) {
        var spec=Get(symbol);
        var result=(windows ? new[] {1,2,8,9} : Registers).Take(spec.Arguments).Select(i=>new ValueSlot(SlotKind.Local,i)).ToHashSet();
        result.Add(FunctionRecovery.Stack);
        if(spec.Variadic) result.Add(FunctionRecovery.Accumulator); // AL counts x86 vector arguments.
        return result;
    }
    public static IReadOnlySet<ValueSlot> Results(string symbol) => Get(symbol).Void ? new HashSet<ValueSlot>() : FunctionRecovery.ImportResults;
    public static string BindingExpression(string symbol) => symbol switch {
        "___stdinp"=>"address(&stdin)", "___stdoutp"=>"address(&stdout)", "___stderrp"=>"address(&stderr)",
        // The binder may describe lazy-function slots as well as data imports.
        // Function execution is still restricted to recovered direct targets.
        "dyld_stub_binder"=>"reinterpret_cast<std::uintptr_t>(&unsupported_loader_stub)",
        _ => Functions.ContainsKey(symbol) ? $"reinterpret_cast<std::uintptr_t>(&::{symbol[1..]})"
            : throw new NotSupportedException("No application data binding for "+symbol)
    };
}
