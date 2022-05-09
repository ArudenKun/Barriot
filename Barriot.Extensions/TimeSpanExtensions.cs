using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Barriot.Extensions
{
    public static class TimeSpanExtensions
    {
        private static readonly Lazy<IReadOnlyDictionary<string, Func<string, TimeSpan>>> _callback = new(ValueFactory);

        private static readonly Regex _regex = new(@"(\d*)\s*([a-zA-Z]*)\s*(?:and|,)?\s*", RegexOptions.Compiled);

        private static IReadOnlyDictionary<string, Func<string, TimeSpan>> ValueFactory()
        {
            var callback = ImmutableDictionary.CreateBuilder<string, Func<string, TimeSpan>>();
            callback["second"] = Seconds;
            callback["seconds"] = Seconds;
            callback["sec"] = Seconds;
            callback["s"] = Seconds;
            callback["minute"] = Minutes;
            callback["minutes"] = Minutes;
            callback["min"] = Minutes;
            callback["m"] = Minutes;
            callback["hour"] = Hours;
            callback["hours"] = Hours;
            callback["h"] = Hours;
            callback["day"] = Days;
            callback["days"] = Days;
            callback["d"] = Days;
            callback["week"] = Weeks;
            callback["weeks"] = Weeks;
            callback["w"] = Weeks;
            callback["month"] = Months;
            callback["months"] = Months;
            return callback.ToImmutable();
        }

        private static TimeSpan Seconds(string match)
            => new(0, 0, int.Parse(match));

        private static TimeSpan Minutes(string match)
            => new(0, int.Parse(match), 0);

        private static TimeSpan Hours(string match)
            => new(int.Parse(match), 0, 0);

        private static TimeSpan Days(string match)
            => new(int.Parse(match), 0, 0, 0);

        private static TimeSpan Weeks(string match)
            => new((int.Parse(match) * 7), 0, 0, 0);

        private static TimeSpan Months(string match)
            => new((int.Parse(match) * 30), 0, 0, 0);

        /// <summary>
        ///     Turns a <see cref="TimeSpan"/> into a readable string.
        /// </summary>
        /// <param name="span">The current struct to turn into a readable string.</param>
        /// <returns>A readable string.</returns>
        public static string ToReadable(this TimeSpan span)
        {
            StringBuilder sb = new();
            if (span.Days > 0)
            {
                sb.Append($"{span.Days} day{(span.Days > 1 ? "s" : "")}");
                if (span.Hours > 0 && (span.Minutes > 0 || span.Seconds > 0))
                    sb.Append(", ");
                else if (span.Hours > 0)
                    sb.Append(" and ");
                else return sb.ToString();
            }
            if (span.Hours > 0)
            {
                sb.Append($"{span.Hours} hour{(span.Hours > 1 ? "s" : "")}");
                if (span.Minutes > 0 && span.Seconds > 0)
                    sb.Append(", ");
                else if (span.Minutes > 0)
                    sb.Append(" and ");
                else return sb.ToString();
            }
            if (span.Minutes > 0)
            {
                sb.Append($"{span.Minutes} minute{(span.Minutes > 1 ? "s" : "")}");
                if (span.Seconds > 0)
                    sb.Append(" and ");
                else return sb.ToString();
            }
            if (span.Seconds > 0)
                sb.Append($"{span.Seconds} second{(span.Seconds > 1 ? "s" : "")}");
            return sb.ToString();
        }

        /// <summary>
        ///     Gets a timespan based on its string counterpart, almost like a tryparse.
        /// </summary>
        /// <param name="span">The string to pass.</param>
        /// <returns>A timespan. <see cref="TimeSpan.Zero"/> if nothing was found.</returns>
        public static TimeSpan GetSpan(this string span)
        {
            if (!TimeSpan.TryParse(span, out TimeSpan timeSpan))
            {
                span = span.ToLower().Trim();
                MatchCollection matches = _regex.Matches(span);
                if (matches.Any())
                    foreach (Match match in matches)
                        if (_callback.Value.TryGetValue(match.Groups[2].Value, out var result))
                            timeSpan += result(match.Groups[1].Value);
            }
            return timeSpan;
        }
    }
}
