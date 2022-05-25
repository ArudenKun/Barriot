using Barriot.Extensions;
using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;
using Barriot.Interaction.Services;
using Barriot.Models.Files;

namespace Barriot.Interaction.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class SarMakeModule : BarriotModuleBase
    {
        private readonly SarMakeService _service;

        public SarMakeModule(SarMakeService service)
        {
            _service = service;
        }

        [ComponentInteraction("sar-new-message")]
        public async Task NewMessageAsync()
        {
            var cb = new ComponentBuilder()
                .WithButton("Start message creation", "sar-new-message-channel", ButtonStyle.Primary);

            await UpdateAsync(
                text: ":exclamation: **Please press the button below to start message creation.** *Keep the channel ID where the message should be sent ready.*",
                components: cb.Build());
        }

        [ComponentInteraction("sar-new-message-channel")]
        public async Task FindingChannelAsync()
        {
            var mb = new ModalBuilder()
                .WithTitle("Specify message channel:")
                .WithCustomId("sar-message-channel-confirm")
                .AddTextInput("Channel ID:", "entry", TextInputStyle.Short, "rules-n-info", 1, 65, true);

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("sar-message-channel-confirm")]
        public async Task ChannelFoundAsync(QueryModal<string> modal)
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
        public async Task CreateContentAsync()
        {
            var mb = new ModalBuilder()
                .WithTitle("Write the message you want to use:")
                .WithCustomId($"sar-new-message-confirm")
                .AddTextInput("Message content (Markdown supported):", "entry", TextInputStyle.Paragraph, null, 1, 2000);

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("sar-new-message-confirm")]
        public async Task CreatedContentAsync(QueryModal<string> modal)
        {
            if (!_service.TryGetData(Context.User.Id, out var args) || args is null)
            {
                await RespondAsync(
                    text: $":x: **This sar creation has been abandoned!** {FileExtensions.GetError(ErrorInfo.SARContextAbandoned)}",
                    ephemeral: true);
                return;
            }

            if (string.IsNullOrEmpty(modal.Result))
            {
                await RespondAsync(
                    text: ":x: **Cannot send an empty message!** *Message creation failed because you failed to fill in content.*",
                    ephemeral: true);
                return;
            }

            if (!_service.TryAddData(Context.User.Id, x => x.Content = modal.Result!))
                throw new InvalidOperationException();

            var cb = new ComponentBuilder()
                .WithButton("Format as embed", $"sar-from-message-defined:{true}", ButtonStyle.Secondary)
                .WithButton("Format as text", $"sar-from-message-defined:{false}", ButtonStyle.Secondary);

            var eb = new EmbedBuilder()
                .WithTitle("Message content:")
                .WithColor(Color.Blue)
                .WithDescription(modal.Result);

            await RespondAsync(
                text: ":question: **How do you want to format your message?**",
                embed: eb.Build(),
                components: cb.Build(),
                ephemeral: true);
        }

        [ComponentInteraction("sar-from-message-defined:*")]
        public async Task MessageSourceAsync(bool embed)
        {
            if (!_service.TryGetData(Context.User.Id, out var args) || args is null)
            {
                await RespondAsync(
                    text: $":x: **This sar creation has been abandoned!** {FileExtensions.GetError(ErrorInfo.SARContextAbandoned)}",
                    ephemeral: true);
                return;
            }

            if (!_service.TryAddData(Context.User.Id, x => x.FormatAsEmbed = embed))
                throw new InvalidOperationException();

            var cb = new ComponentBuilder()
                .WithButton("Button", $"sar-from-message-format:{true}", ButtonStyle.Secondary)
                .WithButton("Drop-down", $"sar-from-message-format:{false}", ButtonStyle.Secondary);

            await UpdateAsync(
                text: ":question: **How do you want to format your self-assign role (SAR)?**, *Please keep your role ID ready. By pressing an option, the creation menu will open.*",
                components: cb.Build());
        }

        [ComponentInteraction("sar-from-message-source:*")]
        public async Task MessageSourceAsync(ulong messageId)
        {
            _service.CreateFromManageCache(Context.User.Id, messageId);

            var cb = new ComponentBuilder()
                .WithButton("Button", $"sar-from-message-format:{true}", ButtonStyle.Secondary)
                .WithButton("Drop-down", $"sar-from-message-format:{false}", ButtonStyle.Secondary);

            await UpdateAsync(
                text: ":question: **How do you want to format your self-assign role (SAR)?**, *Please keep your role ID ready, by pressing an option, the creation menu will open.*",
                components: cb.Build());
        }

        [ComponentInteraction("sar-from-message-format:*")]
        public async Task FormatQueryAsync(bool button)
            => await RespondWithModalAsync<SarMakeModal>($"sar-from-message-confirm:{button}");

        [ModalInteraction("sar-from-message-confirm:*")]
        public async Task ConfirmFromMessageAsync(bool button, SarMakeModal modal)
        {
            if (!_service.TryGetData(Context.User.Id, out var args) || args is null)
            {
                await RespondAsync(
                    text: $":x: **This sar creation has been abandoned!** {FileExtensions.GetError(ErrorInfo.SARContextAbandoned)}",
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

            var message = args.Message;
            var cb = new ComponentBuilder();

            if (message is null)
            {
                EmbedBuilder emb = null!;
                if (args.FormatAsEmbed)
                {
                    emb = new();
                    emb.WithColor(Color.Blue);
                    emb.WithDescription(args.Content);
                }
                message = await args.Channel.SendMessageAsync(
                    text: args.FormatAsEmbed
                        ? null
                        : args.Content,
                    embed: emb?.Build());
            }

            else
                cb = ComponentBuilder.FromMessage(message);

            var name = string.IsNullOrEmpty(modal.Label)
                ? role.Name
                : modal.Label;

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
                var sb = new SelectMenuBuilder();
                if (!message.Components.Any())
                {
                    sb.WithCustomId("sar-from-menu");
                    sb.WithMinValues(1);
                    sb.WithPlaceholder("Select a (or more) roles to remove/add.");
                }

                else
                {
                    var rows = cb.ActionRows.Where(x => x.Components.Any(x => x is SelectMenuComponent)).ToList();
                    if (rows.Any())
                    {
                        sb = (rows[0].Components.First() as SelectMenuComponent)!.ToBuilder();
                    }
                    else
                    {
                        await RespondAsync(
                            text: ":x: **A self-assign message with only buttons cannot include a dropdown.** *Consider creating a new message starting with only a dropdown instead.*",
                            ephemeral: true);
                        return;
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
                sb.WithMaxValues(sb.Options.Count);

                if (cb.ActionRows is null)
                {
                    cb.ActionRows = new(1);
                    cb.ActionRows.Add(new ActionRowBuilder().AddComponent(sb.Build()));
                }

                else
                    cb.ActionRows[0] = new ActionRowBuilder().AddComponent(sb.Build());

                await message.ModifyAsync(x =>
                {
                    x.Components = cb.Build();
                });
            }

            var link = $"https://discord.com/channels/{Context.Guild.Id}/{args.Channel.Id}/{message.Id}";

            var eb = new EmbedBuilder()
                .WithTitle("Click to view message")
                .WithUrl($"https://discord.com/channels/{Context.Guild.Id}/{args.Channel.Id}/{message.Id}")
                .WithColor(Color.Blue)
                .AddField("Role:", name);

            if (!button)
                eb.AddField("Description:", string.IsNullOrEmpty(modal.Description) ? "None" : modal.Description);

            _service.TryRemoveData(Context.User.Id);

            await RespondAsync(
                text: $":white_check_mark: **Succesfully added SAR to message.**\n\n> Link: {link}",
                embed: eb.Build(),
                ephemeral: true);
        }
    }
}
