namespace Barriot
{
    /// <summary>
    ///     Represents a builder for Barriot response messages.
    /// </summary>
    public class TextBuilder
    {
        /// <summary>
        ///     Gets or sets the emoji sent at response base.
        /// </summary>
        public ResultFormat Result { get; set; }

        /// <summary>
        ///     Gets or sets the response header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        ///     Gets or sets the response description. <see langword="null"/> if not set.
        /// </summary>
        public string? Description { get; set; } = null;

        /// <summary>
        ///     Gets or sets the response context. <see langword="null"/> if not set.
        /// </summary>
        public string? Context { get; set; } = null;

        /// <summary>
        ///     Creates a new instance of <see cref="TextBuilder"/> with predefined values.
        /// </summary>
        public TextBuilder()
        {
            Header = string.Empty;
            Result = ResultFormat.Success;
        }

        /// <summary>
        ///     Sets the <see cref="Result"/> value.
        /// </summary>
        /// <param name="format">Thanks to an implicit string operand, this parameter will take string.</param>
        /// <returns></returns>
        public TextBuilder WithResult(ResultFormat format)
        {
            Result = format;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="Header"/> value.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public TextBuilder WithHeader(string header)
        {
            Header = header;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="Description"/> value.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public TextBuilder WithDescription(string? description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="Context"/> value.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public TextBuilder WithContext(string? context)
        {
            Context = context;
            return this;
        }

        /// <summary>
        ///     Builds a string from the values of this <see cref="TextBuilder"/>.
        /// </summary>
        /// <returns>A formatted string to send to Discord.</returns>
        /// <exception cref="ArgumentNullException">Thrown if result or header is <see langword="null"/>.</exception>
        public string Build()
        {
            if (string.IsNullOrEmpty(Result.ToString()))
                throw new ArgumentNullException(nameof(Result));
            if (string.IsNullOrEmpty(Header))
                throw new ArgumentNullException(nameof(Header));

            var result = $":{Result}: **{Header}**";

            if (!string.IsNullOrEmpty(Context))
                result += $" *{Context}*";
            if (!string.IsNullOrEmpty(Description))
                result += $"\n\n> {Description}";

            return result;
        }
    }
}
