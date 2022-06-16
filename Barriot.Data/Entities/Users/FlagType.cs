namespace Barriot
{
    /// <summary>
    ///     Controlling enum for permissions in the Barriot system. Compare to this enum for permission checks or call <see cref="UserEntity.HasFlag(FlagType)"/>
    /// </summary>
    [Flags]
    public enum FlagType
    {
        /// <summary>
        ///     A custom flag.
        /// </summary>
        Custom,

        /// <summary>
        ///     The user is a top voter of last month.
        /// </summary>
        TopVoter,

        /// <summary>
        ///     The user is a contributor.
        /// </summary>
        Contributor,

        /// <summary>
        ///     A recognized developer of Barriot.
        /// </summary>
        Developer,

        /// <summary>
        ///     The user is member of the support team.
        /// </summary>
        Support,

        /// <summary>
        ///     The user is any tier of champion.
        /// </summary>
        Champion,

        /// <summary>
        ///     The user is any tier of button presser.
        /// </summary>
        Component,

        /// <summary>
        ///     The user is any tier of command executive.
        /// </summary>
        Command,

        /// <summary>
        ///     The user donated for Barriot development.
        /// </summary>
        Donator
    }
}
