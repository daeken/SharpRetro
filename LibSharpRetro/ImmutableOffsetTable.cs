using System.Numerics;

namespace LibSharpRetro;

public class ImmutableOffsetTable<KeyT, ValueT> where KeyT : INumber<KeyT> {
    readonly KeyT[] Offsets;
    readonly (KeyT Size, ValueT Value)[] Entries;

    public ImmutableOffsetTable(IEnumerable<(KeyT Offset, KeyT Size, ValueT Value)> entries) {
        var elist = entries.OrderBy(x => x.Offset).ToList();
        Offsets = new KeyT[elist.Count];
        Entries = new (KeyT Size, ValueT Value)[elist.Count];

        KeyT lastEnd = default;
        for(var i = 0; i < elist.Count; i++) {
            var (offset, size, value) = elist[i];
            if(i != 0 && offset < lastEnd)
                throw new NotSupportedException($"Entry {i} overlaps with {i - 1} -- {lastEnd} vs {offset}-{offset + size}");
            lastEnd = offset + size;
            Offsets[i] = offset;
            Entries[i] = (size, value);
        }
    }

    public bool Contains(KeyT index) {
        var ind = Array.BinarySearch(Offsets, index);
        if (ind < 0) ind = ~ind - 1;
        if(ind < 0) return false;
        var offset = Offsets[ind];
        var size = Entries[ind].Size;
        return index >= offset && index - offset < size;
    }

    public bool TryGetEntry(KeyT index, out KeyT offset, out KeyT size, out ValueT value) {
        offset = size = default;
        value = default;
        
        var ind = Array.BinarySearch(Offsets, index);
        if (ind < 0) ind = ~ind - 1;
        if(ind < 0) return false;
        offset = Offsets[ind];
        (size, value) = Entries[ind];
        return index >= offset && index - offset < size;
    }

    public bool TryGetValue(KeyT index, out ValueT value) => TryGetEntry(index, out _, out _, out value);
}