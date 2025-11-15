using NxRecompile;

var exe = new ExeLoader(args[0]);
var recompiler = new CoreRecompiler(exe);
recompiler.Recompile();