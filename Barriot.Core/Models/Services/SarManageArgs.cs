namespace Barriot.Models.Services
{
    /// <summary>
    ///     A class responsible as arguments for managing an existing self-assign role.
    /// </summary>
    public sealed class SarManageArgs
    {
        /// <summary>
        ///     Gets or sets the creation date of this instance.
        /// </summary>
        public DateTime CreationDate { get; }

        /// <summary>
        ///     Gets or sets the message of this instance.
        /// </summary>
        public RestUserMessage Message { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="SarManageArgs"/> from the provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public SarManageArgs(RestUserMessage message)
        {
            CreationDate = DateTime.UtcNow;
            Message = message;
        }
    }
}
