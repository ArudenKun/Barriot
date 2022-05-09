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

        /// <summary>
        ///     The embed builder of this paginator.
        /// </summary>
        public EmbedBuilder EmbedBuilder { get; set; } = new();

        /// <summary>
        ///     The component builder of this paginator.
        /// </summary>
        public ComponentBuilder ComponentBuilder { get; set; } = new();

        /// <summary>
        ///     The custom ID of this paginator.
        /// </summary>
        public string CustomId { get; set; } = string.Empty;

        /// <summary>
        ///     Overrides the current embed instance of the builder, adding the arguments inside the passed builder.
        /// </summary>
        /// <param name="builder">The embed builder to pass into the paginator builder.</param>
        /// <returns>The builder instance with a new embed builder.</returns>
        public PaginatorBuilder<T> WithEmbed(EmbedBuilder builder)
        {
            EmbedBuilder = builder;
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
        ///     Adds a custom ID to the builder, for paging.
        /// </summary>
        /// <param name="customid">The custom ID to add.</param>
        /// <returns>The builder instance with a custom ID included.</returns>
        public PaginatorBuilder<T> WithCustomId(string customid)
        {
            CustomId = customid;
            return this;
        }

        /// <summary>
        ///     Overrides the current component instance of the builder, adding the arguments inside the passed builder.
        /// </summary>
        /// <param name="builder">The component builder to pass into the paginator builder.</param>
        /// <returns>The builder instance with a new component builder.</returns>
        public PaginatorBuilder<T> WithComponents(ComponentBuilder builder)
        {
            ComponentBuilder = builder;
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

            if (string.IsNullOrEmpty(CustomId))
                throw new InvalidOperationException("The custom ID of a paginatorbuilder cannot be empty.");

            return new Paginator<T>(_valueFormatter, EmbedBuilder, ComponentBuilder, CustomId);
        }
    }
}
