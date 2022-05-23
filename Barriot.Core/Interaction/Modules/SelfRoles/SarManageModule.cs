using Barriot.Interaction.Attributes;
using Barriot.Extensions.Pagination;
using Barriot.Interaction.Modals;
using Barriot.Extensions;
using Barriot.Interaction.Services;

namespace Barriot.Interaction.Modules.Administration
{
    [EnabledInDm(false)]
    [RequireUserPermission(GuildPermission.Administrator)]
    [IgnoreBlacklistedUsers]
    public class SarManageModule : BarriotModuleBase
    {
        private readonly SarManageService _service;

        public SarManageModule(SarManageService service)
        {
            _service = service;
        }

        [SlashCommand("self-roles", "Manage self-assign roles for this guild.")]
        public async Task ManageAsync()
        {
            var cb = new ComponentBuilder()
                .WithButton("Add new SAR message", "sar-new-message", ButtonStyle.Primary)
                .WithButton("Edit a SAR message", "sar-message-manage", ButtonStyle.Secondary);

            await RespondAsync(
                text: ":bar_chart: **Manage self-assign roles (SAR) in this server.** Please choose any of the below options.",
                components: cb.Build(),
                ephemeral: true);
        }

        [ComponentInteraction("sar-message-manage")]
        public async Task FindingMessageAsync()
        {
            var mb = new ModalBuilder()
                .WithCustomId("sar-message-view")
                .WithTitle("Manage an existing SAR message:")
                .AddTextInput("Message link:", "entry", TextInputStyle.Short);

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("sar-message-view")]
        public async Task ManageMessageAsync(QueryModal<string> modal)
        {
            if (!modal.Result.TryGetLinkData(out var data))
            {
                await RespondAsync(
                    text: ":x: **Input link is not a Discord message link!**",
                    ephemeral: true);
                return;
            }

            var channel = await Context.Guild.GetChannelAsync(data[1]);

            if (channel is null || channel is not RestTextChannel textChannel)
            {
                await RespondAsync(
                    text: ":x: **The message link does not lead to a usable/viewable channel.**",
                    ephemeral: true);
                return;
            }

            var message = await textChannel.GetMessageAsync(data[2]);

            if (message is null || message is not RestUserMessage userMessage)
            {
                await RespondAsync(
                    text: ":x: **The message link does not lead to a usable message.**",
                    ephemeral: true);
                return;
            }

            if (userMessage.Interaction is not null || userMessage.Author.Id != Context.Client.CurrentUser.Id)
            {
                await RespondAsync(
                    text: ":x: **This message is not a valid self-assign role message!** *The message has to be created by Barriot and not be part of an interaction.*",
                    ephemeral: true);
                return;
            }

            _service.CreateFromMessage(userMessage.Id, userMessage);

            var cb = new ComponentBuilder()
                .WithButton("Add new role to message", $"sar-from-message-source:{userMessage.Id}", ButtonStyle.Success)
                .WithButton("Modify message content", $"sar-message-editing:{userMessage.Id}", ButtonStyle.Secondary);

            await UpdateAsync(
                text: ":scroll: **Manage this message.** *Please select one of the below options to add a role or modify this message's content.*",
                components: cb.Build());
        }

        [ComponentInteraction("sar-message-editing:*")]
        public async Task EditingContentAsync(ulong messageId)
        {
            var mb = new ModalBuilder()
                .WithTitle("Modify message content:")
                .AddTextInput("New message content (markdown supported):", "entry", TextInputStyle.Paragraph)
                .WithCustomId($"sar-message-edited:{messageId}");

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("sar-message-edited:*")]
        public async Task EditSuccessAsync(ulong messageId, QueryModal<string> modal)
        {
            if (!_service.TryGetData(messageId, out var data))
            {
                await RespondAsync(
                    text: ":x: **This message modification has been abandoned!** \n\n> This could be the case because it took over 15 minutes to finish your message, or because you started on another message.",
                    ephemeral: true);
                return;
            }

            _service.TryRemoveData(messageId);

            await data.ModifyAsync(x => x.Content = modal.Result);

            await RespondAsync(
                text: ":white_check_mark: **Succesfully modified message content!**");
        }
    }
}
