namespace Barriot.Interaction.Modules.SelfRoles
{
    public class SarRemoveModule
    {
        [ComponentInteraction("sar-message-removing")]
        public async Task RemovingMessageAsync()
        {
            await Task.CompletedTask;
        }

        [ComponentInteraction("sar-removed-message:*")]
        public async Task RemovedMessageAsync(ulong[] selectedValues)
        {
            await Task.CompletedTask;
        }

        [ComponentInteraction("sar-role-removing")]
        public async Task RemovingRoleAsync()
        {
            await Task.CompletedTask;
        }

        [ComponentInteraction("sar-removed-role")]
        public async Task RemovedRoleAsync(ulong[] selectedValues)
        {
            await Task.CompletedTask;
        }
    }
}
