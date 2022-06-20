namespace Barriot.Application.Interactions.Attributes
{
    /// <summary>
    ///     This attribute makes sure that components on the source message of this interaction are disabled after successful execution.
    /// </summary>
    public sealed class DisableSourceAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if (context.Interaction is not RestMessageComponent messageComponent)
                return BarriotPreconditionResult.FromError(
                    reason: nameof(DisableSourceAttribute) + " is only supported for message components.");

            var builder = new ComponentBuilder();

            var rows = ComponentBuilder.FromMessage(messageComponent.Message).ActionRows;

            for (int i = 0; i < rows.Count; i++)
            {
                foreach (var component in rows[i].Components)
                {
                    switch (component)
                    {
                        case ButtonComponent button:
                            builder.WithButton(button.ToBuilder()
                                .WithDisabled(true), i);
                            break;
                        case SelectMenuComponent menu:
                            builder.WithSelectMenu(menu.ToBuilder()
                                .WithDisabled(true), i);
                            break;
                    }
                }
            }

            try
            {
                await messageComponent.Message.ModifyAsync(x => x.Components = builder.Build());
                return BarriotPreconditionResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return BarriotPreconditionResult.FromError(
                    reason: ex.Message + " at: " + nameof(DisableSourceAttribute));
            }
        }
    }
}
