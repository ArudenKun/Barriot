namespace Barriot.Extensions.Files
{
    public enum ErrorType
    {
        /// <summary>
        ///     An invalid timespan.
        /// </summary>
        InvalidTimeSpan,

        /// <summary>
        ///     The input of a certain string is too long.
        /// </summary>
        InputTooLong,

        /// <summary>
        ///     Sending reminders cannot be done because user disabled DM.
        /// </summary>
        ReminderSendFailed,
    }
}
