using System.Text.RegularExpressions;

namespace SharpRetro.Decompiler;

/// Collect exact spellings of compiler-owned local types for final source
/// refactorings. A conflicting spelling or unresolved auto stays unknown.
public static class CppLocalTypes {
    public static IReadOnlyDictionary<string,string> Collect(CppSequence body,IReadOnlyDictionary<string,string> parameters,
        IReadOnlyDictionary<string,string>? callReturns=null) {
        var result=new Dictionary<string,string>(parameters);
        var declarations=new Dictionary<string,HashSet<string>>();
        IEnumerable<CppLine> Lines(CppStatement s)=>s switch {
            CppLine l=>[l],CppSequence q=>q.Body.SelectMany(Lines),CppScope q=>Lines(q.Body),
            CppIf c=>Lines(c.Then).Concat(Lines(c.Else)),CppWhile w=>Lines(w.Body),CppDoWhile w=>Lines(w.Body),
            CppFor w=>(w.Declares==null?Enumerable.Empty<CppLine>():[new CppLine(w.Initializer+";",w.Declares)]).Concat(Lines(w.Body)),_=>[]
        };
        foreach(var line in Lines(body).Where(l=>l.Declares!=null)) {
            string name=line.Declares!;
            var lexed=CppLexical.Tokenize(line.Text);
            if(!lexed.Valid) continue;
            var token=lexed.Tokens.FirstOrDefault(t=>t.Identifier && t.Text==name);
            if(token.Length==0) continue;
            string type=Regex.Replace(line.Text[..token.Start],@"\[\[[^\]]*\]\]","").Trim();
            bool immutable=type=="const auto";
            if(type is "auto" or "const auto") {
                var rhs=Regex.Match(line.Text[(token.Start+token.Length)..],@"^\s*=(?!=)\s*(.*);\s*$");
                if(!rhs.Success) continue;
                string expression=rhs.Groups[1].Value;
                string? inferred=null;
                if(Regex.IsMatch(expression,@"^[A-Za-z_]\w*$")) inferred=result.GetValueOrDefault(expression);
                var call=Regex.Match(expression,@"^(?<function>[A-Za-z_]\w*)\s*\(");
                if(call.Success) {
                    string function=call.Groups["function"].Value;
                    inferred=function switch {"byte_pointer"=>"u8*","const_byte_pointer"=>"const u8*","address"=>"std::uint64_t",_=>callReturns?.GetValueOrDefault(function)};
                }
                if(inferred==null) continue;
                inferred=Regex.Replace(inferred,@"\s+const$","");
                if(!inferred.Contains('*') && inferred.StartsWith("const ",StringComparison.Ordinal)) inferred=inferred[6..];
                type=immutable ? inferred.EndsWith('*') ? inferred+" const" : "const "+inferred : inferred;
            }
            if(!declarations.TryGetValue(name,out var types)) declarations[name]=types=[];
            types.Add(type);
            if(types.Count==1) result[name]=type;else result.Remove(name);
        }
        return result;
    }
}
