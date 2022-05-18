using Discord;

namespace Barriot.Extensions.Pagination
{
    public struct Page
    {
        /// <summary>
        ///     The embed produced by this page.
        /// </summary>
        public EmbedBuilder Embed { get; set; }

        /// <summary>
        ///     The components produced by this page.
        /// </summary>
        public ComponentBuilder Component { get; set; }

        internal Page(EmbedBuilder embed, ComponentBuilder component)
        {
            Embed = embed;
            Component = component;
        }
    }
}
