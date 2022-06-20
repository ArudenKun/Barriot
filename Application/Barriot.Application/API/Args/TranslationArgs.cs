using Newtonsoft.Json;

namespace Barriot.Application.API.Args
{
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
