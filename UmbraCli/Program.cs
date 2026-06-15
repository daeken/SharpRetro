using UmbraCore;

// (T1')×3 NvnReplay entry: --replay <frame-dir> [repeat]
// Loads an NVNCAP frame and drives it through NvnVulkan
// directly. ~1s vs 280s emulator-boot. See NVNCAP.md.
if(args.Length >= 2 && args[0] == "--replay") {
    var rep = args.Length >= 3 ? int.Parse(args[2]) : 1;
    return UmbraCore.Core.NvnReplay.Run(args[1], rep);
}

var mainLoop = new MainLoop(args[0], args[1]);
return 0;
