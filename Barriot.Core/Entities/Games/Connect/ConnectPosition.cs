namespace Barriot.Entities.Connect
{
    public class ConnectPosition
    {
        /// <summary>
        ///     The X position.
        /// </summary>
        public short X { get; set; }

        /// <summary>
        ///     The Y position.
        /// </summary>
        public short Y { get; set; }

        /// <summary>
        ///     The player who set this position.
        /// </summary>
        public ulong SetBy { get; set; } = 0;

        /// <summary>
        ///     The emoji for this position.
        /// </summary>
        public string Emoji { get; set; } = ":black_large_square:";

        /// <summary>
        ///     Modifies a position with granted values.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The same position with modified values.</returns>
        public ConnectPosition Modify(Action<ConnectPosition> action)
        {
            action(this);
            return this;
        }

        /// <summary>
        ///     Creates a position with granted values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A new position with base values.</returns>
        public static ConnectPosition Build(short x, short y)
            => new() { X = x, Y = y };
    }
}
