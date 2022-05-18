using Barriot.Caching;
using Barriot.Interaction.Modals;

namespace Barriot.Interaction.Modules.SelfRoles
{
    public class SarMakeModule : BarriotModuleBase
    {
        private readonly ChannelCache _cache;

        public SarMakeModule(ChannelCache cache)
        {
            _cache = cache;
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
            var channels = await _cache.GetChannelsAsync(Context.Guild, x => x.Id == channelId);

            if (!channels.Any())
            {
                await RespondAsync(
                    text: "**Provided channel ID is not from a channel in this server!** *Please try again by pressing start in the previous message.*",
                    ephemeral: true);
                return;
            }

            var channel = channels.First();

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

            var cb = new ComponentBuilder()
                .WithButton("Write message content", $"sar-new-message-creating:{channelId}");

            await RespondAsync(
                text: ":white_check_mark: **Found channel to write messages to!**",
                components: cb.Build(),
                ephemeral: true);
        }

        [ComponentInteraction("sar-new-message-creating:*")]
        public async Task CreatingNewMessageAsync(ulong channelId)
        {

        }

        [ComponentInteraction("sar-new-message-confirm:*")]
        public async Task ConfirmNewMessageAsync(ulong channelId)
        {
            
        }

        [ComponentInteraction("sar-from-message-defined")]
        public async Task MessageSourceAsync(ulong messageId)
        {
            var cb = new ComponentBuilder()
                .WithButton("Button", $"sar-from-message-format:{messageId},{true}", ButtonStyle.Secondary)
                .WithButton("Drop-down", $"sar-from-message-format:{messageId},{false}", ButtonStyle.Secondary);

            await UpdateAsync(
                text: ":question: **How do you want to format your self-assign role (SAR)?**, *Please keep your role ID ready, by pressing an option, the creation menu will open.",
                components: cb.Build());
        }

        [ComponentInteraction("sar-from-message-format:*,*")]
        public async Task FormatQueryAsync(ulong messageId, bool button)
            => await RespondWithModalAsync<SarMakeModal>($"sar-from-message-confirm:{messageId},{button}");

        [ComponentInteraction("sar-from-message-confirm:*,*")]
        public async Task ConfirmFromMessageAsync(ulong messageId, bool button, SarMakeModal modal)
        {
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

            var sarMessage = guild.SelfRoleMessages.First(x => x.MessageId == messageId);

            var channel = await Context.Client.GetChannelAsync(sarMessage.ChannelId);

            if (channel is null || channel is not RestTextChannel textChannel)
            {
                await RespondAsync(
                    text: ":x: **The message channel this SAR creation should target does not exist anymore!** *Because of this, SAR creation can no longer continue.*",
                    ephemeral: true);
                return;
            }

            var message = await textChannel.GetMessageAsync(messageId);

            if (message is null || message is not RestUserMessage userMessage)
            {
                await RespondAsync(
                    text: ":x: **The message this SAR creation should target does not exist anymore!** *Because of this, SAR creation can no longer continue.*",
                    ephemeral: true);
                return;
            }

            var name = modal.Label ?? role.Name;
            var cb = ComponentBuilder.FromMessage(userMessage);

            if (button)
            {
                cb.WithButton(name, $"sar-from-button:{roleId}", ButtonStyle.Secondary);

                await userMessage.ModifyAsync(x =>
                {
                    x.Components = cb.Build();
                });
            }
            else
            {
                var actionRow = cb.ActionRows.First(x => x.Components.Any(x => x is SelectMenuComponent)); // there is always only 1 select menu row, and 1 select menu in that row.

                var sb = (actionRow.Components.First() as SelectMenuComponent)!.ToBuilder();

                if (sb.Options.Count >= 25)
                {
                    await RespondAsync(
                        text: ":x: **Failed to add SAR to dropdown!** *The max amount of options in a select menu is 25, this value has been reached.*",
                        ephemeral: true);
                    return;
                }    
                sb.AddOption(name, roleId.ToString(), modal.Description);

                actionRow.Components = new();

                actionRow.AddComponent(sb.Build());

                await userMessage.ModifyAsync(x =>
                {
                    x.Components = cb.Build();
                });
            }

            await UpdateAsync(
                text: $":white_check_mark: **Succesfully added SAR to message.**\n\n> Link: https://discord.com/channels/{Context.Guild.Id}/{sarMessage.ChannelId}/{sarMessage.MessageId}");
        }
    }
}
