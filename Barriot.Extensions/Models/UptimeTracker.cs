namespace Barriot.Models
{
    /// <summary>
    ///     The uptime retention class.
    /// </summary>
    public class UptimeTracker
    {
        /// <summary>
        ///     The time when this application was started.
        /// </summary>
        public DateTime StartTime { get; set; }

        public UptimeTracker()
            => StartTime = DateTime.UtcNow;

        /// <summary>
        ///     Gets a string of the timespan between now and the time at which this app was started.
        /// </summary>
        /// <returns>
        ///     The total application uptime in string.
        /// </returns>
        public override string ToString()
            => $"{(DateTime.UtcNow - StartTime)}";
    }
}
