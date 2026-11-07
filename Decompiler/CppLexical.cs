namespace SharpRetro.Decompiler;

// Shared lexical locations for emitter-owned source passes. Unsupported or
// unterminated fragments refuse analysis; literal/comment text is preserved.
internal static class CppLexical {
    internal readonly record struct Token(int Start,int Length,string Text,bool Identifier=false);
    internal sealed record Lexed(IReadOnlyList<Token> Tokens,bool Valid,IReadOnlyList<(int Start,int Length)>? Comments=null);
    // This scanner only classifies identifier/punctuation locations. All text
    // between replacements is copied byte-for-byte; comments, quoted/character
    // literals and raw literals are never interpreted as carrier identifiers.
    // Unterminated fragments conservatively disable the complete pass instead
    // of trying to carry lexical state across separately emitted AST lines.
    internal static Lexed Tokenize(string text) {
        var tokens=new List<Token>(); var comments=new List<(int Start,int Length)>(); int i=0;
        bool Start(char c) => c=='_' || char.IsLetter(c) || c>=128;
        bool Part(char c) => Start(c) || char.IsDigit(c);
        bool Literal(out int end) {
            end=i;
            string? prefix=new[] {"u8R\"","uR\"","UR\"","LR\"","R\"","u8\"","u\"","U\"","L\"","\"","u8'","u'","U'","L'","'"}
                .FirstOrDefault(p=>text.AsSpan(i).StartsWith(p,StringComparison.Ordinal));
            if(prefix==null) return false;
            int quote=i+prefix.Length-1;
            if(prefix.Contains('R')) {
                int open=text.IndexOf('(',quote+1);
                if(open<0 || open-quote-1>16) { end=-1; return true; }
                string delimiter=text[(quote+1)..open];
                if(delimiter.Any(c=>char.IsWhiteSpace(c) || c is ')' or '\\')) { end=-1; return true; }
                string close=")"+delimiter+"\""; int found=text.IndexOf(close,open+1,StringComparison.Ordinal);
                end=found<0 ? -1 : found+close.Length;
            } else {
                char terminator=text[quote]; int at=quote+1;
                while(at<text.Length) {
                    if(text[at]=='\\') { at+=2; continue; }
                    if(text[at]==terminator) { end=at+1; break; }
                    if(text[at] is '\r' or '\n') { end=-1; return true; }
                    at++;
                }
                if(end==i) end=-1;
            }
            if(end>=0) while(end<text.Length && Part(text[end])) end++;
            return true;
        }
        while(i<text.Length) {
            char c=text[i];
            if(char.IsWhiteSpace(c)) { i++; continue; }
            if(c=='/' && i+1<text.Length && text[i+1]=='/') {
                int newline=text.IndexOf('\n',i+2),commentEnd=newline<0 ? text.Length : newline;
                int last=commentEnd-1;
                while(last>=i && text[last] is '\r' or ' ' or '\t') last--;
                // Translation-phase line splicing precedes C++ comment
                // recognition. The continuation can cross this AST fragment's
                // implicit emitted newline, so refuse a trailing backslash
                // even when the fragment itself contains no newline yet.
                if(last>=i && text[last]=='\\') return new(tokens,false);
                comments.Add((i,commentEnd-i)); i=newline<0 ? text.Length : newline+1; continue;
            }
            if(c=='/' && i+1<text.Length && text[i+1]=='*') {
                int close=text.IndexOf("*/",i+2,StringComparison.Ordinal);
                if(close<0) return new(tokens,false);
                comments.Add((i,close+2-i));
                i=close+2; continue;
            }
            if(Literal(out int end)) {
                if(end<0) return new(tokens,false);
                i=end; continue;
            }
            int start=i;
            if(Start(c)) {
                i++; while(i<text.Length && Part(text[i])) i++;
                tokens.Add(new(start,i-start,text[start..i],true)); continue;
            }
            if(char.IsDigit(c) || c=='.' && i+1<text.Length && char.IsDigit(text[i+1])) {
                i++;
                while(i<text.Length && (Part(text[i]) || text[i] is '.' or '\'' ||
                      text[i] is '+' or '-' && text[i-1] is 'e' or 'E' or 'p' or 'P')) i++;
                tokens.Add(new(start,i-start,text[start..i])); continue;
            }
            if(c=='\\') return new(tokens,false); // escaped identifiers/line splices
            string symbol=i+1<text.Length ? text.Substring(i,2) : c.ToString();
            if(symbol is "::" or "->" or "++" or "--" or "&&" or "||" or "==" or "!=" or "<=" or ">=" or "<<" or ">>" or "+=" or "-=" or "*=" or "/=" or "%=" or "&=" or "|=" or "^=") i+=2;
            else { symbol=c.ToString(); i++; }
            tokens.Add(new(start,i-start,symbol));
        }
        return new(tokens,true,comments);
    }
}
