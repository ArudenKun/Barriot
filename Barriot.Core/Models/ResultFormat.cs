using System.Diagnostics.CodeAnalysis;

namespace Barriot
{
    /// <summary>
    ///     Represents the emoji format in which the returned Discord string will be started with.
    /// </summary>
    public readonly struct ResultFormat
    {
        private readonly string _format;

        /// <summary>
        ///     Creates a new <see cref="ResultFormat"/> from the input string.
        /// </summary>
        /// <param name="format"></param>
        public ResultFormat(string format)
            => _format = format;

        /// <summary>
        ///     Format a success result.
        /// </summary>
        public static ResultFormat Success
            => new("white_check_mark");

        /// <summary>
        ///     Format a failure result.
        /// </summary>
        public static ResultFormat Failure
            => new("x");

        /// <summary>
        ///     Format a not-allowed result.
        /// </summary>
        public static ResultFormat NotAllowed
            => new("no_entry_sign");

        /// <summary>
        ///     Format a list result.
        /// </summary>
        public static ResultFormat List
            => new("books");

        /// <summary>
        ///     Format a question result.
        /// </summary>
        public static ResultFormat Question
            => new("question");

        /// <summary>
        ///     Format an important result.
        /// </summary>
        public static ResultFormat Important
            => new("exclamation");

        /// <summary>
        ///     Format a warning result.
        /// </summary>
        public static ResultFormat Warning
            => new("warning");

        /// <summary>
        ///     Format a deletion result.
        /// </summary>
        public static ResultFormat Deleting
            => new("wastebasket");

        /// <summary>
        ///     Returns the internal format of this <see cref="ResultFormat"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{_format}";

        /// <summary>
        ///     Compares the underlying value of this <see cref="ResultFormat"/> to another.
        /// </summary>
        /// <param name="obj">The other entity to compare to.</param>
        /// <returns></returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is ResultFormat format && format._format == _format)
                return true;
            return false;
        }

        /// <summary>
        ///     Gets the hashcode of underlying <see cref="ResultFormat"/> to a comparable result.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _format.GetHashCode();
        }

        /// <summary>
        ///     Calls the internal equality comparer to the left and right entities.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ResultFormat left, ResultFormat right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Calls the internal equality comparer to the left and right entities.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ResultFormat left, ResultFormat right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Creates a new <see cref="ResultFormat"/> based on the input string.
        /// </summary>
        /// <param name="input"></param>
        public static implicit operator ResultFormat(string input)
            => new(input);
    }
}
