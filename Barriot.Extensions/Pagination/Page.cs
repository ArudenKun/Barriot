using Discord;

namespace Barriot.Extensions.Pagination
{
    public struct Page
    {
        /// <summary>
        ///     The embed produced by this page.
        /// </summary>
        public Embed Embed { get; set; }

        /// <summary>
        ///     The components produced by this page.
        /// </summary>
        public MessageComponent Component { get; set; }

        internal Page(Embed embed, MessageComponent component)
        {
            Embed = embed;
            Component = component;
        }
    }
}
