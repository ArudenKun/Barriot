namespace Barriot.Pagination
{
    public readonly struct Page
    {
        /// <summary>
        ///     The embed produced by this page.
        /// </summary>
        public EmbedBuilder Embed { get; }

        /// <summary>
        ///     The components produced by this page.
        /// </summary>
        public ComponentBuilder Component { get; }

        internal Page(EmbedBuilder embed, ComponentBuilder component)
        {
            Embed = embed;
            Component = component;
        }
    }
}
