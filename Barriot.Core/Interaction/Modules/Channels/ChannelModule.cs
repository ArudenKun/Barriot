using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;

namespace Barriot.Interaction.Modules
{
    [EnabledInDm(false)]
    [RequireBotPermission(ChannelPermission.ManageChannels)]
    [IgnoreBlacklistedUsers]
    public class ChannelModule : BarriotModuleBase
    {
        [SlashCommand("channel", "Manages this channel.")]
        public async Task ManageChannelAsync()
        {
            var perms = (Context.Channel as ITextChannel)!.PermissionOverwrites.Where(x => x.TargetId == Context.Guild.Id);

            bool isLocked = false;
            if (perms.Any() && perms.First().Permissions.SendMessages == PermValue.Deny)
                isLocked = true;

            var cb = new ComponentBuilder()
                .WithButton($"{(isLocked ? "Unlock" : "Lock")} channel", $"channel-lock:{Context.User.Id},{isLocked}", isLocked ? ButtonStyle.Success : ButtonStyle.Danger)
                .WithButton("Clone channel", $"channel-clone:{Context.User.Id}", ButtonStyle.Secondary)
                .WithButton("Delete channel", $"channel-delete:{Context.User.Id}", ButtonStyle.Danger)
                .WithButton("Prune channel", $"channel-prune:{Context.User.Id}", ButtonStyle.Danger);

            await RespondAsync(
                text: ":woman_office_worker: **Manage this channel.** Please select an action below.",
                components: cb.Build(),
                ephemeral: true);
        }

        [DoUserCheck]
        [ComponentInteraction("channel-clone:*")]
        public async Task CloneAsync(ulong _)
        {
            if (Context.Channel is RestTextChannel channel)
            {
                await RespondAsync(
                    text: ":white_check_mark: **Cloning channel...**");

                var pinnedMessages = await channel.GetPinnedMessagesAsync();

                await channel.DeleteAsync();

                var clone = await Context.Guild.CreateTextChannelAsync(channel.Name, x =>
                {
                    x.PermissionOverwrites = channel.PermissionOverwrites.ToList();
                    x.CategoryId = channel.CategoryId;
                    x.Position = channel.Position;
                    x.Topic = channel.Topic;
                    x.SlowModeInterval = channel.SlowModeInterval;
                    x.IsNsfw = channel.IsNsfw;
                });

                await clone.SendMessageAsync(":white_check_mark: **This channel has successfully been cloned!**");
            }
            else
                throw new NotSupportedException();
        }

        [DoUserCheck]
        [ComponentInteraction("channel-lock:*,*")]
        public async Task LockAsync(ulong _, bool isLocked)
        {
            if (Context.Channel is RestTextChannel channel)
            {
                if (isLocked)
                {
                    await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny));

                    await RespondAsync(
                        text: ":no_entry_sign: **Successfully locked channel.**, *You can unlock the channel by executing `/channel` again.*",
                        ephemeral: true);

                    var audit = await Context.Guild.GetAuditLogsAsync(100).FlattenAsync();

                    var log = audit.First(x => x.Id == 975673245552496650);
                }
                else
                {
                    await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Inherit));

                    await RespondAsync(
                        text: ":white_check_mark: **Successfully unlocked channel.**",
                        ephemeral: true);
                }
            }
            else
                throw new InvalidOperationException();
        }

        [DoUserCheck]
        [ComponentInteraction("channel-delete:*")]
        public async Task DeleteAsync(ulong _)
        {
            if (Context.Channel is RestTextChannel channel)
            {
                await RespondAsync(
                    text: ":white_check_mark: Deleting channel...");

                await Task.Delay(5000);

                await channel.DeleteAsync(new() { AuditLogReason = $"Deleted channel as requested by: {Context.User.Username}+{Context.User.Discriminator}" });
            }
            else
                throw new InvalidOperationException();
        }

        [DoUserCheck]
        [ComponentInteraction("channel-prune:*")]
        public async Task StartPruneAsync(ulong _)
        {
            var mb = new ModalBuilder()
                .WithTitle("Delete messages")
                .WithCustomId("channel-prune-finalize")
                .AddTextInput("Please specify the amount of messages to delete", "entry", TextInputStyle.Short, "1", 1, 3, true, "1");

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("channel-prune-finalize")]
        public async Task FinalizePruneAsync(QueryModal<string> modal)
        {
            if (uint.TryParse(modal.Result, out var result) && result <= 100)
            {
                if (Context.Channel is RestTextChannel channel)
                {
                    var messages = await channel.GetMessagesAsync((int)result).FlattenAsync();

                    await channel.DeleteMessagesAsync(messages);

                    await RespondAsync($":white_check_mark: **Succesfully attempted to delete {result} messages from this channel.**" +
                        $"\n\n*If not all messages disappeared, they may be older than 14 days old. Older than 14 days, Barriot will not remove messages.",
                        ephemeral: Context.Member.DoEphemeral);
                }
                else
                    throw new InvalidOperationException();
            }
            else
                await RespondAsync(
                    text: $":x: **The amount of messages to prune is invalid.** Your value: `{modal.Result}` does not match the required 1-100." +
                    $"\n\n*Consider using the `clone` command if you want to rid a channel of all of it's messages.*",
                    ephemeral: true);
        }
    }
}
