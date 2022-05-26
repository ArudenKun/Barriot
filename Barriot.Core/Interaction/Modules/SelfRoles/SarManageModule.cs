using Barriot.Extensions;
using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;
using Barriot.Interaction.Services;
using Barriot.Models;
using Barriot.Models.Files;

namespace Barriot.Interaction.Modules
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
                format: "bar_chart",
                header: "Manage self-assign roles (SAR) in this server.",
                context: "Please choose any of the below options.",
                components: cb,
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
            if (!JumpUrl.TryParse(modal.Result!, out var messageUrl))
            {
                await RespondAsync(
                    error: "Input link is not a Discord message link!",
                    context: $"Input: ({modal.Result})");
                return;
            }

            var channel = await Context.Guild.GetChannelAsync(messageUrl.ChannelId);

            if (channel is null || channel is not RestTextChannel textChannel)
            {
                await RespondAsync(
                    error: "The message link does not lead to a usable/viewable channel.",
                    context: $"Input: ({modal.Result})");
                return;
            }

            var message = await textChannel.GetMessageAsync(messageUrl.MessageId);

            if (message is null || message is not RestUserMessage userMessage)
            {
                await RespondAsync(
                    error: "The message link does not lead to a usable message.",
                    context: $"Input: ({modal.Result})");
                return;
            }

            if (userMessage.Interaction is not null || userMessage.Author.Id != Context.Client.CurrentUser.Id)
            {
                await RespondAsync(
                    error: "This message is not a valid self-assign role message!",
                    context: "The message has to be created by Barriot and not be part of an interaction.");
                return;
            }

            _service.CreateFromMessage(userMessage.Id, userMessage);

            var cb = new ComponentBuilder()
                .WithButton("Add new role to message", $"sar-from-message-source:{userMessage.Id}", ButtonStyle.Success)
                .WithButton("Modify message content", $"sar-message-editing:{userMessage.Id}", ButtonStyle.Secondary);

            await RespondAsync(
                format: "scroll",
                header: "Manage this message.",
                context: "Please select one of the below options to add a role or modify this message's content.",
                components: cb,
                ephemeral: true);
        }

        [ComponentInteraction("sar-message-editing:*")]
        public async Task EditingContentAsync(ulong messageId)
        {
            var mb = new ModalBuilder()
                .WithTitle("Modify message content")
                .AddTextInput("(Markdown supported)", "entry", TextInputStyle.Paragraph)
                .WithCustomId($"sar-message-edited:{messageId}");

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("sar-message-edited:*")]
        public async Task EditSuccessAsync(ulong messageId, QueryModal<string> modal)
        {
            var eb = new EmbedBuilder()
                .WithTitle("New message content:")
                .WithDescription(modal.Result);

            if (!_service.TryGetData(messageId, out var data))
            {
                await RespondAsync(
                    text: $":x: **This message modification has been abandoned!** {FileExtensions.GetError(ErrorInfo.SARContextAbandoned)}",
                    embed: eb.Build(),
                    ephemeral: true);
                return;
            }

            await data.ModifyAsync(x => x.Content = modal.Result);

            await RespondAsync(
                format: ResultFormat.Success,
                header: "Succesfully modified message content!",
                embed: eb,
                ephemeral: true);
        }
    }
}
