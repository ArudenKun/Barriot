using Newtonsoft.Json;

namespace Barriot.API.Translation
{
    public class TranslateClient : ITranslateClient
    {
        private readonly HttpClient _httpClient;

        public TranslateClient(HttpClient client)
            => _httpClient = client;

        /// <inheritdoc/>
        public async Task<List<LanguageData>> GetSupportedLanguagesAsync()
            => JsonConvert.DeserializeObject<List<LanguageData>>(await _httpClient.GetStringAsync("/languages"))
            ?? new();

        /// <inheritdoc/>
        public async Task<string> TranslateAsync(Action<TranslationRequest> action)
        {
            var post = new TranslationRequest();
            action(post);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "q", post.Text },
                { "source", post.Source.ToString() },
                { "target", post.Target.ToString() },
                { "api_key", post.ApiKey }
            });
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/translate")
            {
                Content = content
            });
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<Translation>(await response.Content.ReadAsStringAsync());
                return result?.TranslatedText
                    ?? string.Empty;
            }
            return string.Empty;
        }
    }

    public class Translation
    {
        [JsonProperty("translatedText")]
        public string TranslatedText { get; set; } = "";
    }

    public class TranslationRequest
    {
        [JsonProperty("q")]
        public string Text { get; set; } = "";

        [JsonProperty("source")]
        public string Source { get; set; } = "";

        [JsonProperty("target")]
        public string Target { get; set; } = "";

        [JsonProperty("api_key")]
        public string ApiKey { get; set; } = "";
    }

    public class LanguageData
    {
        [JsonProperty("code")]
        public string Code { get; set; } = "";

        [JsonProperty("name")]
        public string Name { get; set; } = "";
    }
}
