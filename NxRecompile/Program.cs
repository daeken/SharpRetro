using CoreArchCompiler;
using NxRecompile;

var exe = new ExeLoader(args[0]);
var recompiler = new CoreRecompiler(exe);
recompiler.Recompile();
recompiler.CleanupIR();
var cb = new CodeBuilder();
recompiler.Output(cb);
File.WriteAllText("test.c", cb.Code);