namespace Barriot.Interactions.Modals.Evaluation
{
    internal sealed class EvaluationArgs
    {
        /// <summary>
        ///     Contains the guild, channel, client and user of the interaction.
        /// </summary>
        public BarriotInteractionContext Context { get; }

        /// <summary>
        ///     Gets or sets a text message to the result.
        /// </summary>
        public string? Result { get; private set; } = null;

        /// <summary>
        ///     Gets or sets an attachment url to the result.
        /// </summary>
        public string? Attachment { get; private set; } = null;

        /// <summary>
        ///     Creates a new instance of evaluation context.
        /// </summary>
        /// <param name="context"></param>
        public EvaluationArgs(BarriotInteractionContext context)
            => Context = context;

        /// <summary>
        ///     Prints a new 
        /// </summary>
        /// <param name="result"></param>
        public void Print(object result)
            => Result = result.ToString();

        public void Attach(object attachment)
            => Attachment = attachment.ToString();
    }
}
