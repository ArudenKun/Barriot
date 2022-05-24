using System.Diagnostics.CodeAnalysis;

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

        public static ResultFormat Deleting
            => new("wastebucket");

        public override string ToString()
            => _format;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is ResultFormat format && format._format == _format)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return _format.GetHashCode();
        }

        public static bool operator ==(ResultFormat left, ResultFormat right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResultFormat left, ResultFormat right)
        {
            return !(left == right);
        }
    }
}
