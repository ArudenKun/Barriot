using System.Text.RegularExpressions;

namespace Barriot.Extensions
{
    public static class StringExtensions
    {
        private static readonly Lazy<Regex> _linkRegex = new(() => new(@"", RegexOptions.Compiled));

        /// <summary>
        ///     Reduces the length of the <paramref name="input"/> and appends the <paramref name="finalizer"/> to humanize the returned string.
        /// </summary>
        /// <remarks>
        ///     Returns the string unchanged if the length is less or equal to <paramref name="maxLength"/>.
        /// </remarks>
        /// <param name="input">The input string to reduce the length of.</param>
        /// <param name="maxLength">The max length the input string is allowed to be.</param>
        /// <param name="killAtWhitespace">Wether to kill the string at whitespace instead of cutting off at a word.</param>
        /// <param name="finalizer">The finalizer to humanize this string with.</param>
        /// <returns>The input string reduced to fit the length set by <paramref name="maxLength"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the length of maxlength is below 0 after the finalizer has been reduced from it.</exception>
        public static string Reduce(this string input, int maxLength, bool killAtWhitespace = false, string finalizer = "...")
        {
            if (input.Length > maxLength)
            {
                maxLength -= (finalizer.Length + 1); // reduce the length of the finalizer + a single integer to convert to valid range.

                if (maxLength < 1)
                    throw new ArgumentOutOfRangeException(nameof(maxLength));

                if (killAtWhitespace)
                {
                    var range = input.Split(' ');
                    for (int i = 2; input.Length + finalizer.Length > maxLength; i++) // set i as 2, 1 for index reduction, 1 for initial word removal, then increment.
                        input = string.Join(' ', range[..(range.Length - i)]);

                    input += finalizer;
                }
                return input[..maxLength] + finalizer;
            }
            else return input;
        }

        public static bool TryGetLinkData(this string? messageLink, out ulong[] data)
        {
            data = Array.Empty<ulong>();

            if (string.IsNullOrEmpty(messageLink))
                return false;

            var extraction = messageLink.Split('/');

            for (int i = 0; i < extraction.Length; i++)
            {
                if (ulong.TryParse(extraction[i], out data[i]))
                    continue;
            }

            if (data.Length == 3)
                return true;

            return false;
        }
    }
}
