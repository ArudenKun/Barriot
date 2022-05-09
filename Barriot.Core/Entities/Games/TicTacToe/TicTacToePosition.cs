namespace Barriot.Entities.TicTacToe
{
    public class TicTacToePosition
    {
        /// <summary>
        ///     The X coordinate of this position.
        /// </summary>
        public short X { get; set; }

        /// <summary>
        ///     The Y coordinate of this position.
        /// </summary>
        public short Y { get; set; }

        /// <summary>
        ///     The display icon.
        /// </summary>
        public string Icon { get; set; } = "⚫";

        /// <summary>
        ///     The style.
        /// </summary>
        public ButtonStyle Style { get; set; } = ButtonStyle.Secondary;

        /// <summary>
        ///     By who this icon was set.
        /// </summary>
        public ulong SetBy { get; set; } = 0;

        /// <summary>
        ///     Modifies the current position.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The current modified position.</returns>
        public TicTacToePosition Modify(Action<TicTacToePosition> action)
        {
            action(this);
            return this;
        }

        /// <summary>
        ///     Creates a position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The base position.</returns>
        public static TicTacToePosition Build(short x, short y)
            => new() { X = x, Y = y };
    }
}
