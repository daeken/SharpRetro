namespace SharpRetro.Decompiler;

public sealed record NativeObjectField(long Offset,int Width,string Name);
public sealed record NativeObjectView(string Name,string Variable,SsaDefinition Base,bool ReadOnly,IReadOnlyList<NativeObjectField> Fields);

/// Observed nonoverlapping fields are named accessor views, not invented C++
/// allocation bounds. Accessors retain modular address formation and the native
/// aligned(1),may_alias scalar views. A real frame array has a separate bounds proof.
public sealed class NativeObjectViews {
    public IReadOnlyList<NativeObjectView> Views { get; }
    readonly IReadOnlyDictionary<(ulong,int,int), (NativeObjectView View,NativeObjectField Field)> accesses;
    NativeObjectViews(IReadOnlyList<NativeObjectView> views,Dictionary<(ulong,int,int),(NativeObjectView,NativeObjectField)> accesses) {
        Views=views;this.accesses=accesses;
    }
    public static NativeObjectViews Analyze(ulong entry,NativePointerPlan plan) {
        var views=new List<NativeObjectView>();
        var accessors=new Dictionary<(ulong,int,int),(NativeObjectView,NativeObjectField)>();
        foreach(var group in plan.Accesses.Where(a=>a.Base.Kind==SsaDefinitionKind.Parameter && a.Offset!=null)
                    .GroupBy(a=>a.Base.Root)) {
            var spans=group.Select(a=>(Offset:a.Offset!.Value,a.Width)).Distinct().OrderBy(a=>a.Offset).ToArray();
            if(spans.Length<2 || spans.Length>32 || spans.Any(a=>a.Width is not (8 or 16 or 32 or 64 or 128))) continue;
            bool overlaps=false;
            for(int i=1;i<spans.Length;i++) if((Int128)spans[i-1].Offset+spans[i-1].Width/8>spans[i].Offset) overlaps=true;
            if(overlaps) continue;
            string suffix=$"{entry:x}_{group.Key.Slot.Index}";
            var fields=spans.Select(a=>new NativeObjectField(a.Offset,a.Width,
                a.Offset<0 ? $"field_minus_{unchecked(0UL-(ulong)a.Offset):x}" : $"field_{a.Offset:x}")).ToArray();
            var view=new NativeObjectView("object_view_"+suffix,"object_"+suffix,group.Key,plan.IsReadOnly(group.Key),fields);
            views.Add(view);
            foreach(var access in group) {
                var field=fields.Single(f=>f.Offset==access.Offset && f.Width==access.Width);
                accessors[(access.Block,access.Address.Id,access.Width)]=(view,field);
            }
        }
        return new(views,accessors);
    }
    public string? Access(ulong block,RecoveredValue address,int width) => accesses.TryGetValue((block,address.Id,width),out var access)
        ? $"{access.View.Variable}.{access.Field.Name}()" : null;
    public IEnumerable<string> Declarations() {
        foreach(var view in Views) {
            yield return $"struct {view.Name} {{";
            yield return $"    {(view.ReadOnly ? "const " : "")}u8* base;";
            foreach(var field in view.Fields) {
                string type=field.Width==128 ? "u128" : "u"+field.Width;
                string cv=view.ReadOnly ? "const " : "";
                string offset=field.Offset>=0 ? $"0x{field.Offset:x}ULL" : $"(std::uint64_t(0)-0x{unchecked(0UL-(ulong)field.Offset):x}ULL)";
                yield return $"    {cv}{type}& {field.Name}() const {{ return *reinterpret_cast<{cv}{type}*>(byte_offset(base,{offset})); }}";
            }
            yield return "};";
        }
    }
}
