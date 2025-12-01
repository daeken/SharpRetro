using CoreArchCompiler;
using LibSharpRetro;
using NxRecompile;

var exe = new ExeLoader(args[0]);
var recompiler = new CoreRecompiler(exe);
recompiler.Recompile();
recompiler.CleanupIR();
var cb = new CodeBuilder();
recompiler.Output(cb);
File.WriteAllText("test.c", cb.Code);

/*try {
    Sh.Run("clang-format", "-i", "-style=file", "test.c");
} catch(Exception ex) {
    Console.Error.WriteLine(ex);
}*/

// /opt/homebrew/opt/llvm/bin/clang-tidy --fix --fix-notes --fix-errors test.c -- -isysroot"$(xcrun --sdk macosx --show-sdk-path)" -DBUILD_LIB -Wno-shift-count-overflow -Wno-unused-value
//Sh.Run("/opt/homebrew/opt/llvm/bin/clang-tidy", "--fix", "test.c");
