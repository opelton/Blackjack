using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions {

    public static IEnumerable<t> Randomize<t>(this IEnumerable<t> target) {
        System.Random r = new System.Random();

        return target.OrderBy(x => (r.Next()));
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> container) {
        return container == null || !container.Any();
    }
}