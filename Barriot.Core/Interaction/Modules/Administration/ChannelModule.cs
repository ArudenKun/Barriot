using Barriot.Extensions;
using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
    [EnabledInDm(false)]
    [RequireBotPermission(ChannelPermission.ManageChannels)]
    [IgnoreBlacklistedUsers]
    public class ChannelModule : BarriotModuleBase
    {
        [Group("channel", "Manages the current channel")]
        public class NestedChannelModule : BarriotModuleBase
        {
            [SlashCommand("lock", "Locks or unlocks this channel.")]
            public async Task LockAsync(
                [Summary("announce", "True to announce the (un)lock. False to silently (un)lock.")] bool announceLock = true)
            {
                await Task.CompletedTask;
            }

            [SlashCommand("prune", "Prunes a set count of messages from this channel. (Max 100)")]
            public async Task PruneAsync(
                [Summary("count", "The amount of messages to prune (max 100)")] int messageCount,
                [Summary("reason", "The reason why these messages will be pruned.")] string? reason = null)
            {
                if (messageCount is <= 0 or > 100)
                {
                    await RespondAsync(
                        text: $":x: **The amount of messages to prune is invalid.** Your value: {messageCount} does not match the required 1-100." +
                        $"\n\n*Consider using the `clone` command if you want to rid a channel of all of it's messages.*",
                        ephemeral: true);
                }

                else
                {
                    var logMessage = ModerationExtensions.FormatLogReason(Context.User.ToString(), reason);

                    if (Context.Channel is RestTextChannel channel) // It will be, but lets safety cast to prevent stupid format warnings.
                    {
                        var messages = await channel.GetMessagesAsync(messageCount).FlattenAsync();

                        await channel.DeleteMessagesAsync(messages, new()
                        {
                            AuditLogReason = logMessage
                        });
                    }

                    var response = string.IsNullOrEmpty(reason)
                        ? "The messages have been deleted with no reason provided.."
                        : $"\n\n> **Reason:** {reason}";
                    await RespondAsync($":white_check_mark: **Succesfully deleted {messageCount} messages from this channel.** {response}" +
                        $"\n\n*If not all messages disappeared, they may be older than 14 days old. Older than 14 days, Barriot will not remove messages.",
                        ephemeral: Context.UserData.DoEphemeral);
                }
            }

            [SlashCommand("clone", "Creates an empty clone of the channel in the same position, and deletes the current one.")]
            public async Task CloneAsync(
                [Summary("reason", "The reason why this channel is being cleared.")] string? reason = null)
            {
                if (Context.Channel is RestTextChannel channel)
                {
                    var logMessage = ModerationExtensions.FormatLogReason(Context.User.ToString(), reason);

                    await RespondAsync(
                        text: ":x: **Cloning channel...**");

                    var pinnedMessages = await channel.GetPinnedMessagesAsync();

                    await channel.DeleteAsync(new()
                    {
                        AuditLogReason = logMessage
                    });

                    var clone = await Context.Guild.CreateTextChannelAsync(channel.Name, x =>
                    {
                        x.PermissionOverwrites = channel.PermissionOverwrites.ToList();
                        x.CategoryId = channel.CategoryId;
                        x.Position = channel.Position;
                        x.Topic = channel.Topic;
                        x.SlowModeInterval = channel.SlowModeInterval;
                        x.IsNsfw = channel.IsNsfw;
                    },
                    new()
                    {
                        AuditLogReason = logMessage
                    });
                }
                else
                    throw new NotSupportedException(); // Not possible
            }
        }
    }
}
