namespace CoreLib.Application.Common.Utility
{
    public static class CollectionExtension
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);

            if (list is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }

        public static IEnumerable<TSource> ListDistinctBy<TSource, TKey>(this IEnumerable<TSource> src, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = [];
            foreach (TSource element in src)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
    }
}
