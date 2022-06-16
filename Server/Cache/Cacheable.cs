namespace Server
{
    internal static class Cacheable
    {

        private static ITextChannel? _voteChannel;
        /// <summary>
        ///     Gets the vote channel from its ulong value.
        /// </summary>
        /// <returns></returns>
        public static ITextChannel GetVoteChannel(this DiscordSocketClient client)
            => _voteChannel ??= client.Guilds.Select(x => x.Channels.First(x => x.Id == Config.Settings.VoteChannel)).FirstOrDefault() as ITextChannel
            ?? throw new ArgumentNullException("Invalid channel", nameof(Config.Settings.VoteRole));

        //private static IRole? _voteRole;
        ///// <summary>
        /////     Gets the vote role from its ulong value.
        ///// </summary>
        ///// <returns></returns>
        //public static IRole GetVoteRole(this DiscordSocketClient client)
        //    => _voteRole ??= client.Guilds.Select(x => x.Roles.First(x => x.Id == Config.Settings.VoteRole)).FirstOrDefault()
        //        ?? throw new ArgumentNullException("Invalid role", nameof(Config.Settings.VoteRole));
    }
}
