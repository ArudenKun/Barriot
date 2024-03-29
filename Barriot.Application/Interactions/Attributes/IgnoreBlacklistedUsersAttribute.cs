﻿namespace Barriot.Application.Interactions.Attributes
{
    /// <summary>
    ///     An attribute that checks if the user in question is blacklisted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class IgnoreBlacklistedUsersAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            await Task.CompletedTask;

            var validContext = (context as BarriotInteractionContext)!;

            if (validContext.Member.IsBlacklisted)
            {
                return BarriotPreconditionResult.FromError(
                    reason: "User is blacklisted and cannot enter command execution.",
                    displayReason: "**You are blacklisted!** This means you are unable to interact with Barriot in any way.");
            }
            return BarriotPreconditionResult.FromSuccess();
        }
    }
}
