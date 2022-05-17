using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules.Administration
{
    [EnabledInDm(false)]
    [RequireUserPermission(GuildPermission.Administrator)]
    [IgnoreBlacklistedUsers]
    public class SarManageModule : BarriotModuleBase
    {
        [SlashCommand("self-roles", "Manage self-assign roles for this guild.")]
        public async Task ManageAsync()
        {

        }

        [ComponentInteraction("sar-messages-manage")]
        public async Task ManageMessagesAsync()
        {

        }

        [ComponentInteraction("sar-message-select")]
        public async Task SelectMessageAsync(int page)
        {
            // select message to edit.
        }

        [ComponentInteraction("sar-message-selected")]
        public async Task ManageSingleMessageAsync(ulong[] selectedValues)
        {
            // delete this message or delete roles from this message.
        }
    }
}
