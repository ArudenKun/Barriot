namespace Barriot.Pagination
{
    /// <summary>
    ///     Represents a paginator builder to create a <see cref="Paginator{T}"/> which provides <see cref="Page"/>'s.
    /// </summary>
    /// <typeparam name="T">The argument for which a paginator should be created.</typeparam>
    public class PaginatorBuilder<T>
    {
        private Func<T, PageFieldBuilder>? _valueFormatter;

        private string _cid = string.Empty;

        /// <summary>
        ///     Adds a pagebuilder to the builder, which provides formatting for pages to be created.
        /// </summary>
        /// <param name="fieldFormatter">The method in which the fields will be formatted.</param>
        /// <returns>The builder instance with a page builder included.</returns>
        public PaginatorBuilder<T> WithPages(Func<T, PageFieldBuilder> fieldFormatter)
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
        ///     Builds a paginator based on the values passed by previous calls to the paginatorbuilder.
        /// </summary>
        /// <returns>A paginator with a generic argument matching the argument passed in the paginatorbuilder.</returns>
        public Paginator<T> Build()
        {
            if (_valueFormatter is null)
                throw new InvalidOperationException("The value formatter is null. Please call 'WithPages' to correct this error.");

            if (_cid is null)
                throw new InvalidOperationException("The custom ID of a paginatorbuilder cannot be null.");

            return new Paginator<T>(_valueFormatter, _cid);
        }
    }
}
