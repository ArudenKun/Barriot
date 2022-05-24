namespace Barriot
{
    public class TextBuilder
    {
        public ResultFormat Result { get; private set; } = ResultFormat.Success;

        public string Header { get; private set; }

        public string? Description { get; private set; } = null;

        public string? Context { get; private set; } = null;

        public TextBuilder()
        {
            Header = string.Empty;
        }

        public TextBuilder WithResult(ResultFormat format)
        {
            Result = format;
            return this;
        }

        public TextBuilder WithHeader(string header)
        {
            Header = header;
            return this;
        }

        public TextBuilder WithDescription(string? description)
        {
            Description = description;
            return this;
        }

        public TextBuilder WithContext(string? context)
        {
            Context = context;
            return this;
        }

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
                result += $"\n\n>{Description}";

            return result;
        }
    }
}
