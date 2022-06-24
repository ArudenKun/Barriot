namespace Barriot.Pagination
{
    /// <summary>
    ///     Represents a builder for pagination fields.
    /// </summary>
    public class PageFieldBuilder
    {
        /// <summary>
        ///     Gets or sets the title of the field in this formatter.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the value of the field in this formatter.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets if the fields should be inlined.
        /// </summary>
        public bool DoInline { get; set; }

        /// <summary>
        ///     Creates a new field formatter for embed pagination.
        /// </summary>
        /// <param name="title">The title of the embed field.</param>
        /// <param name="value">The value of the embed field.</param>
        /// <param name="doInline">Wether to format the field inline or not.</param>
        public PageFieldBuilder(string title, string value, bool doInline = false)
        {
            Title = title;
            Value = value;
            DoInline = doInline;
        }

        /// <summary>
        ///     Creates a new field formatter for embed pagination.
        /// </summary>
        public PageFieldBuilder()
        {
            Title = string.Empty;
            Value = string.Empty;
            DoInline = false;
        }

        /// <summary>
        ///     Sets the <see cref="DoInline"/> value.
        /// </summary>
        /// <param name="doInline"></param>
        /// <returns></returns>
        public PageFieldBuilder WithInline(bool doInline)
        {
            DoInline = doInline;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="Title"/> value.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public PageFieldBuilder WithTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Value cannot be null or empty.", nameof(title));

            Title = title;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="Value"/> value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public PageFieldBuilder WithValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));

            Value = value;
            return this;
        }
    }
}
