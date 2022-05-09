using Barriot.Extensions;
using Barriot.Extensions.Files;
using Barriot.Interaction.Attributes;
using Barriot.Extensions.Pagination;

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
                    text: FileHelper.GetErrorFromFile(ErrorType.InvalidTimeSpan, "time until this reminder is sent"),
                    ephemeral: true);

            else if (timeBetween is null && frequency > 1)
                await RespondAsync(
                    text: FileHelper.GetErrorFromFile(ErrorType.InvalidTimeSpan, "time between reminders"),
                    ephemeral: true);

            else if (timeBetween is not null && timeBetween?.TotalMinutes < 5d)
                await RespondAsync(
                    text: ":x: **The timespan can not be shorter than 5 minutes!**",
                    ephemeral: true);

            else
            {
                try
                {
                    var embed = new EmbedBuilder()
                        .WithDescription(FileHelper.GetInfoFromFile(InfoType.ReminderCheckUp))
                        .WithFooter("Make sure you keep your DM's open to receive it!")
                        .WithColor(new Color(Context.UserData.Color));

                    await Context.User.SendMessageAsync(
                        text: ":wave: **Hi, just checking up on you!**",
                        embed: embed.Build());

                    await RemindEntity.CreateAsync(message, spanUntil, Context.User.Id, frequency, timeBetween);

                    await RespondAsync(
                        text: $":thumbsup: **Got it!** I will remind you to {message} in {spanUntil.ToReadable()}" +
                        $"{((frequency > 1) ? $"\n\n> This reminder will repeat {frequency} time(s) every {timeBetween?.ToReadable()}." : "")}",
                        ephemeral: Context.UserData.DoEphemeral);
                }
                catch
                {
                    await RespondAsync(
                        text: $":x: **Reminder creation failed!** {FileHelper.GetErrorFromFile(ErrorType.ReminderSendFailed)}",
                        ephemeral: true);
                }
            }
        }

        [SlashCommand("reminders", "Lists your current reminders.")]
        public async Task ListRemindersAsync([Summary("page", "The reminders page")] int page = 1)
        {
            var reminders = (await RemindEntity.GetManyAsync(Context.User)).ToEnumerable().ToList();

            if (reminders.Any())
            {
                if (!Paginator<RemindEntity>.TryGet(out var paginator))
                {
                    paginator = new PaginatorBuilder<RemindEntity>()
                        .WithEmbed(new EmbedBuilder()
                            .WithColor(new Color(Context.UserData.Color)))
                        .WithPages(x =>
                        {
                            string sendRepeat = "";
                            if (x.Frequency > 1)
                                sendRepeat = $"\n⤷ *Set to repeat {x.Frequency} more time(s).";
                            return new($"{x.Expiration} (UTC)", x.Message ?? "No message set" + sendRepeat);
                        })
                        .WithCustomId($"reminders-list:{Context.User.Id}")
                        .WithComponents(new ComponentBuilder()
                            .WithButton("Delete reminders", $"reminders-deleting:{Context.User.Id}", ButtonStyle.Secondary))
                        .Build();
                }
                var value = paginator.GetPage(page, reminders);

                await RespondAsync(
                    text: $":page_facing_up: **Your reminders:** You are able to set a total of {25} reminders, and are currently able to add {25 - reminders.Count} more.",
                    embed: value.Embed,
                    components: value.Component,
                    ephemeral: Context.UserData.DoEphemeral);
            }
            else
                await RespondAsync(
                    text: $":x: **You have no reminders!** Use ` /remind ` to set reminders.",
                    ephemeral: true);
        }

        [DoUserCheck]
        [ComponentInteraction("reminders-list:*,*")]
        public async Task ListRemindersAsync(ulong _, int page)
        {
            var reminders = (await RemindEntity.GetManyAsync(Context.User)).ToEnumerable().ToList();

            if (reminders.Any())
            {
                if (!Paginator<RemindEntity>.TryGet(out var paginator))
                {
                    paginator = new PaginatorBuilder<RemindEntity>()
                        .WithEmbed(new EmbedBuilder()
                            .WithColor(new Color(Context.UserData.Color)))
                        .WithPages(x =>
                        {
                            string sendRepeat = "";
                            if (x.Frequency > 1)
                                sendRepeat = $"\n⤷ *Set to repeat {x.Frequency} more time(s).";
                            return new($"{x.Expiration} (UTC)", x.Message ?? "No message set" + sendRepeat);
                        })
                        .WithCustomId($"reminders-list:{Context.User.Id}")
                        .WithComponents(new ComponentBuilder()
                            .WithButton("Delete reminders", $"reminders-deleting:{Context.User.Id}", ButtonStyle.Secondary))
                        .Build();
                }
                var value = paginator.GetPage(page, reminders);

                await RespondAsync(
                    text: $":page_facing_up: **Your reminders:** You are able to set a total of {25} reminders, and are currently able to add {25 - reminders.Count} more.",
                    embed: value.Embed,
                    components: value.Component,
                    ephemeral: Context.UserData.DoEphemeral);
            }
            else
                await RespondAsync(
                    text: $":x: **You have no reminders!** Use ` /remind ` to set reminders.",
                    ephemeral: true);
        }

        [DoUserCheck]
        [ComponentInteraction("reminders-deleting:*")]
        public async Task DeletingRemindersAsync(ulong _)
        {
            var selection = (await RemindEntity.GetManyAsync(Context.User))
                .ToEnumerable()
                .ToList();

            if (!selection.Any())
                await RespondAsync(
                    text: ":x: **You have no reminders to delete!**",
                    ephemeral: true);

            else
            {
                var sb = new SelectMenuBuilder()
                    .WithMinValues(1)
                    .WithMaxValues(selection.Count)
                    .WithCustomId($"reminders-deleted:{Context.User.Id}")
                    .WithPlaceholder("Select 1 or more reminders to delete.");

                for (int i = 0; i < selection.Count; i++)
                    sb.AddOption(selection[i].Expiration.ToString(), i.ToString(), selection[i].Message.Reduce(100));

                var cb = new ComponentBuilder()
                    .WithSelectMenu(sb);

                await RespondAsync(
                    text: ":wastebasket: **Delete reminders:** *Select the reminders you want to delete in the dropdown below.*",
                    components: cb.Build(),
                    ephemeral: Context.UserData.DoEphemeral);
            }
        }

        [DoUserCheck]
        [ComponentInteraction("reminders-deleted:*")]
        public async Task DeletedRemindersAsync(ulong _, string[] selectedReminders)
        {
            var selection = (await RemindEntity.GetManyAsync(Context.User)).ToEnumerable().ToList();

            if (!selection.Any())
                await RespondAsync(text: ":x: **You have no reminders to delete!**",
                    ephemeral: true);

            else
            {
                foreach (var value in selectedReminders)
                {
                    if (!int.TryParse(value, out int parsed))
                        continue;

                    else
                        await selection[parsed].DeleteAsync();
                }
                await RespondAsync(
                    text: $":white_check_mark: **Succesfully removed {selectedReminders.Length} reminder(s).",
                    ephemeral: Context.UserData.DoEphemeral);
            }
        }
    }
}
