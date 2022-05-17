using Discord;

namespace Barriot.Extensions.Pagination
{
    public class Paginator<T>
    {
        private const int pageSize = 10;

        private static Paginator<T>? instance;

        private readonly Func<T, FieldFormatter> _valueFormatter;
        private readonly Func<object, EmbedBuilder>? _embedBuilder;
        private readonly Func<object, ComponentBuilder>? _componentBuilder;
        private readonly string _customId;

        internal Paginator(Func<T, FieldFormatter> valueFormatter, Func<object, EmbedBuilder>? eb, Func<object, ComponentBuilder>? cb, string customId)
        {
            _valueFormatter = valueFormatter;

            _embedBuilder = eb;
            _componentBuilder = cb;
            _customId = customId;

            instance = this;
        }

        /// <summary>
        ///     Attempts to grab a page from the paginator.
        /// </summary>
        /// <param name="pageNumber">The page number to create a page for.</param>
        /// <param name="wildCards">Wildcards to add to the custom Id's of this builder.</param>
        /// <returns>A <see cref="Page"/> for the respective <paramref name="pageNumber"/></returns>
        public Page GetPage(int pageNumber, List<T> entries, object parameter, params string[] wildCards)
        {
            var maxPages = (int)Math.Ceiling((double)(entries.Count / pageSize));

            var index = (pageNumber * pageSize) - pageSize;

            var toGather = pageSize;
            if (index + pageSize >= entries.Count)
                toGather = entries.Count - index;

            var eb = (_embedBuilder?.Invoke(parameter) ?? new EmbedBuilder())
                .WithColor(Color.Blue);

            var range = entries.GetRange(index, toGather);

            foreach (var entry in range)
            {
                var formatter = _valueFormatter(entry);
                eb.AddField(formatter.Title, formatter.Value, formatter.DoInline);
            }

            var cid = _customId + string.Join(',', wildCards);
            var cb = (_componentBuilder?.Invoke(parameter) ?? new ComponentBuilder())
                .WithButton(
                    label: "Previous page",
                    customId: cid + $",{pageNumber - 1}",
                    style: ButtonStyle.Danger,
                    disabled: pageNumber <= 1)
                .WithButton(
                    label: "Next page",
                    customId: cid + $",{pageNumber + 1}",
                    style: ButtonStyle.Primary,
                    disabled: pageNumber >= maxPages);

            eb.WithFooter($"Page {pageNumber}/{maxPages} | Barriot by Rozen.");

            return new Page(eb.Build(), cb.Build());
        }

        /// <summary>
        ///     Checks to see if a paginator exists for the specific generic paginator.
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
            => instance is not null;

        /// <summary>
        ///     Tries to get a paginator from it's generic parameter. 
        /// </summary>
        /// <param name="paginator"></param>
        /// <returns></returns>
        public static bool TryGet(out Paginator<T> paginator)
        {
            if (instance is not null)
            {
                paginator = instance;
                return true;
            }
            else
            {
                paginator = null!;
                return false;
            }
        }
    }
}
