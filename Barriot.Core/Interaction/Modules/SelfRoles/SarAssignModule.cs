using System.Text;

namespace Barriot.Interaction.Modules.SelfRoles
{
    public class SarAssignModule : BarriotModuleBase
    {
        [ComponentInteraction("sar-from-button:*")]
        public async Task AssignRoleFromButtonAsync(ulong roleId)
        {
            if (Context.User is not RestGuildUser guildUser)
                return;

            bool added = true;
            if (!guildUser.RoleIds.Any(x => x == roleId))
                await guildUser.AddRoleAsync(roleId);

            else
            {
                added = false;
                await guildUser.RemoveRoleAsync(roleId);
            }

            await RespondAsync(
                text: $":white_check_mark: **Succesfully {(added ? "added" : "removed")} <@&{roleId}>.**",
                ephemeral: true);
        }

        [ComponentInteraction("sar-from-menu")]
        public async Task AssignRoleFromMenuAsync(ulong[] selectedValues)
        {
            if (Context.User is not RestGuildUser guildUser)
                return;

            await DeferAsync(true);

            var builder = new StringBuilder();

            builder.AppendLine(":white_check_mark: **Succesfully modified roles:**\n");

            foreach(var roleId in selectedValues)
            {
                bool added = true;
                if (!guildUser.RoleIds.Any(x => x == roleId))
                    await guildUser.AddRoleAsync(roleId);

                else
                {
                    added = false;
                    await guildUser.RemoveRoleAsync(roleId);
                }

                builder.AppendLine($"> **{(added ? "Added" : "Removed")}:** <@&{roleId}>.");
            }

            await FollowupAsync(
                text: builder.ToString(),
                ephemeral: true);
        }
    }
}
