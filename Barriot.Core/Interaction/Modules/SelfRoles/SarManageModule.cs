using Barriot.Interaction.Attributes;
using Barriot.Extensions.Pagination;
using Barriot.Interaction.Modals;

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
            var cb = new ComponentBuilder()
                .WithButton("Add new SAR message", "sar-new-message", ButtonStyle.Primary)
                .WithButton("Manage messages", "sar-messages-manage:1", ButtonStyle.Secondary);

            await RespondAsync(
                text: ":bar_chart: **Manage self-assign roles (SAR) in this server.** Please choose any of the below options.",
                components: cb.Build(),
                ephemeral: true);
        }

        [ComponentInteraction("sar-messages-manage:*")]
        public async Task ManageMessagesAsync(int page)
        {
            var guild = await GuildEntity.GetAsync(Context.Guild.Id);

            if (!Paginator<SelfAssignMessage>.TryGet(out var paginator))
            {
                paginator = new PaginatorBuilder<SelfAssignMessage>()
                    .WithCustomId("sar-messages-manage")
                    .WithPages(x => new($"{x.MessageId}", $"In channel: <#{x.ChannelId}> ({x.AssignRoles.Count} roles)"))
                    .Build();
            }
            var value = paginator.GetPage(page, guild.SelfRoleMessages);

            value.Component.WithButton("Modify a message from this page", $"sar-message-select:{page}", ButtonStyle.Secondary);

            await UpdateAsync(
                text: ":newspaper: **Manage your current SAR messages.**",
                components: value.Component.Build(),
                embed: value.Embed.Build());
        }

        [ComponentInteraction("sar-message-select:*")]
        public async Task SelectMessageAsync(int page)
        {
            var guild = await GuildEntity.GetAsync(Context.Guild.Id);

            var sb = new SelectMenuBuilder()
                .WithMinValues(1)
                .WithMaxValues(1)
                .WithCustomId("sar-message-selected")
                .WithPlaceholder("Please select a message to edit.");

            int index = page * 10 - 10;

            var range = guild.SelfRoleMessages.GetRange(index, guild.SelfRoleMessages.Count - index);
            for (int i = 0; i < range.Count; i++)
            {
                if (i == 10)
                    break;
                sb.AddOption($"ID: {range[i].MessageId}", $"{range[i].MessageId}", $"From channel: <#{range[i].ChannelId}>");
            }

            var cb = new ComponentBuilder()
                .WithSelectMenu(sb);

            await UpdateAsync(
                text: ":pen_fountain: **Select a message to edit it's self-assign roles or content.**",
                components: cb.Build());
        }

        [ComponentInteraction("sar-message-selected")]
        public async Task ManageSingleMessageAsync(ulong[] selectedValues)
            => await ManageSingleMessageInternalAsync(selectedValues[0]);

        [ComponentInteraction("sar-message-view:*,*")]
        public async Task ManageMessageRolesAsync(ulong messageId, int page)
            => await ManageSingleMessageInternalAsync(messageId, page);

        private async Task ManageSingleMessageInternalAsync(ulong messageId, int page = 1)
        {
            var guild = await GuildEntity.GetAsync(Context.Guild.Id);

            var message = guild.SelfRoleMessages.First(x => x.MessageId == messageId);

            if (message is null)
                return; // failsafe

            if (!Paginator<SelfAssignRole>.TryGet(out var paginator))
            {
                paginator = new PaginatorBuilder<SelfAssignRole>()
                    .WithPages(x => new($"{x.Name}", $"{x.Description}"))
                    .WithCustomId("sar-message-view")
                    .Build();
            }
            var value = paginator.GetPage(page, message.AssignRoles, messageId);

            value.Component.WithButton("Add new role to message", $"sar-from-message-defined:{messageId}", ButtonStyle.Success);
            value.Component.WithButton("Modify message content", $"sar-message-editing:{messageId}", ButtonStyle.Secondary);
            value.Component.WithButton("Remove role from this page", $"sar-role-removing:{messageId},{page}", ButtonStyle.Danger);

            var link = $"https://discord.com/channels/{Context.Guild.Id}/{message.ChannelId}/{message.MessageId}";

            await UpdateAsync(
                text: ":scroll: **Manage roles on this message.** Please select one of the below options to manage individual roles or this message's content.",
                embed: value.Embed.Build(),
                components: value.Component.Build());
        }

        [ComponentInteraction("sar-message-editing:*")]
        public async Task EditMessageContentAsync(ulong messageId)
        {
            var mb = new ModalBuilder()
                .WithTitle("Modify message content:")
                .AddTextInput("New message content (markdown supported):", "entry", TextInputStyle.Paragraph)
                .WithCustomId($"sar-message-edited:{messageId}");

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("sar-message-edited:*")]
        public async Task EditContentSuccessAsync(ulong messageId, QueryModal<string> modal)
        {
            var guild = await GuildEntity.GetAsync(Context.Guild.Id);

            var sarMessage = guild.SelfRoleMessages.First(x => x.MessageId == messageId);

            if (sarMessage is null)
                return;

            var channel = await Context.Guild.GetChannelAsync(sarMessage.ChannelId);

            if (channel is null || channel is not RestTextChannel textChannel)
                return; // user deleted channel

            var message = await textChannel.GetMessageAsync(messageId);

            if (message is null || message is not RestUserMessage userMessage)
                return;

            await userMessage.ModifyAsync(x => x.Content = modal.Result);

            await UpdateAsync(
                text: ":white_check_mark: **Succesfully modified message content!**");
        }
    }
}
