namespace Barriot.Caching
{
    /// <summary>
    ///     A render model that generates data about all guilds Barriot has access to.
    /// </summary>
    public class GuildCache
    {
        private DateTime _lastChecked;

        private IReadOnlyCollection<RestGuild>? _guilds;

        private readonly DiscordRestClient _client;

        private bool _inLoadingState = false;

        public GuildCache(DiscordRestClient client)
        {
            _client = client;
            _lastChecked = DateTime.UtcNow;
        }

        private async Task RunApiCallAsync()
        {
            _inLoadingState = true;
            _guilds = await _client.GetGuildsAsync();
            _inLoadingState = false;
        }

        // This call will dispose a call to the background handler to prevent the call taking far too long.
        private IReadOnlyCollection<RestGuild>? ReturnAndPopulate()
        {
            if (_inLoadingState)
                return _guilds;

            if (_guilds is null || DateTime.UtcNow.AddMinutes(15) <= _lastChecked)
            {
                _lastChecked = DateTime.UtcNow;
                _ = RunApiCallAsync();
            }
            return _guilds;
        }

        /// <summary>
        ///     Gets the current total amount of guilds.
        /// </summary>
        /// <returns>An integer representing the amount of guilds Barriot has access to.</returns>
        public int GetGuildCount()
            => ReturnAndPopulate()?.Count ?? 0;

        /// <summary>
        ///     Gets a collection of guilds Barriot has access to.
        /// </summary>
        /// <param name="filter">The filter to run through the collection.</param>
        /// <returns>A collection of guilds.</returns>
        public IEnumerable<RestGuild>? GetGuilds(Func<RestGuild, bool>? filter = null)
        {
            var range = ReturnAndPopulate();

            return (filter is not null)
                ? range?.Where(filter)
                : range ?? null;
        }
    }
}
