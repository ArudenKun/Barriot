namespace Barriot.Extensions.Files
{
    public static class FileHelper
    {
        /// <summary>
        ///     Gets the data from a file.
        /// </summary>
        /// <param name="fileName">The name of the file to get all data from.</param>
        /// <returns>A populated type of <see cref="FileData"/></returns>
        public static FileData GetDataFromFile(string fileName)
        {
            var data = File.ReadAllLines(Path.Combine("Files", fileName + ".txt"));
            return new(data);
        }

        private static readonly Dictionary<ErrorType, string[]> _errorData = new();

        /// <summary>
        ///     Gets an appended error string from the given <paramref name="error"/>
        /// </summary>
        /// <param name="error">The error type to get a file for.</param>
        /// <param name="parameter">The parameter this applies to.</param>
        /// <param name="seperator">The seperator applying to the format in which this result is returned.</param>
        /// <returns>A string matching the data from <paramref name="error"/></returns>
        public static string GetErrorFromFile(ErrorType error, string parameter = "", string seperator = "\n")
        {
            if (!_errorData.TryGetValue(error, out var data))
            {
                data = File.ReadAllLines(Path.Combine("Files", "Error", error + ".txt"));
                _errorData.Add(error, data);
            }

            if (!string.IsNullOrEmpty(parameter))
                data[0] = data[0].Replace(@"{parameter}", parameter);

            return string.Join(seperator, data);
        }

        private static readonly Dictionary<InfoType, string[]> _infoData = new();

        /// <summary>
        ///     Gets an appended info string from the given <paramref name="info"/>
        /// </summary>
        /// <param name="info">The information type to get all file data for.</param>
        /// <param name="seperator">The seperator applying to the format in which this result is returned.</param>
        /// <returns>A string matching the data from <paramref name="info"/></returns>
        public static string GetInfoFromFile(InfoType info, string seperator = "\n")
        {
            if (!_infoData.TryGetValue(info, out var data))
            {
                data = File.ReadAllLines(Path.Combine("Files", "Info", info + ".txt"));
                _infoData.Add(info, data);
            }
            return string.Join(seperator, data);
        }
    }
}
