﻿namespace Barriot.Application.Interactions.Attributes
{
    /// <summary>
    ///     This attribute makes sure that the user who pressed this button is the only one who can execute it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class DoUserCheckAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            await Task.CompletedTask;

            if (context.Interaction is not RestMessageComponent component)
                return BarriotPreconditionResult.FromError(
                    reason: nameof(DoUserCheckAttribute) + " is only supported for message components.");

            else
            {
                var param = component.Data.CustomId.Split(':');

                if (param.Length > 1 && ulong.TryParse(param[1].Split(',')[0], out ulong id))
                {
                    if (context.User.Id != id)
                    {
                        return BarriotPreconditionResult.FromError(
                            reason: "Context user cannot operate this component.",
                            displayReason: "**You can't interact with this component!** It is intended for someone else.");
                    }
                    else
                        return BarriotPreconditionResult.FromSuccess();
                }
                else
                    return BarriotPreconditionResult.FromError(
                        reason: "Parse cannot be done if no user ID exists.");
            }
        }
    }
}
