namespace Barriot.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, T replacement)
        {
            bool set = false;
            foreach (var value in enumerable)
            {
                if (set is false && predicate(value))
                {
                    set = true;
                    yield return replacement;
                }
                yield return value;
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="replacementAction"></param>
        /// <returns></returns>
        public static IEnumerable<T> ReplaceAll<T>(this IEnumerable<T> enumerable, Func<T, T> replacementAction)
        {
            foreach(var value in enumerable)
            {
                yield return replacementAction(value);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="entry"></param>
        public static void ObservableAdd<T>(this List<T> list, T entry)
        {
            list.ObservableAddRange(new[] { entry });
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="entries"></param>
        public static void ObservableAddRange<T>(this List<T> list, IEnumerable<T> entries)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            list = list.Concat(entries).ToList();
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        public static void ObservableRemove<T>(this List<T> list, Predicate<T> predicate)
        {
            list = list.Where(x => !predicate(x)).ToList();
        }
    }
}
