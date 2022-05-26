using Barriot.Entities.Pins;
using Barriot.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Barriot.Models
{
    /// <summary>
    ///     Represents a message jump url.
    /// </summary>
    public readonly struct JumpUrl
    {
        /// <summary>
        ///     The url of this message reference.
        /// </summary>
        public string Url { get; }

        /// <summary>
        ///     The source of this url.
        /// </summary>
        public JumpUrlType Type { get; }

        /// <summary>
        ///     The Id of the channel this url jumps to.
        /// </summary>
        public ulong ChannelId { get; }

        /// <summary>
        ///     The Id of the message this url jumps to.
        /// </summary>
        public ulong MessageId { get; }

        public JumpUrl(string url, JumpUrlType type, ulong channelId, ulong messageId)
        {
            Url = url;
            Type = type;
            ChannelId = channelId;
            MessageId = messageId;
        }

        /// <summary>
        ///     Attempts to create a <see cref="JumpUrl"/> from the provided <paramref name="messageUrl"/>.
        /// </summary>
        /// <param name="messageUrl">The url to attempt to parse.</param>
        /// <param name="value">The returned value.</param>
        /// <returns><see langword="true"/> if succesfully parsed. <see langword="false"/> if not.</returns>
        public static bool TryParse(string messageUrl, out JumpUrl value)
        {
            value = new();

            if (!messageUrl.TryGetUrlData(out var data))
                return false;

            var type = (data[0] is 0ul) 
                ? JumpUrlType.DirectMessage 
                : JumpUrlType.GuildMessage;

            value = new(messageUrl, type, data[1], data[2]);

            return true;
        }

        /// <summary>
        ///     Creates a <see cref="JumpUrl"/> from the provided <paramref name="messageUrl"/>.
        /// </summary>
        /// <param name="messageUrl">The url to parse.</param>
        /// <returns>A new instance of <see cref="JumpUrl"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="messageUrl"/> is an invalid jump url.</exception>
        public static JumpUrl Parse(string messageUrl)
        {
            if (!messageUrl.TryGetUrlData(out var data))
                throw new ArgumentException("Provided argument is not a valid message url.", nameof(messageUrl));

            var type = (data[0] is 0ul)
                ? JumpUrlType.DirectMessage
                : JumpUrlType.GuildMessage;

            return new(messageUrl, type, data[1], data[2]);
        }

        public override string ToString()
            => Url;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is JumpUrl url && url.Url == Url)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }

        public static bool operator ==(JumpUrl left, JumpUrl right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(JumpUrl left, JumpUrl right)
        {
            return !(left == right);
        }
    }
}
