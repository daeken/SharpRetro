namespace StaticRecompilerBase;

public static class Extensions {
    extension(string value) {
        public string Indent() => string.Join('\n', value.Split('\n').Select(x => $"\t{x}"));
    }
}