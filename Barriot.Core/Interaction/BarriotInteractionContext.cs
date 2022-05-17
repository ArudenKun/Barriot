namespace Barriot.Interaction
{
    /// <summary>
    ///     The context for a Barriot interaction, including the user entity that invoked it.
    /// </summary>
    public sealed class BarriotInteractionContext : RestInteractionContext
    {
        /// <summary>
        ///     The Barriot user entity for this interaction.
        /// </summary>
        public UserEntity Member { get; }

        /// <summary>
        ///     If the user in context has won a game. Only applies to challenges.
        /// </summary>
        public bool WonGameInSession { get; set; } = false;

        internal BarriotInteractionContext(DiscordRestClient client, RestInteraction interaction, Func<string, Task> responseCallback)
            : base(client, interaction, responseCallback)
        {
            Member = UserEntity.GetAsync(User)
                .GetAwaiter()
                .GetResult();
        }
    }
}
