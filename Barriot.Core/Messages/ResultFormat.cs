namespace Barriot
{
    public readonly struct ResultFormat
    {
        private readonly string _format;

        public ResultFormat(string format)
            => _format = format;

        public static ResultFormat Success
            => new("white_check_mark");

        public static ResultFormat Failure
            => new("x");

        public static ResultFormat NotAllowed
            => new("no_entry_sign");

        public static ResultFormat List
            => new("books");

        public static ResultFormat Question
            => new("question");

        public static ResultFormat Important
            => new("exclamation");

        public static ResultFormat Warning
            => new("warning");

        public override string ToString()
            => _format;
    }
}
