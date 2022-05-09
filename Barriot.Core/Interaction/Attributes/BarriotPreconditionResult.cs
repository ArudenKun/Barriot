namespace Barriot.Interaction.Attributes
{
    public class BarriotPreconditionResult : PreconditionResult
    {
        /// <summary>
        ///     The reason why this precondition failed.
        /// </summary>
        public string? DisplayReason { get; }

        protected BarriotPreconditionResult(InteractionCommandError? error, string reason, string? displayReason)
            : base(error, reason)
        {
            DisplayReason = displayReason;
        }

        public static BarriotPreconditionResult FromError(string reason, string? displayReason = null)
            => new(InteractionCommandError.UnmetPrecondition, reason, displayReason);

        public static new BarriotPreconditionResult FromSuccess()
            => new(null, "", null);
    }
}
