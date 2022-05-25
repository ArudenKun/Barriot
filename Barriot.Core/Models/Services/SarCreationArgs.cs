namespace Barriot.Models.Services
{
    /// <summary>
    ///     A class responsible for storing data on new self-assign role creation.
    /// </summary>
    public sealed class SarCreationArgs
    {
        /// <summary>
        ///     Gets or sets the creation date of this instance.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     Gets or sets the channel of this instance.
        /// </summary>
        public RestTextChannel Channel { get; set; }

        /// <summary>
        ///     Gets or sets the message of this instance.
        /// </summary>
        public RestUserMessage? Message { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the content of this instance.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets if the result should be formatted as embed.
        /// </summary>
        public bool FormatAsEmbed { get; set; } = false;

        /// <summary>
        ///     Creates a new instance of <see cref="SarCreationArgs"/> from provided <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel"></param>
        internal SarCreationArgs(RestTextChannel channel)
        {
            Channel = channel;
            CreationDate = DateTime.UtcNow;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SarCreationArgs"/> from provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException">Thrown if the channel is not a text channel.</exception>
        internal SarCreationArgs(RestUserMessage message)
        {
            Message = message;
            if (message.Channel is not RestTextChannel textChannel)
                throw new InvalidOperationException();
            Channel = textChannel;
            CreationDate = DateTime.UtcNow;
        }
    }
}
