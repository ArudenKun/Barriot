using Barriot.Models.Services;

namespace Barriot.Interaction.Services
{
    public class SarMakeService
    {
        private readonly Dictionary<ulong, SarCreationArgs> _dataCache;

        private readonly System.Timers.Timer _timer;

        private readonly SarManageService _manageService;

        public SarMakeService(SarManageService service)
        {
            _manageService = service;

            _timer = new(900000)
            {
                AutoReset = true,
                Enabled = true,
            };
            _timer.Elapsed += ClearCache;
            _dataCache = new();
        }

        private void ClearCache(object? sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var kvp in _dataCache)
                if (kvp.Value is not null && DateTime.UtcNow.AddMinutes(15) <= kvp.Value.CreationDate)
                    _dataCache.Remove(kvp.Key);
        }

        /// <summary>
        ///     Creates a cache entity from a channel.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="channel"></param>
        public void CreateFromChannel(ulong userId, RestTextChannel channel)
        {
            if (_dataCache.ContainsKey(userId))
                _dataCache[userId] = new(channel);
            else
                _dataCache.Add(userId, new(channel));
        }

        /// <summary>
        ///     Creates a cache entity from a message.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public void CreateFromMessage(ulong userId, RestUserMessage message)
        {
            if (_dataCache.ContainsKey(userId))
                _dataCache[userId] = new(message);
            else
                _dataCache.Add(userId, new(message));
        }

        /// <summary>
        ///     Tries to claim a cache entity from the management service.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void CreateFromManageCache(ulong userId, ulong messageId)
        {
            if (!_manageService.TryGetData(messageId, out var message))
                throw new InvalidOperationException();

            CreateFromMessage(userId, message);
        }

        /// <summary>
        ///     Attempts to add data to an existing cache entity.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool TryAddData(ulong userId, Action<SarCreationArgs> action)
        {
            if (_dataCache.TryGetValue(userId, out var args))
            {
                action(args);
                _dataCache[userId] = args;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        ///     Attempts to get the current cache entity matching provided <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool TryGetData(ulong userId, out SarCreationArgs? args)
        {
            if (_dataCache.TryGetValue(userId, out args))
                return true;
            else
                return false;
        }

        /// <summary>
        ///     Checks if a user is contained in cache.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ContainsUser(ulong userId)
            => _dataCache.ContainsKey(userId);

        /// <summary>
        ///     Attempts to remove a cache entity matching provided <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool TryRemoveData(ulong userId)
            => _dataCache.Remove(userId);
    }
}
