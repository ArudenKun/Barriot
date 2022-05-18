using Barriot.Interaction.Modals;
using Barriot.Interaction.Services;

namespace Barriot.Interaction.Modules.SelfRoles
{
    public class SarMakeModule : BarriotModuleBase
    {
        private readonly SarMakeService _service;

        public SarMakeModule(SarMakeService service)
        {
            _service = service;
        }

        [ComponentInteraction("sar-new-message")]
        public async Task MakeNewMessageAsync()
        {
            var cb = new ComponentBuilder()
                .WithButton("Start message creation", "sar-new-message-channel", ButtonStyle.Primary);

            await UpdateAsync(
                text: ":exclamation: **Please press the button below to start message creation.** *Keep the channel ID where the message should be sent ready.*",
                components: cb.Build());
        }

        [ComponentInteraction("sar-new-message-channel")]
        public async Task CheckNewMessageChannelAsync()
        {
            var mb = new ModalBuilder()
                .WithTitle("Specify message channel:")
                .WithCustomId("sar-message-channel-confirm")
                .AddTextInput("Channel ID:", "entry", TextInputStyle.Short, "rules-n-info", 1, 65, true);

            await RespondWithModalAsync(mb.Build());
        }

        [ComponentInteraction("sar-message-channel-confirm")]
        public async Task NewMessageChannelConfirmAsync(QueryModal<string> modal)
        {
            if (!ulong.TryParse(modal.Result, out var channelId))
            {
                await RespondAsync(
                    text: ":x: **Provided channel ID is not a valid Discord ID!** *Please try again by pressing start in the previous message.*",
                    ephemeral: true);
                return;
            }

            var channel = await Context.Client.GetChannelAsync(channelId);

            if (channel is null)
            {
                await RespondAsync(
                    text: "**Provided channel ID is not from a channel in this server!** *Please try again by pressing start in the previous message.*",
                    ephemeral: true);
                return;
            }

            if (channel is not RestTextChannel textChannel)
            {
                await RespondAsync(
                    text: ":x: **Provided channel is not a text channel. I can't send messages there!** *Please try again by pressing start in the previous message.*",
                    ephemeral: true);
                return;
            }

            if (!(await Context.Guild.GetCurrentUserAsync()).GetPermissions(textChannel).Has(ChannelPermission.SendMessages))
            {
                await RespondAsync(
                    text: ":x: **Provided channel is a channel I cannot send messages to.** *Please set up the perms so I can, and then try again.*",
                    ephemeral: true);
                return;
            }

            _service.CreateFromChannel(Context.User.Id, textChannel);

            var cb = new ComponentBuilder()
                .WithButton("Write message content", $"sar-new-message-creating");

            await RespondAsync(
                text: ":white_check_mark: **Found channel to write messages to!**",
                components: cb.Build(),
                ephemeral: true);
        }

        [ComponentInteraction("sar-new-message-creating")]
        public async Task CreatingNewMessageAsync()
        {
            var mb = new ModalBuilder()
                .WithTitle("Write the message you want to use:")
                .WithCustomId($"sar-new-message-confirm")
                .AddTextInput("Message content (Markdown supported):", "entry", TextInputStyle.Paragraph, null, 1, 2000);

            await RespondWithModalAsync(mb.Build());
        }

        [ComponentInteraction("sar-new-message-confirm")]
        public async Task ConfirmNewMessageAsync(QueryModal<string> modal)
        {
            if (!_service.TryGetData(Context.User.Id, out var args) || args is null)
            {
                await RespondAsync(
                    text: ":x: **This sar creation has been abandoned!** \n\n> This could be the case because it took over 15 minutes to finish your message, or because you started on another message.",
                    ephemeral: true);
                return;
            }

            if (string.IsNullOrEmpty(modal.Result))
            {
                await RespondAsync(
                    text: ":x: **Cannot send an empty message!** Message creation failed because you failed to fill in content.",
                    ephemeral: true);
                return;
            }

            if (!_service.TryAddData(Context.User.Id, x => x.Content = modal.Result!))
                throw new InvalidOperationException();

            var cb = new ComponentBuilder()
                .WithButton("Format as embed", $"sar-from-message-defined:{true}", ButtonStyle.Secondary)
                .WithButton("Format as text", $"sar-from-message-defined:{false}", ButtonStyle.Secondary);

            await UpdateAsync(
                text: ":question: **How do you want to format your message?**",
                components: cb.Build());
        }

        [ComponentInteraction("sar-from-message-defined:*")]
        public async Task MessageSourceAsync(bool embed)
        {
            if (!_service.TryGetData(Context.User.Id, out var args) || args is null)
            {
                await RespondAsync(
                    text: ":x: **This sar creation has been abandoned!** \n\n> This could be the case because it took over 15 minutes to finish your message, or because you started on another message.",
                    ephemeral: true);
                return;
            }

            if (!_service.TryAddData(Context.User.Id, x => x.FormatAsEmbed = embed))
                throw new InvalidOperationException();

            var cb = new ComponentBuilder()
                .WithButton("Button", $"sar-from-message-format:{true}", ButtonStyle.Secondary)
                .WithButton("Drop-down", $"sar-from-message-format:{false}", ButtonStyle.Secondary);

            await UpdateAsync(
                text: ":question: **How do you want to format your self-assign role (SAR)?**, *Please keep your role ID ready, by pressing an option, the creation menu will open.",
                components: cb.Build());
        }

        [ComponentInteraction("sar-from-message-source:*,*")]
        public async Task MessageSourceAsync(ulong messageId)
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

            _service.CreateFromMessage(Context.User.Id, userMessage);

            var cb = new ComponentBuilder()
                .WithButton("Button", $"sar-from-message-format:{true}", ButtonStyle.Secondary)
                .WithButton("Drop-down", $"sar-from-message-format:{false}", ButtonStyle.Secondary);

            await UpdateAsync(
                text: ":question: **How do you want to format your self-assign role (SAR)?**, *Please keep your role ID ready, by pressing an option, the creation menu will open.",
                components: cb.Build());
        }

        [ComponentInteraction("sar-from-message-format:*")]
        public async Task FormatQueryAsync(bool button)
            => await RespondWithModalAsync<SarMakeModal>($"sar-from-message-confirm:{button}");

        [ComponentInteraction("sar-from-message-confirm:*,*")]
        public async Task ConfirmFromMessageAsync(bool button, SarMakeModal modal)
        {
            if (!_service.TryGetData(Context.User.Id, out var args) || args is null)
            {
                await RespondAsync(
                    text: ":x: **This sar creation has been abandoned!** \n\n> This could be the case because it took over 15 minutes to finish your message, or because you started on another message.",
                    ephemeral: true);
                return;
            }

            if (!ulong.TryParse(modal.Role, out var roleId))
            {
                await RespondAsync(
                    text: ":x: **Provided role ID is not a valid Discord ID!** *Please try again by reselecting your option in the previous message.*",
                    ephemeral: true);
                return;
            }

            var role = Context.Guild.GetRole(roleId);

            if (role is null)
            {
                await RespondAsync(
                    text: ":x: **Provided role ID is not a role in this server!** *Please try again by reselecting your option in the previous message.*",
                    ephemeral: true);
                return;
            }

            var guild = await GuildEntity.GetAsync(Context.Guild.Id);

            var message = args.Message;

            if (message is null)
            {
                EmbedBuilder eb = null!;
                if (args.FormatAsEmbed)
                {
                    eb = new();
                    eb.WithColor(Color.Blue);
                    eb.WithDescription(args.Content);
                }
                message = await args.Channel.SendMessageAsync(
                    text: args.FormatAsEmbed ? null : args.Content,
                    embed: eb?.Build());
            }

            var name = modal.Label ?? role.Name;
            var cb = message.Components.Any() 
                ? ComponentBuilder.FromMessage(message) 
                : new();

            if (button)
            {
                cb.WithButton(name, $"sar-from-button:{roleId}", ButtonStyle.Secondary);

                await message.ModifyAsync(x =>
                {
                    x.Components = cb.Build();
                });
            }
            else
            {
                var actionRow = new ActionRowBuilder();
                var sb = new SelectMenuBuilder()
                    .WithCustomId("sar-from-menu")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .WithPlaceholder("Select a (or more) roles to remove/add.");

                if (cb.ActionRows.Any())
                {
                    var rows = cb.ActionRows.Where(x => x.Components.Any(x => x is SelectMenuComponent));
                    if (rows.Any())
                    {
                        actionRow = rows.First();
                        sb = (actionRow.Components.First() as SelectMenuComponent)!.ToBuilder();
                        cb.ActionRows.RemoveAll(x => x.Components.Any(x => x is SelectMenuComponent)); // there can only be one.
                    }
                }

                if (sb.Options.Count >= 25)
                {
                    await RespondAsync(
                        text: ":x: **Failed to add SAR to dropdown!** *The max amount of options in a select menu is 25, this value has been reached.*",
                        ephemeral: true);
                    return;
                }

                sb.AddOption(name, roleId.ToString(), modal.Description);

                actionRow.Components = new();

                actionRow.AddComponent(sb.WithMaxValues(sb.Options.Count).Build());

                cb.ActionRows.Add(actionRow);

                cb.ActionRows = cb.ActionRows.OrderBy(x => x.Components.Count).ToList();

                await message.ModifyAsync(x =>
                {
                    x.Components = cb.Build();
                });
            }

            await UpdateAsync(
                text: $":white_check_mark: **Succesfully added SAR to message.**\n\n> Link: https://discord.com/channels/{Context.Guild.Id}/{args.Channel.Id}/{message.Id}");
        }
    }
}
