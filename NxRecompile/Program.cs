using NxRecompile;

var exe = new ExeLoader(args[0]);
if(true) {
    var recompiler = new CoreRecompiler(exe);
    recompiler.Recompile();
    Directory.CreateDirectory("recompiledModules");
    recompiler.BuildAndLink("recompiledModules", "libtest.dylib");
}
