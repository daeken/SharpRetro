namespace ParserCombinators;

public readonly struct Bobbin {
    public readonly Dictionary<(Pattern, int), (Bobbin, dynamic)?> Memoization = [];
    public readonly string String;
    public readonly int Start, End;
    public readonly int Length, TotalLength;

    public Bobbin(string str, int start = 0, int end = -1) {
        String = str;
        Start = start;
        TotalLength = str.Length;
        End = end == -1 || end > TotalLength ? TotalLength : end;
        Length = End - Start;
    }

    public Bobbin(Bobbin other, int start = -1, int end = -1) {
        Memoization = other.Memoization;
        String = other.String;
        TotalLength = other.TotalLength;
        Start = start == -1 ? other.Start : start;
        End = end == -1 ? other.End : end;
        Length = End - Start;
    }
		
    public Bobbin Forward(int count) => new(this, Start + count);

    public override string ToString() => Start == 0 && End == TotalLength ? String : String.Substring(Start, Length);
		
    public static implicit operator Bobbin(string value) => new(value);
    public static implicit operator string(Bobbin value) => value.ToString();
}