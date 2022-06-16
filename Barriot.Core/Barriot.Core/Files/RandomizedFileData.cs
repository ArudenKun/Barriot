namespace Barriot.Models.Files
{
    /// <summary>
    ///     Represents data fetched from a file with a randomizer to control the value returned.
    /// </summary>
    public sealed class RandomizedFileData
    {
        private static readonly Random _random = new();

        /// <summary>
        ///     All lines in this file.
        /// </summary>
        public string[] Lines;

        /// <summary>
        ///     The selected line.
        /// </summary>
        public string SelectedLine;

        /// <summary>
        ///     The index of the selected line.
        /// </summary>
        public int Index;

        internal RandomizedFileData(string[] fileEnties)
        {
            Lines = fileEnties;
            Index = _random.Next(0, Lines.Length);
            SelectedLine = Lines[Index];
        }
    }
}
