using Barriot.Extensions;
using Barriot.Interaction.Attributes;
using Barriot.Models.Files;
using Barriot.Pagination;
using MongoDB.Bson;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class ReminderModule : BarriotModuleBase
    {
        [SlashCommand("remind", "Creates a reminder for a set time with provided message.")]
        public async Task RemindAsync(
            [Summary("time", "The time until this reminder")] TimeSpan spanUntil,
            [Summary("message", "What you want to be reminded about")] string message,
            [Summary("frequency", "How many times this reminder should be repeated")] int frequency = 1,
            [Summary("span", "What the time between each repetition should be")] TimeSpan? timeBetween = null)
        {
            if (frequency <= 0)
                frequency = 1;

            if (spanUntil == TimeSpan.Zero)
                await RespondAsync(
                    text: FileExtensions.GetError(ErrorInfo.InvalidTimeSpan, "time until this reminder is sent"),
                    ephemeral: true);

            else if (timeBetween is null && frequency > 1)
                await RespondAsync(
                    text: FileExtensions.GetError(ErrorInfo.InvalidTimeSpan, "time between reminders"),
                    ephemeral: true);

            else if (timeBetween is not null && timeBetween?.TotalMinutes < 5d)
                await RespondAsync(
                    text: ":x: **The timespan can not be shorter than 5 minutes!**",
                    ephemeral: true);

            else
            {
                if (!Context.Interaction.IsDMInteraction)
                {
                    try
                    {
                        var embed = new EmbedBuilder()
                            .WithDescription(FileExtensions.GetEmbedContent(EmbedInfo.ReminderCheckUp))
                            .WithFooter("Make sure you keep your DM's open to receive it!")
                            .WithColor(new Color(Context.Member.Color));

                        await Context.User.SendMessageAsync(
                            text: ":wave: **Hi, just checking up on you!**",
                            embed: embed.Build());
                    }
                    catch
                    {
                        await RespondAsync(
                            text: $":x: **Reminder creation failed!** {FileExtensions.GetError(ErrorInfo.ReminderSendFailed)}",
                            ephemeral: true);
                        return;
                    }
                }

                await RemindEntity.CreateAsync(message, spanUntil, Context.User.Id, frequency, timeBetween);

                await RespondAsync(
                    text: $":thumbsup: **Got it!** I will remind you to {message} in {spanUntil.ToReadable()}" +
                    $"{((frequency > 1) ? $"\n\n> This reminder will repeat {frequency} time(s) every {timeBetween?.ToReadable()}." : "")}",
                    ephemeral: Context.Member.DoEphemeral);
            }
        }

        [SlashCommand("reminders", "Lists your current reminders.")]
        public async Task ListRemindersAsync([Summary("page", "The reminders page")] int page = 1)
            => await ListRemindersInternal(page);

        [ComponentInteraction("reminders-list:*")]
        public async Task ListRemindersFromExistingAsync(int page)
        {
            await ListRemindersInternal(page);
        }

        private async Task ListRemindersInternal(int page)
        {
            if (page < 1)
                page = 1;

            var reminders = await RemindEntity.GetManyAsync(Context.User);

            if (reminders.Any())
            {
                if (!Paginator<RemindEntity>.TryGet(out var paginator))
                {
                    paginator = new PaginatorBuilder<RemindEntity>()
                        .WithPages(x =>
                        {
                            string sendRepeat = "";
                            if (x.Frequency > 1)
                                sendRepeat = $"\n⤷ *Set to repeat {x.Frequency} more time(s).";
                            return new($"{x.Expiration} (UTC)", x.Message ?? "No message set" + sendRepeat);
                        })
                        .WithCustomId("reminders-list")
                        .Build();
                }
                var value = paginator.GetPage(page, reminders);

                value.Component.WithButton("Delete reminders from this page", $"reminders-deleting:{page}", ButtonStyle.Secondary);

                await RespondAsync(
                    page: value,
                    header: "Your reminders:");
            }
            else
                await RespondAsync(
                    error: "You have no reminders!",
                    context: "Use ` /remind ` to set reminders.");
        }

        [ComponentInteraction("reminders-deleting:*")]
        public async Task DeletingRemindersAsync(int page)
        {
            var selection = await RemindEntity.GetManyAsync(Context.User);

            if (!selection.Any())
                await UpdateAsync(
                    error: "You have no reminders to delete!");

            else
            {
                var sb = new SelectMenuBuilder()
                    .WithMinValues(1)
                    .WithCustomId("reminders-deleted")
                    .WithPlaceholder("Select 1 or more reminders to delete.");

                int index = page * 10 - 10;

                var range = selection.GetRange(index, selection.Count - index);
                for (int i = 0; i < range.Count; i++)
                {
                    if (i == 10)
                        break;
                    sb.AddOption(range[i].Expiration.ToString(), range[i].ObjectId.ToString(), range[i].Message.Reduce(100));
                }

                sb.WithMaxValues(sb.Options.Count);

                var cb = new ComponentBuilder()
                    .WithSelectMenu(sb);

                await UpdateAsync(
                    format: ResultFormat.Deleting,
                    header: "Delete reminders:",
                    context: "Select the reminders you want to delete in the dropdown below.",
                    components: cb.Build());
            }
        }

        [ComponentInteraction("reminders-deleted")]
        public async Task DeletedRemindersAsync(ObjectId[] selectedReminders)
        {
            var selection = await RemindEntity.GetManyAsync(Context.User);

            if (!selection.Any())
                await UpdateAsync(
                    error: "You have no reminders to delete!");

            else
            {
                foreach (var value in selectedReminders)
                {
                    var reminder = selection.First(x => x.ObjectId == value);

                    if (reminder is not null)
                        await reminder.DeleteAsync();
                }
                await UpdateAsync(
                    format: ResultFormat.Success,
                    header: $"Succesfully removed {selectedReminders.Length} reminder(s).");
            }
        }
    }
}
