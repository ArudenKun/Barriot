namespace Barriot.Interaction.Modules.SelfRoles
{
    public class SarRemoveModule
    {
        [ComponentInteraction("sar-removed-message:*")]
        public async Task RemovedMessageAsync(ulong messageId)
        {

        }

        [ComponentInteraction("sar-removed-role:*")]
        public async Task RemovedRoleAsync(ulong roleId)
        {

        }
    }
}
