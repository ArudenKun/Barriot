namespace Barriot.Interaction.Services
{
    public class InfoService
    {
        public DateTime OnlineSince { get; set; }

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

        private int _guildCount;

        private DateTime _lastChecked;

        private readonly DiscordRestClient _client;

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
