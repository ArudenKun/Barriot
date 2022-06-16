namespace Barriot.Interactions.Services
{
    public class InfoService
    {
        private int _guildCount;

        private DateTime _lastChecked;

        private readonly DiscordRestClient _client;

        /// <summary>
        ///     Controls when the client has started (UTC)
        /// </summary>
        public DateTime OnlineSince { get; set; }

        /// <summary>
        ///     Gets the current total guild count.
        /// </summary>
        public int GuildCount
        {
            get
            {
                if (_guildCount is 0 || DateTime.UtcNow.AddMinutes(15) <= _lastChecked)
                {
                    _lastChecked = DateTime.UtcNow;
                    _ = RunApiCallAsync();
                    return 0;
                }
                return _guildCount;
            }
        }

        public InfoService(DiscordRestClient client)
        {
            OnlineSince = DateTime.UtcNow;

            _client = client;
            _lastChecked = DateTime.UtcNow;
        }

        private async Task RunApiCallAsync()
        {
            _guildCount = (await _client.GetGuildsAsync()).Count;
        }
    }
}
