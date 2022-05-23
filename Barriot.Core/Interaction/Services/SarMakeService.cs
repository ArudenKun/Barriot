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
            foreach(var kvp in _dataCache)
                if (kvp.Value is not null && DateTime.UtcNow.AddMinutes(15) <= kvp.Value.CreationDate)
                    _dataCache.Remove(kvp.Key);
        }

        public void CreateFromChannel(ulong userId, RestTextChannel channel)
        {
            if (_dataCache.ContainsKey(userId))
                _dataCache[userId] = new(channel);
            else
                _dataCache.Add(userId, new(channel));
        }

        public void CreateFromMessage(ulong userId, RestUserMessage message)
        {
            if (_dataCache.ContainsKey(userId))
                _dataCache[userId] = new(message);
            else
                _dataCache.Add(userId, new(message));
        }

        public void CreateFromManageCache(ulong userId, ulong messageId)
        {
            if (!_manageService.TryGetData(messageId, out var message))
                throw new InvalidOperationException();

            CreateFromMessage(userId, message);
        }

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

        public bool TryGetData(ulong userId, out SarCreationArgs? args)
        {
            if (_dataCache.TryGetValue(userId, out args))
                return true;
            else
                return false;
        }

        public bool ContainsUser(ulong userId)
            => _dataCache.ContainsKey(userId);

        public bool TryRemoveData(ulong userId)
            => _dataCache.Remove(userId);

        public class SarCreationArgs
        {
            public DateTime CreationDate { get; set; }

            public RestTextChannel Channel { get; set; }

            public RestUserMessage? Message { get; set; } = null!;

            public string Content { get; set; } = string.Empty;

            public bool FormatAsEmbed { get; set; } = false;

            internal SarCreationArgs(RestTextChannel channel)
            {
                Channel = channel;
                CreationDate = DateTime.UtcNow;
            }

            internal SarCreationArgs(RestUserMessage message)
            {
                Message = message;
                if (message.Channel is not RestTextChannel textChannel)
                    throw new InvalidOperationException();
                Channel = textChannel;
                CreationDate = DateTime.UtcNow; 
            }
        }
    }
}
