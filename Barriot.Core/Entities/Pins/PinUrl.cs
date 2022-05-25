using Barriot.Entities.Pins;
using Barriot.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Barriot
{
    /// <summary>
    ///     Represents a message jump url.
    /// </summary>
    public record PinUrl
    {
        /// <summary>
        ///     The url of this message reference.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     The source of this url.
        /// </summary>
        public PinUrlType Type { get; set; }

        /// <summary>
        ///     The Id of the channel this url jumps to.
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        ///     The Id of the message this url jumps to.
        /// </summary>
        public ulong MessageId { get; set; }

        public PinUrl(string url, PinUrlType type, ulong channelId, ulong messageId)
        {
            Url = url;
            Type = type;
            ChannelId = channelId;
            MessageId = messageId;
        }

        /// <summary>
        ///     Attempts to create a <see cref="PinUrl"/> from the provided <paramref name="messageUrl"/>.
        /// </summary>
        /// <param name="messageUrl">The url to attempt to parse.</param>
        /// <param name="value">The returned value.</param>
        /// <returns><see langword="true"/> if succesfully parsed. <see langword="false"/> if not.</returns>
        public static bool TryParse(string messageUrl, out PinUrl? value)
        {
            value = null!;

            if (!StringExtensions.TryGetUrlData(messageUrl, out var data))
                return false;

            var type = (data[0] is 0ul) 
                ? PinUrlType.DirectMessage 
                : PinUrlType.GuildMessage;

            value = new(messageUrl, type, data[1], data[2]);

            return true;
        }

        /// <summary>
        ///     Creates a <see cref="PinUrl"/> from the provided <paramref name="messageUrl"/>.
        /// </summary>
        /// <param name="messageUrl">The url to parse.</param>
        /// <returns>A new instance of <see cref="PinUrl"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="messageUrl"/> is an invalid jump url.</exception>
        public static PinUrl Parse(string messageUrl)
        {
            if (!StringExtensions.TryGetUrlData(messageUrl, out var data))
                throw new ArgumentException("Provided argument is not a valid message url.", nameof(messageUrl));

            var type = (data[0] is 0ul)
                ? PinUrlType.DirectMessage
                : PinUrlType.GuildMessage;

            return new(messageUrl, type, data[1], data[2]);
        }

        public override string ToString()
            => Url;
    }
}
