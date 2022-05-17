using Barriot.API.Translation;

namespace Barriot.Caching
{
    public class TranslationCache
    {
        private readonly ITranslateClient _client;

        private List<IEnumerable<LanguageData>>? _languages;

        private DateTime _lastCheck;

        private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public TranslationCache(ITranslateClient client)
        {
            _lastCheck = DateTime.UtcNow;
            _client = client;
        }

        public async Task<List<IEnumerable<LanguageData>>> GetAllLanguagesAsync()
        {
            if (_languages is null || _lastCheck > DateTime.UtcNow.AddDays(1))
            {
                _lastCheck = DateTime.UtcNow;

                var languages = await _client.GetSupportedLanguagesAsync();

                List<IEnumerable<LanguageData>> data = new();

                var cursor = _alphabet.Length / 2;
                for (int i = 0; i < _alphabet.Length; i += cursor)
                    data.Add(languages.OrderBy(x => x.Name[0]).ToList().GetRange(i, cursor));

                _languages = data;

                return data;
            }
            else
                return _languages;
        }
    }
}
