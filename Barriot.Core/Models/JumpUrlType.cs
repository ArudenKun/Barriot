namespace Barriot.Models
{
    /// <summary>
    ///     The jump Url source.
    /// </summary>
    public enum JumpUrlType : int
    {
        /// <summary>
        ///     The Url sources from a direct message.
        /// </summary>
        DirectMessage = 0,

        /// <summary>
        ///     The Url sources from a guild message.
        /// </summary>
        GuildMessage = 1
    }
}
