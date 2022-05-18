using System.Linq;

namespace Barriot.Caching
{
    public class ChannelCache
    {
        private readonly Dictionary<ulong, GuildChannelCache> _channels;

        public ChannelCache()
        {
            _channels = new();
        }

        private class GuildChannelCache
        {
            public DateTime LastChecked { get; set; }

            public IReadOnlyCollection<RestChannel>? Channels { get; set; } = null;

            public GuildChannelCache()
            {
                LastChecked = DateTime.UtcNow;
            }

            public async Task<IEnumerable<RestChannel>> GetChannelsAsync(RestGuild guild)
            {
                if (Channels is null || DateTime.UtcNow.AddMinutes(15) <= LastChecked)
                {
                    Channels = await guild.GetChannelsAsync();
                    LastChecked = DateTime.UtcNow;
                }
                return Channels;
            }
        }

        public async Task<IEnumerable<RestChannel>> GetChannelsAsync(RestGuild guild, Func<RestChannel, bool> filter)
        {
            IEnumerable<RestChannel> channels;
            if (_channels.TryGetValue(guild.Id, out var cache))
            {
                channels = await cache.GetChannelsAsync(guild);
            }
            else
            {
                _channels.Add(guild.Id, new());
                if (_channels.TryGetValue(guild.Id, out cache))
                {
                    channels = await cache.GetChannelsAsync(guild);
                }
                else
                    throw new InvalidOperationException();
            }

            return (filter is not null)
                ? channels.Where(filter)
                : channels ?? Enumerable.Empty<RestChannel>();
        }
    }
}
