using Discord;

namespace Barriot.Extensions.Pagination
{
    /// <summary>
    ///     Represents a paginator builder to create a <see cref="Paginator{T}"/> which provides <see cref="Page"/>'s.
    /// </summary>
    /// <typeparam name="T">The argument for which a paginator should be created.</typeparam>
    public class PaginatorBuilder<T>
    {
        private Func<T, FieldFormatter>? _valueFormatter;

        private Func<object, EmbedBuilder>? _embedFormatter;

        private Func<object, ComponentBuilder>? _compFormatter;

        private string _cid = string.Empty;

        /// <summary>
        ///     Overrides the current embed instance of the builder, adding the arguments inside the passed builder.
        /// </summary>
        /// <param name="builder">The embed builder to pass into the paginator builder.</param>
        /// <returns>The builder instance with a new embed builder.</returns>
        public PaginatorBuilder<T> WithEmbed(Func<object, EmbedBuilder> builder)
        {
            _embedFormatter = builder;
            return this;
        }

        /// <summary>
        ///     Adds a pagebuilder to the builder, which provides formatting for pages to be created.
        /// </summary>
        /// <param name="fieldFormatter">The method in which the fields will be formatted.</param>
        /// <returns>The builder instance with a page builder included.</returns>
        public PaginatorBuilder<T> WithPages(Func<T, FieldFormatter> fieldFormatter)
        {
            _valueFormatter = fieldFormatter;
            return this;
        }

        /// <summary>
        ///     Adds a custom ID to the builder with a ulong parameter with usercheck logic, for paging.
        /// </summary>
        /// <param name="customId">The custom ID to add.</param>
        /// <returns>The builder instance with a custom ID included.</returns>
        public PaginatorBuilder<T> WithCustomId(string customId)
        {
            _cid = customId;
            return this;
        }

        /// <summary>
        ///     Overrides the current component instance of the builder, adding the arguments inside the passed builder.
        /// </summary>
        /// <param name="builder">The component builder to pass into the paginator builder.</param>
        /// <returns>The builder instance with a new component builder.</returns>
        public PaginatorBuilder<T> WithComponents(Func<object, ComponentBuilder> builder)
        {
            _compFormatter = builder;
            return this;
        }

        /// <summary>
        ///     Builds a paginator based on the values passed by previous calls to the paginatorbuilder.
        /// </summary>
        /// <returns>A paginator with a generic argument matching the argument passed in the paginatorbuilder.</returns>
        public Paginator<T> Build()
        {
            if (_valueFormatter is null)
                throw new InvalidOperationException("The value formatter is null. Please call 'WithPages' to correct this error.");

            if (_cid is null)
                throw new InvalidOperationException("The custom ID of a paginatorbuilder cannot be null.");

            return new Paginator<T>(_valueFormatter, _embedFormatter, _compFormatter, _cid);
        }
    }
}
