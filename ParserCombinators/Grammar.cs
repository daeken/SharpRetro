namespace ParserCombinators;

public delegate (Bobbin, dynamic)? Pattern(Bobbin text);
	
public class Grammar(Pattern StartPattern) {
    public dynamic Parse(Bobbin text, bool requireAll = true) {
        var ret = StartPattern(text);
        if(ret == null) return null;
        var (remaining, value) = ret.Value;
        if(requireAll && remaining.Length != 0) return null;
        return value;
    }
}