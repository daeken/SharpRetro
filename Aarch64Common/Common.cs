using CoreArchCompiler;

namespace Aarch64Common;

using CoreArchCompiler;

public static class Common {
    static int HighestSetBit(ulong v, int bits) {
        for(var i = bits - 1; i >= 0; --i)
            if((v & (1UL << i)) != 0)
                return i;
        return -1;
    }

    static ulong ZeroExtend(ulong v, int bits) => v & Ones(bits);
    static ulong Ones(int bits) => Enumerable.Range(0, bits).Select(i => 1UL << i).Aggregate((a, b) => a | b);

    static ulong Replicate(ulong v, int bits, int start, int rep, int ext) {
        var repval = (v >> start) & Ones(rep);
        var times = ext / rep;
        var val = 0UL;
        for(var i = 0; i < times; ++i)
            val = (val << rep) | repval;
        return v | (val << start);
    }

    static ulong RollRight(ulong v, int size, int rotate) => ((v << (size - rotate)) | (v >> rotate)) & Ones(size);
    
    static (ulong, ulong) MakeMasks(uint n, uint imms, uint immr, int m, bool immediate) {
        var len = HighestSetBit((n << 6) | (imms ^ 0b111111U), 7);
        if(!(len > 0 && m >= 1 << len)) throw new BailoutException();

        var levels = ZeroExtend(Ones(len), 6);
        if(!(!immediate || (imms & levels) != levels)) throw new BailoutException();
            
        var S = imms & levels;
        var R = immr & levels;

        var diff = (S - R) & 0b111111;
        var esize = 1 << len;
        var d = diff & Ones(len);

        var welem = ZeroExtend(Ones((int) (S + 1)), esize);
        var telem = ZeroExtend(Ones((int) (d + 1)), esize);

        var wmask = Replicate(RollRight(welem, esize, (int) R), esize, 0, esize, m);
        var tmask = Replicate(telem, esize, 0, esize, m);
        return (wmask, tmask);
    }

    public static ulong MakeWMask(uint n, uint imms, uint immr, long m, int immediate) => MakeMasks(n, imms, immr, (int) m, immediate != 0).Item1;
    public static ulong MakeTMask(uint n, uint imms, uint immr, long m, int immediate) => MakeMasks(n, imms, immr, (int) m, immediate != 0).Item2;
}