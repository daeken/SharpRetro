using System.Numerics;

namespace NxTranslate;

public static class Extensions {
    public static IEnumerable<IReadOnlyList<T>> GroupByProximity<T, U>(this IEnumerable<T> source, U maxGap, Func<T, U> selector) where U : INumber<U> {
        using var e = source.GetEnumerator();
        if (!e.MoveNext()) yield break;

        var group = new List<T> { e.Current };
        var prevAddr = selector(e.Current);

        while(e.MoveNext()) {
            var cur = e.Current;
            var curAddr = selector(cur);
            if(curAddr - prevAddr < maxGap)
                group.Add(cur);
            else {
                yield return group;
                group = [cur];
            }
            prevAddr = curAddr;
        }

        yield return group;
    }
}