namespace Barriot.Extensions.Files
{
    public sealed class FileData
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

        internal FileData(string[] fileEnties)
        {
            Lines = fileEnties;
            Index = _random.Next(0, Lines.Length);
            SelectedLine = Lines[Index];
        }
    }
}
