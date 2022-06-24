namespace Barriot.Application.Interactions.Attributes
{
    /// <summary>
    ///     Represents a precondition result that contains understandable information for the user.
    /// </summary>
    public sealed class BarriotPreconditionResult : PreconditionResult
    {
        /// <summary>
        ///     The reason why this precondition failed.
        /// </summary>
        public string? DisplayReason { get; }

        private BarriotPreconditionResult(InteractionCommandError? error, string reason, string? displayReason)
            : base(error, reason)
        {
            DisplayReason = displayReason;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="displayReason"></param>
        /// <returns></returns>
        public static BarriotPreconditionResult FromError(string reason, string? displayReason = null)
            => new(InteractionCommandError.UnmetPrecondition, reason, displayReason);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static new BarriotPreconditionResult FromSuccess()
            => new(null, "", null);
    }
}
