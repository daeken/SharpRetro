using System.Text;
using System.Text.RegularExpressions;
using static SharpRetro.Decompiler.CppLexical;

namespace SharpRetro.Decompiler;

public sealed record CppSharedParameter(string Name,string Type);
public sealed record CppSharedHelper(string Name,IReadOnlyList<CppSharedParameter> Params,IReadOnlyList<string> Captures,string ReturnType,CppSequence Body);
public sealed record CppSharedRegionsResult(CppSequence Body,IReadOnlyList<CppSharedHelper> Helpers,int Occurrences,int StatementsSaved);

/// Share closed emitter-owned regions after declaration placement. Free locals
/// are passed as simple lvalue references: creating the call never reads an
/// uninitialized carrier and preserves aliases and observable free-object identity.
/// Interior bindings must be trivial, address-unobserved and wholly contained.
/// This pass moves neither effects nor loads across a call-site boundary.
public static class CppSharedRegions {
    const int MaxNodes=12000,MaxCandidates=18000,MaxCost=128,MaxSiblings=16,MaxHelpers=8,MaxParameters=8;
    [Flags] enum Flow { Normal=1,Return=2,Throw=4,Break=8,Continue=16,Invalid=32 }
    sealed class Binding(string name,string type,int declaration,bool capture=false) {
        public readonly string Name=name,Type=type;
        public readonly int Declaration=declaration;
        public readonly bool Capture=capture;
        public readonly List<int> Uses=[];
    }
    sealed class Fragment(string text,Lexed lexed) {
        public readonly string Text=text;
        public readonly Lexed Lexed=lexed;
        public readonly Dictionary<int,Binding> Bindings=[];
        public bool Valid=lexed.Valid;
    }
    sealed class Occurrence(int id,CppStatement source) {
        public readonly int Id=id;
        public readonly CppStatement Source=source;
        public int End=id,Cost;
        public Flow Flow;
        public readonly List<Occurrence> Children=[];
        public Fragment? Fragment;
        public Fragment? ForCondition,ForIncrement;
        public Binding? Declaration;
    }
    sealed record Candidate(Occurrence Sequence,int First,int Last,int Start,int End,int Cost,bool Terminal,string Key,
        IReadOnlyList<Binding> Free,IReadOnlyList<Binding> Locals,IReadOnlyList<Binding> Captures);

    static readonly HashSet<string> Keywords=["return","throw","break","continue","true","false","nullptr","const","static_cast","reinterpret_cast","const_cast",
        "std","bool","char","signed","unsigned","short","int","long","void","__int128","bits","u8","u16","u32","u64","u128","maybe_unused"];
    static readonly HashSet<string> Introspection=["decltype","typeid","sizeof","alignof","alignas","noexcept","constexpr","consteval","static_assert","__func__","__FUNCTION__","__PRETTY_FUNCTION__",
        "source_location","__builtin_return_address","__builtin_frame_address","__builtin_constant_p","__LINE__","__FILE__"];
    static readonly HashSet<string> ValueCalls=["static_cast","reinterpret_cast","const_cast","address","pointer","byte_offset","mask","narrow","sext","shift_left","shift_right","shift_arithmetic",
        "rotate_right","signed_key","unsigned_divide","signed_divide","popcount","leading_zeroes","reverse_bits","checked_flag","std::size_t","std::ptrdiff_t"];

    public static CppSharedRegionsResult Factor(CppSequence body,IReadOnlyDictionary<string,string> knownTypes,string returnType,IReadOnlySet<string> captureNames,bool favorReadability=false) {
        CppSharedRegionsResult Unchanged() => new(body,[],0,0);
        var declaredNames=new HashSet<string>(); var identifiers=new HashSet<string>(knownTypes.Keys);
        bool valid=true; int count=0;
        void Inventory(CppStatement statement,int depth) {
            if(++count>MaxNodes || depth>192) { valid=false; return; }
            if(statement is CppLine { Declares:{ } name }) declaredNames.Add(name);
            switch(statement) {
                case CppSequence s: foreach(var child in s.Body) Inventory(child,depth+1); break;
                case CppScope s: Inventory(s.Body,depth+1); break;
                case CppIf s: Inventory(s.Then,depth+1); Inventory(s.Else,depth+1); break;
                case CppWhile s: Inventory(s.Body,depth+1); break;
                case CppDoWhile s: Inventory(s.Body,depth+1); break;
                case CppFor s: if(s.Declares!=null)declaredNames.Add(s.Declares);Inventory(s.Body,depth+1);break;
            }
        }
        Inventory(body,0); if(!valid) return Unchanged();
        var environment=new Dictionary<string,Binding>();
        foreach(var (name,type) in knownTypes) if(!declaredNames.Contains(name) || captureNames.Contains(name)) environment[name]=new(name,type,-1,captureNames.Contains(name));
        foreach(var name in captureNames) environment.TryAdd(name,new(name,knownTypes.GetValueOrDefault(name,""),-1,true));
        var occurrences=new List<Occurrence>();
        Fragment Bind(string text,Dictionary<string,Binding> scope,int owner) {
            var lexed=Tokenize(text); var fragment=new Fragment(text,lexed);
            if(!lexed.Valid) { valid=false; return fragment; }
            var tokens=lexed.Tokens;
            for(int i=0;i<tokens.Count;i++) {
                var token=tokens[i]; if(!token.Identifier) { if(token.Text=="#") fragment.Valid=false; continue; }
                identifiers.Add(token.Text);
                if(Introspection.Contains(token.Text) || token.Text.StartsWith("__builtin_",StringComparison.Ordinal)) fragment.Valid=false;
                bool member=i>0 && tokens[i-1].Text is "." or "->" or "::";
                if(member) continue;
                if(scope.TryGetValue(token.Text,out var binding)) { fragment.Bindings[i]=binding; binding.Uses.Add(owner); continue; }
                if(Keywords.Contains(token.Text) || i+1<tokens.Count && tokens[i+1].Text is "(" or "::" ||
                    token.Text.StartsWith("image_",StringComparison.Ordinal)) continue;
                // Type/template names do not denote free variables. Unknown
                // bare names cannot silently become an uncaptured local.
                if(i+1<tokens.Count && tokens[i+1].Text=="<" && TemplateCall(tokens,i+1)) continue;
                if(TypeWord(token.Text)) continue;
                fragment.Valid=false;
            }
            if(tokens.Any(t=>t.Text is "goto" or "case" or "switch" or "for" or "try" or "catch")) valid=false;
            if(tokens.Count>=2 && tokens[0].Identifier && tokens[1].Text==":") valid=false;
            return fragment;
        }
        Occurrence Build(CppStatement statement,Dictionary<string,Binding> scope) {
            var node=new Occurrence(occurrences.Count,statement); occurrences.Add(node);
            switch(statement) {
                case CppLine line:
                    if(line.Declares is { } name) {
                        var binding=new Binding(name,DeclarationType(line),node.Id); node.Declaration=binding;
                        // The declarator's binding is visible in its initializer.
                        scope[name]=binding;
                    }
                    node.Fragment=Bind(line.Text,scope,node.Id); node.Cost=1+line.Text.Count(c=>c=='\n');
                    node.Flow=LineFlow(node.Fragment); break;
                case CppSequence sequence:
                    foreach(var child in sequence.Body) node.Children.Add(Build(child,scope));
                    node.Flow=SequenceFlow(node.Children); node.Cost=node.Children.Sum(c=>c.Cost); break;
                case CppScope nested:
                    node.Children.Add(Build(nested.Body,new(scope))); node.Flow=node.Children[0].Flow; node.Cost=2+node.Children[0].Cost; break;
                case CppIf conditional:
                    node.Fragment=Bind(conditional.Condition.Expression,scope,node.Id);
                    node.Children.Add(Build(conditional.Then,new(scope))); node.Children.Add(Build(conditional.Else,new(scope)));
                    node.Flow=node.Children[0].Flow|node.Children[1].Flow;
                    node.Cost=conditional.Else.Body.Count==0 ? 2+node.Children[0].Cost : conditional.Else.Body.Count==1 && conditional.Else.Body[0] is CppIf ? 1+node.Children.Sum(c=>c.Cost) : 3+node.Children.Sum(c=>c.Cost); break;
                case CppWhile loop:
                    node.Fragment=Bind(loop.Condition,scope,node.Id); node.Children.Add(Build(loop.Body,new(scope)));
                    node.Flow=Flow.Normal|(node.Children[0].Flow & ~(Flow.Normal|Flow.Break|Flow.Continue)); node.Cost=2+node.Children[0].Cost; break;
                case CppDoWhile loop:
                    node.Fragment=Bind(loop.Condition.Expression,scope,node.Id); node.Children.Add(Build(loop.Body,new(scope)));
                    node.Flow=Flow.Normal|(node.Children[0].Flow & ~(Flow.Normal|Flow.Break|Flow.Continue)); node.Cost=2+node.Children[0].Cost; break;
                case CppFor loop:
                    var loopScope=new Dictionary<string,Binding>(scope);
                    if(loop.Declares is { } induction) {
                        var binding=new Binding(induction,DeclarationType(new(loop.Initializer+";",induction)),node.Id);
                        node.Declaration=binding;loopScope[induction]=binding;
                    }
                    node.Fragment=Bind(loop.Initializer,loopScope,node.Id);
                    node.ForCondition=Bind(loop.Condition,loopScope,node.Id);node.ForIncrement=Bind(loop.Increment,loopScope,node.Id);
                    node.Children.Add(Build(loop.Body,new(loopScope)));
                    node.Flow=Flow.Normal|(node.Children[0].Flow & ~(Flow.Normal|Flow.Break|Flow.Continue));node.Cost=2+node.Children[0].Cost;
                    if(!node.ForCondition.Valid||!node.ForIncrement.Valid)node.Flow|=Flow.Invalid;
                    break;
                default: node.Flow=Flow.Invalid; valid=false; break;
            }
            if(node.Fragment?.Valid==false) node.Flow|=Flow.Invalid;
            node.End=occurrences.Count-1; return node;
        }
        var root=Build(body,environment); if(!valid) return Unchanged();
        var candidates=new List<Candidate>();
        foreach(var sequence in occurrences.Where(n=>n.Source is CppSequence)) {
            for(int first=0;first<sequence.Children.Count && candidates.Count<MaxCandidates;first++) {
                int cost=0;
                for(int last=first;last<sequence.Children.Count && last<first+MaxSiblings;last++) {
                    cost+=sequence.Children[last].Cost; if(cost>MaxCost) break;
                    if(cost<2) continue;
                    var selected=sequence.Children.Skip(first).Take(last-first+1).ToArray(); var flow=SequenceFlow(selected);
                    if((flow & (Flow.Break|Flow.Continue|Flow.Invalid))!=0) continue;
                    bool terminal=(flow & Flow.Normal)==0 && (flow & (Flow.Return|Flow.Throw))!=0;
                    if(!terminal && (flow & (Flow.Return|Flow.Throw))!=0 || terminal && !ReturnTypeAllowed(returnType)) continue;
                    int start=selected[0].Id,end=selected[^1].End;
                    var descendants=occurrences.Skip(start).Take(end-start+1).ToArray();
                    var locals=descendants.Select(n=>n.Declaration).OfType<Binding>().ToArray();
                    if(locals.Any(b=>!Trivial(b.Type) || b.Uses.Any(id=>id<start || id>end))) continue;
                    var localSet=locals.ToHashSet(); var free=new List<Binding>(); var captures=new List<Binding>();
                    bool safe=true;
                    foreach(var node in descendants) foreach(var fragment in new[]{node.Fragment,node.ForCondition,node.ForIncrement}.OfType<Fragment>()) {
                        if(!fragment.Valid || InteriorAddress(fragment,localSet) || TemplateBinding(fragment)) { safe=false; break; }
                        foreach(var binding in fragment.Bindings.OrderBy(p=>p.Key).Select(p=>p.Value)) {
                            if(localSet.Contains(binding)) continue;
                            if(binding.Capture) { if(!captures.Contains(binding)) captures.Add(binding); }
                            else if(!free.Contains(binding)) { if(!Trivial(binding.Type)) { safe=false; break; } free.Add(binding); }
                        }
                        if(!safe || free.Count>MaxParameters) { safe=false; break; }
                    }
                    if(!safe) continue;
                    // A helper must repay the cognitive cost of its interface,
                    // rather than merely save a few printed lines.
                    if(favorReadability&&cost<Math.Max(8,2*free.Count+2))continue;
                    var names=new Dictionary<Binding,string>();
                    for(int i=0;i<locals.Length;i++) names[locals[i]]="@local"+i;
                    for(int i=0;i<free.Count;i++) names[free[i]]="@input"+i;
                    string FragmentKey(Fragment f)=>Rewrite(f,names);
                    string Key(Occurrence n) => n.Source switch {
                        CppLine l=>$"L({n.Declaration?.Type}|{(n.Declaration!=null?names[n.Declaration]:"")}|{l.IsEdgeTransfer}|{l.IsConditionDefinition})"+FragmentKey(n.Fragment!),
                        CppSequence=>"S["+string.Join("\u001e",n.Children.Select(Key))+"]",
                        CppScope=>"B["+Key(n.Children[0])+"]",
                        CppIf c=>"I("+c.Condition.Negated+":"+FragmentKey(n.Fragment!)+")["+string.Join("\u001e",n.Children.Select(Key))+"]",
                        CppWhile=>"W("+FragmentKey(n.Fragment!)+")["+Key(n.Children[0])+"]",
                        CppDoWhile c=>"D("+c.Condition.Negated+":"+FragmentKey(n.Fragment!)+")["+Key(n.Children[0])+"]",
                        CppFor=>"F("+n.Declaration?.Type+"|"+FragmentKey(n.Fragment!)+";"+FragmentKey(n.ForCondition!)+";"+FragmentKey(n.ForIncrement!)+")["+Key(n.Children[0])+"]",_=>"?"
                    };
                    string key=(terminal?"return:":"void:")+string.Join("|",free.Select(b=>TypeKey(b.Type)))+";captures:"+string.Join("|",captures.Select(b=>b.Name+":"+TypeKey(b.Type)))+";"+string.Join("\u001e",selected.Select(Key));
                    candidates.Add(new(sequence,first,last,start,end,cost,terminal,key,free,locals,captures));
                }
            }
        }
        var groups=candidates.GroupBy(c=>c.Key).Where(g=>g.Count()>1).ToArray();
        var used=new List<(int Start,int End)>();
        var replacements=new Dictionary<int,Dictionary<int,(int Last,CppLine Call)>>();
        var helpers=new List<CppSharedHelper>(); int occurrenceCount=0,saved=0;
        string Fresh(string prefix) { int i=0;string name;do{name=prefix+i++;}while(!identifiers.Add(name));return name; }
        List<Candidate> Available(IEnumerable<Candidate> group) {
            var selected=new List<Candidate>();
            foreach(var candidate in group.OrderBy(c=>c.End).ThenBy(c=>c.Start))
                if(!used.Concat(selected.Select(c=>(c.Start,c.End))).Any(r=>candidate.Start<=r.End&&r.Start<=candidate.End)) selected.Add(candidate);
            return selected;
        }
        int Benefit(IReadOnlyList<Candidate> group)=>group.Count<2?0:(group.Count-1)*group[0].Cost-group.Count-2;
        foreach(var group in groups.OrderByDescending(g=>Benefit(Available(g))).ThenBy(g=>g.Key,StringComparer.Ordinal)) {
            if(helpers.Count==MaxHelpers) break;
            var chosen=Available(group); int benefit=Benefit(chosen); if(benefit<=0) continue;
            var exemplar=chosen[0]; var names=new Dictionary<Binding,string>();
            var parameters=new List<CppSharedParameter>();
            foreach(var binding in exemplar.Free) { string name=Fresh("shared_input_");names[binding]=name;parameters.Add(new(name,binding.Type)); }
            foreach(var binding in exemplar.Locals) names[binding]=Fresh("shared_local_");
            var helperBody=new CppSequence(exemplar.Sequence.Children.Skip(exemplar.First).Take(exemplar.Last-exemplar.First+1).Select(n=>Clone(n,names)).ToArray());
            string helperName=Fresh("shared_region_");
            helpers.Add(new(helperName,parameters,exemplar.Captures.Select(b=>b.Name).ToArray(),exemplar.Terminal?returnType:"void",helperBody));
            foreach(var candidate in chosen) {
                if(!replacements.TryGetValue(candidate.Sequence.Id,out var sequence)) replacements[candidate.Sequence.Id]=sequence=[];
                sequence[candidate.First]=(candidate.Last,new((candidate.Terminal?"return ":"")+helperName+"("+string.Join(",",candidate.Free.Select(b=>b.Name))+");"));
            }
            // Else-if rendering can change at a replacement boundary. Measure
            // the actual resulting tree, including helper start/close lines,
            // instead of trusting a context-free statement-count estimate.
            int projected=Printed(body)-Printed((CppSequence)Replace(root))-helpers.Sum(h=>Printed(h.Body)+2);
            if(projected<=saved) {
                foreach(var candidate in chosen) replacements[candidate.Sequence.Id].Remove(candidate.First);
                helpers.RemoveAt(helpers.Count-1); continue;
            }
            foreach(var candidate in chosen) used.Add((candidate.Start,candidate.End));
            occurrenceCount+=chosen.Count; saved=projected;
        }
        if(helpers.Count==0) return Unchanged();
        CppStatement Replace(Occurrence n) {
            if(n.Source is CppSequence) {
                var children=new List<CppStatement>();
                for(int i=0;i<n.Children.Count;i++) {
                    if(replacements.TryGetValue(n.Id,out var ranges) && ranges.TryGetValue(i,out var replacement)) { children.Add(replacement.Call); i=replacement.Last; }
                    else children.Add(Replace(n.Children[i]));
                }
                return new CppSequence(children);
            }
            return n.Source switch {
                CppScope s=>s with { Body=(CppSequence)Replace(n.Children[0]) },
                CppIf s=>s with { Then=(CppSequence)Replace(n.Children[0]),Else=(CppSequence)Replace(n.Children[1]) },
                CppWhile s=>s with { Body=(CppSequence)Replace(n.Children[0]) },
                CppDoWhile s=>s with { Body=(CppSequence)Replace(n.Children[0]) },
                CppFor s=>s with { Body=(CppSequence)Replace(n.Children[0]) },_=>n.Source
            };
        }
        return new((CppSequence)Replace(root),helpers,occurrenceCount,saved);
    }

    static int Printed(CppSequence body) {
        int count=0;CppSourceTree.Emit(body,(text,_)=>count+=1+text.Count(c=>c=='\n'));return count;
    }
    static Flow SequenceFlow(IEnumerable<Occurrence> nodes) {
        Flow result=Flow.Normal;
        foreach(var node in nodes) if((result&Flow.Normal)!=0) result=(result&~Flow.Normal)|node.Flow;
        return result;
    }
    static Flow LineFlow(Fragment fragment) {
        if(!fragment.Valid) return Flow.Invalid;
        var tokens=fragment.Lexed.Tokens;
        if(tokens.Count==0) return Flow.Normal;
        var abrupt=tokens[0].Text switch { "return"=>Flow.Return,"throw"=>Flow.Throw,"break"=>Flow.Break,"continue"=>Flow.Continue,_=>Flow.Normal };
        if(tokens.Any(t=>t.Text is "if" or "while" or "do" or "goto" or "switch" or "for" or "case")) return Flow.Invalid;
        if(tokens.Skip(1).Any(t=>t.Text is "return" or "throw" or "break" or "continue")) return Flow.Invalid;
        if(abrupt!=Flow.Normal && (tokens[^1].Text!=";" || tokens.Count(t=>t.Text==";")!=1)) return Flow.Invalid;
        return abrupt;
    }
    static string Rewrite(Fragment fragment,IReadOnlyDictionary<Binding,string> names) {
        var result=new StringBuilder();int start=0;
        foreach(var (index,binding) in fragment.Bindings.OrderBy(p=>p.Key)) if(names.TryGetValue(binding,out var name)) {
            var token=fragment.Lexed.Tokens[index];result.Append(fragment.Text,start,token.Start-start).Append(name);start=token.Start+token.Length;
        }
        return result.Append(fragment.Text,start,fragment.Text.Length-start).ToString();
    }
    static CppStatement Clone(Occurrence n,IReadOnlyDictionary<Binding,string> names) => n.Source switch {
        CppLine s=>s with { Text=Rewrite(n.Fragment!,names),Declares=n.Declaration is { } b?names[b]:s.Declares },
        CppSequence=>new CppSequence(n.Children.Select(c=>Clone(c,names)).ToArray()),
        CppScope s=>s with { Body=(CppSequence)Clone(n.Children[0],names) },
        CppIf s=>s with { Condition=s.Condition with { Expression=Rewrite(n.Fragment!,names) },Then=(CppSequence)Clone(n.Children[0],names),Else=(CppSequence)Clone(n.Children[1],names) },
        CppWhile s=>s with { Condition=Rewrite(n.Fragment!,names),Body=(CppSequence)Clone(n.Children[0],names) },
        CppDoWhile s=>s with { Condition=s.Condition with { Expression=Rewrite(n.Fragment!,names) },Body=(CppSequence)Clone(n.Children[0],names) },
        CppFor s=>s with { Initializer=Rewrite(n.Fragment!,names),Condition=Rewrite(n.ForCondition!,names),Increment=Rewrite(n.ForIncrement!,names),
            Body=(CppSequence)Clone(n.Children[0],names),Declares=n.Declaration is { } b?names[b]:s.Declares },_=>n.Source
    };
    static string TypeKey(string type)=>string.Concat(Tokenize(type).Tokens.Select(t=>t.Text));
    static bool TypeWord(string text)=>Regex.IsMatch(text,@"^(?:u?int(?:8|16|32|64)_t|size_t|ptrdiff_t|uint(?:8|16|32|64)_t)$");
    static bool Trivial(string type) {
        var lexed=Tokenize(type);if(!lexed.Valid||lexed.Tokens.Count==0)return false;
        string plain=string.Join(" ",lexed.Tokens.Where(t=>t.Text is not "const" and not "*").Select(t=>t.Text)).Replace(" :: ","::",StringComparison.Ordinal);
        if(lexed.Tokens.Any(t=>t.Text is "&" or "&&" or "volatile" or "static" or "thread_local" or "constexpr"))return false;
        return Regex.IsMatch(plain,@"^(?:(?:std::)?(?:u?int(?:8|16|32|64)_t|size_t|ptrdiff_t)|u(?:8|16|32|64|128)|bits|bool|char|int|void|(?:unsigned|signed)(?:\s+(?:char|short|int|long|__int128)){0,2}|short|long(?:\s+long)?|__int128)$");
    }
    static bool ReturnTypeAllowed(string type)=>type=="void"||Trivial(type)||Regex.IsMatch(type,@"^result_[0-9a-f]+$");
    static string DeclarationType(CppLine line) {
        var lexed=Tokenize(line.Text);if(!lexed.Valid||line.Declares==null)return "?";
        var tokens=lexed.Tokens;int start=0;
        if(tokens.Count>3&&tokens[0].Text=="["&&tokens[1].Text=="[") { while(start+1<tokens.Count&&!(tokens[start].Text=="]"&&tokens[start+1].Text=="]"))start++;start+=2; }
        int name=-1;for(int i=start;i<tokens.Count;i++)if(tokens[i].Identifier&&tokens[i].Text==line.Declares){name=i;break;}
        if(name<=start || name+1>=tokens.Count || tokens[name+1].Text is not "=" and not ";")return "?";
        return line.Text[tokens[start].Start..tokens[name].Start].Trim();
    }
    static bool TemplateCall(IReadOnlyList<Token> tokens,int open) {
        int depth=0;
        for(int i=open;i<tokens.Count;i++) {
            if(tokens[i].Text=="<")depth++;else if(tokens[i].Text==">")depth--;else if(tokens[i].Text==">>")depth-=2;
            if(depth<=0)return depth==0&&i+1<tokens.Count&&tokens[i+1].Text=="(";
        }
        return false;
    }
    static bool TemplateBinding(Fragment fragment) {
        var tokens=fragment.Lexed.Tokens;
        for(int i=1;i<tokens.Count;i++) if(tokens[i].Text=="<" && tokens[i-1].Identifier && TemplateCall(tokens,i)) {
            int depth=1;
            for(int j=i+1;j<tokens.Count&&depth>0;j++) {
                if(tokens[j].Text=="<")depth++;else if(tokens[j].Text==">")depth--;else if(tokens[j].Text==">>")depth-=2;
                if(depth>0&&fragment.Bindings.ContainsKey(j))return true;
            }
        }
        return false;
    }
    static bool InteriorAddress(Fragment fragment,IReadOnlySet<Binding> locals) {
        var tokens=fragment.Lexed.Tokens;
        if(tokens.Any(t=>t.Text is "decltype" or "typeid" or "sizeof" or "alignof"))return true;
        foreach(var (index,binding) in fragment.Bindings) if(locals.Contains(binding)) {
            int previous=index-1;while(previous>=0&&tokens[previous].Text=="(")previous--;
            if(previous>=0&&tokens[previous].Text=="&")return true;
            if(tokens.Any(t=>t.Text is "addressof" or "ref" or "cref"))return true;
        }
        for(int i=0;i<tokens.Count;i++) {
            if(tokens[i].Text=="["&&i+1<tokens.Count&&tokens[i+1].Text is "&" or "=" or "]")return true;
            if(!tokens[i].Identifier)continue;
            int open=i+1;string name=tokens[i].Text;
            while(open+1<tokens.Count&&tokens[open].Text=="::"&&tokens[open+1].Identifier){name+="::"+tokens[open+1].Text;open+=2;}
            if(open<tokens.Count&&tokens[open].Text=="<"&&TemplateCall(tokens,open)) {int depth=1;open++;while(open<tokens.Count&&depth>0){if(tokens[open].Text=="<")depth++;else if(tokens[open].Text==">")depth--;else if(tokens[open].Text==">>")depth-=2;open++;}}
            if(open>=tokens.Count||tokens[open].Text!="("||ValueCalls.Contains(name)||Trivial(name))continue;
            int level=1;
            for(int at=open+1;at<tokens.Count&&level>0;at++) {
                if(tokens[at].Text=="(")level++;else if(tokens[at].Text==")")level--;
                if(level>0&&fragment.Bindings.TryGetValue(at,out var binding)&&locals.Contains(binding))return true;
            }
        }
        return false;
    }
}
