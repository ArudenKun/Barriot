namespace Barriot.Extensions
{
    /// <summary>
    ///     A class that adds a set of extension methods for moderation purposes.
    /// </summary>
    public static class ModerationExtensions
    {
        public static string FormatLogReason(string issuer, string? reason = null)
        {
            var logReason = $"Requested by: {issuer}";

            if (!string.IsNullOrEmpty(reason))
            {
                logReason += (", for: " + reason);
                if (logReason.Length > 512) // max audit log length;
                    logReason = logReason[..508] + "...";
            }
            else
                logReason += ", no reason provided.";

            return logReason;
        }

        public static string FormatPartialResponse(string? reason, string query)
        {
            return string.IsNullOrEmpty(reason)
                    ? $"This user has been {query} with no reason provided."
                    : $"\n\n > **Reason:** {reason}";
        }
    }
}
