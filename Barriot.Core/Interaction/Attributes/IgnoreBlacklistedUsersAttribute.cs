namespace Barriot.Interaction.Attributes
{
    public class IgnoreBlacklistedUsersAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            await Task.CompletedTask;

            var validContext = (context as BarriotInteractionContext)!;

            if (validContext.UserData.IsBlacklisted)
            {
                return BarriotPreconditionResult.FromError(
                    reason: "User is blacklisted and cannot enter command execution.",
                    displayReason: "**You are blacklisted!** This means you are unable to interact with Barriot in any way.");
            }
            return BarriotPreconditionResult.FromSuccess();
        }
    }
}
