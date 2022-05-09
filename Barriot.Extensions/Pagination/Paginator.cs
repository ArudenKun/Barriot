﻿using Discord;

namespace Barriot.Extensions.Pagination
{
    public class Paginator<T>
    {
        private const int pageSize = 10;

        private static Paginator<T>? instance;

        private readonly Func<T, FieldFormatter> _valueFormatter;

        private readonly EmbedBuilder _embedBuilder;
        private readonly ComponentBuilder _componentBuilder;

        private readonly string _customId;

        internal Paginator(Func<T, FieldFormatter> valueFormatter, EmbedBuilder eb, ComponentBuilder cb, string customId)
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
        /// <returns>A <see cref="Page"/> for the respective <paramref name="pageNumber"/></returns>
        public Page GetPage(int pageNumber, List<T> entries)
        {
            var maxPages = (int)Math.Ceiling((double)(entries.Count / pageSize));

            var index = (pageNumber * pageSize) - pageSize;

            var toGather = pageSize;
            if (index + pageSize >= entries.Count)
                toGather = entries.Count - index;

            var range = entries.GetRange(index, toGather);

            foreach (var entry in range)
            {
                var formatter = _valueFormatter(entry);
                _embedBuilder.AddField(formatter.Title, formatter.Value, formatter.DoInline);
            }

            _componentBuilder
                .WithButton(
                    label: "Previous page", 
                    customId: _customId + $",{pageNumber - 1}", 
                    style: ButtonStyle.Danger, 
                    disabled: pageNumber <= 1)
                .WithButton(
                    label: "Next page", 
                    customId: _customId + $",{pageNumber + 1}", 
                    style: ButtonStyle.Primary, 
                    disabled: pageNumber >= maxPages);

            _embedBuilder
                .WithFooter($"Page {pageNumber}/{maxPages} | Barriot by Rozen.");

            return new Page(_embedBuilder.Build(), _componentBuilder.Build());
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