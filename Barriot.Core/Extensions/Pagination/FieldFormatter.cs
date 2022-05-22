namespace Barriot.Extensions.Pagination
{
    public class FieldFormatter
    {
        /// <summary>
        ///     The title of the field in this formatter.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The value of the field in this formatter.
        /// </summary>
        public string Value { get; set; }

        public bool DoInline { get; set; }

        /// <summary>
        ///     Creates a new field formatter for embed pagination.
        /// </summary>
        /// <param name="title">The title of the embed field.</param>
        /// <param name="value">The value of the embed field.</param>
        /// <param name="doInline">Wether to format the field inline or not.</param>
        public FieldFormatter(string title, string value, bool doInline = false)
        {
            Title = title;
            Value = value;
            DoInline = doInline;
        }
    }
}
