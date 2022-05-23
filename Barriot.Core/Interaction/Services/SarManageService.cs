namespace Barriot.Interaction.Services
{
    public class SarManageService
    {
        private readonly Dictionary<ulong, SarManageArgs> _dataCache;

        private readonly System.Timers.Timer _timer;

        public SarManageService()
        {
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

        public void CreateFromMessage(ulong messageId, RestUserMessage message)
        {
            if (_dataCache.ContainsKey(messageId))
                _dataCache[messageId] = new(message);
            else
                _dataCache.Add(messageId, new(message));
        }

        public bool TryGetData(ulong messageId, out RestUserMessage args)
        {
            args = null!;
            if (_dataCache.TryGetValue(messageId, out var data))
            {
                args = data.Message;
                return true;
            }
            else
                return false;
        }

        public bool ContainsMessage(ulong messageId)
            => _dataCache.ContainsKey(messageId);

        public bool TryRemoveData(ulong messageId)
            => _dataCache.Remove(messageId);

        public class SarManageArgs
        { 
            public DateTime CreationDate { get; }

            public RestUserMessage Message { get; }

            public SarManageArgs(RestUserMessage message)
            {
                CreationDate = DateTime.UtcNow;
                Message = message;
            }
        }
    }
}
