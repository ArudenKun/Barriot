namespace Barriot
{
    /// <summary>
    ///     An interface to control several games at once.
    /// </summary>
    public interface IGameEntity : IMutableEntity
    {
        /// <summary>
        ///     When this game expires.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        ///     The players participating in this game.
        /// </summary>
        public ulong[] Players { get; set; }

        /// <summary>
        ///     The max X of the board.
        /// </summary>
        public short MaxX { get; set; }

        /// <summary>
        ///     The max Y of the board.
        /// </summary>
        public short MaxY { get; set; }
    }
}
