using Barriot.Models.Files;

namespace Barriot.Extensions
{
    internal static class FileExtensions
    {
        /// <summary>
        ///     Gets the data from a file.
        /// </summary>
        /// <param name="fileName">The name of the file to get all data from.</param>
        /// <returns>A populated type of <see cref="RandomizedFileData"/></returns>
        public static RandomizedFileData GetDataFromFile(string fileName)
        {
            var data = File.ReadAllLines(Path.Combine("Files", fileName + ".txt"));
            return new(data);
        }

        private static readonly Dictionary<ErrorInfo, string[]> _errorData = new();

        /// <summary>
        ///     Gets an appended error string from the given <paramref name="error"/>
        /// </summary>
        /// <param name="error">The error type to get a file for.</param>
        /// <param name="parameter">The parameter this applies to.</param>
        /// <param name="seperator">The seperator applying to the format in which this result is returned.</param>
        /// <returns>A string matching the data from <paramref name="error"/></returns>
        public static string GetError(ErrorInfo error, string parameter = "")
        {
            if (!_errorData.TryGetValue(error, out var data))
            {
                data = File.ReadAllLines(Path.Combine("Files", "Error", error + ".txt"));
                _errorData.Add(error, data);
            }

            if (!string.IsNullOrEmpty(parameter))
                data[0] = data[0].Replace(@"{parameter}", parameter);

            var tb = new TextBuilder()
                .WithResult(MessageFormat.Failure)
                .WithHeader(data[0])
                .WithContext(data[1])
                .WithDescription(string.Join("\n", data[2..]));

            return tb.Build();
        }

        private static readonly Dictionary<EmbedInfo, string[]> _embedData = new();

        /// <summary>
        ///     Gets an appended info string from the given <paramref name="info"/>
        /// </summary>
        /// <param name="info">The information type to get all file data for.</param>
        /// <returns>A string matching the data from <paramref name="info"/></returns>
        public static string GetEmbedContent(EmbedInfo info)
        {
            if (!_embedData.TryGetValue(info, out var data))
            {
                data = File.ReadAllLines(Path.Combine("Files", "Info", info + ".txt"));
                _embedData.Add(info, data);
            }
            return string.Join("\n", data);
        }
    }
}
