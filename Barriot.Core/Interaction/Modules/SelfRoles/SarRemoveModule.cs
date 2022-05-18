namespace Barriot.Interaction.Modules.SelfRoles
{
    public class SarRemoveModule
    {
        [ComponentInteraction("sar-message-removing")]
        public async Task RemovingMessageAsync()
        {

        }

        [ComponentInteraction("sar-removed-message:*")]
        public async Task RemovedMessageAsync(ulong[] selectedValues)
        {

        }

        [ComponentInteraction("sar-role-removing")]
        public async Task RemovingRoleAsync()
        {

        }

        [ComponentInteraction("sar-removed-role")]
        public async Task RemovedRoleAsync(ulong[] selectedValues)
        {

        }
    }
}
