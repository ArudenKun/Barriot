namespace Barriot.Models.Files
{
    /// <summary>
    ///     Control for what error should be fetched from a file.
    /// </summary>
    public enum ErrorInfo
    {
        /// <summary>
        ///     An invalid timespan.
        /// </summary>
        InvalidTimeSpan,

        /// <summary>
        ///     Sending reminders cannot be done because user disabled DM.
        /// </summary>
        ReminderSendFailed,

        /// <summary>
        ///     Self-assign role creation/modification context took too long.
        /// </summary>
        SARContextAbandoned,
    }
}
