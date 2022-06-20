using Barriot.Application.API.Args;

namespace Barriot.Application.API
{
    public interface ITranslateClient
    {
        /// <summary>
        ///     Gets all supported languages in the current API version.
        /// </summary>
        /// <returns></returns>
        Task<List<LanguageData>> GetSupportedLanguagesAsync();

        /// <summary>
        ///     Translates the given input to a translated string.
        /// </summary>
        /// <param name="action">The post to translate.</param>
        /// <returns></returns>
        Task<string> TranslateAsync(Action<TranslationRequest> action);
    }
}
