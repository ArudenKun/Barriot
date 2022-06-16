namespace Server;

public class SocketOperator
{
    private readonly DiscordSocketClient _client;

    private readonly System.Timers.Timer _timer;

    private DateTime _time;

    public SocketOperator(DiscordSocketClient client)
    {
        _client = client;
        _client.MessageReceived += MessageReceived;
        _client.ButtonExecuted += ButtonPressed;

        _time = ConfigureTime(DateTime.UtcNow, new TimeSpan(19, 00, 00));
        _timer = new(5000)
        {
            AutoReset = true,
            Enabled = true
        };
        _timer.Elapsed += OnElapsed!;
        _timer.Start();
    }

    private DateTime ConfigureTime(DateTime time, TimeSpan span, DateTimeKind kind = DateTimeKind.Utc)
    {
        var t = time.Hour > span.Hours
            ? time.AddDays(1)
            : time;
        return new(
            t.Year,
            t.Month,
            t.Day,
            span.Hours,
            span.Minutes,
            span.Seconds,
            kind);
    }

    private async Task ButtonPressed(SocketMessageComponent arg)
    {
        var cb = new ComponentBuilder()
            .WithButton("Stop being reminded", "novote", ButtonStyle.Danger);

        if (arg.User is not SocketGuildUser user)
            return;

        var role = user.Guild.GetRole(Config.Settings.VoteRole);

        switch (arg.Data.CustomId)
        {
            case "vote":
                if (role != null && !user.Roles.Any(x => x == role))
                {
                    await user.AddRoleAsync(role);
                    await arg.RespondAsync(
                        text: ":heart: **You will be reminded to vote every 12 hours!**",
                        components: cb.Build(),
                        ephemeral: true);
                }
                else
                    await arg.RespondAsync(text: ":heart: **You are already being reminded to vote!**",
                        components: cb.Build(),
                        ephemeral: true);
                break;
            case "novote":
                await user.RemoveRoleAsync(role);
                await arg.RespondAsync(
                    text: ":broken_heart: **Okay, I will stop reminding you to vote.** Thank you for contributing!",
                    ephemeral: true);
                break;
        }
    }

    private async void OnElapsed(object sender, System.Timers.ElapsedEventArgs unused)
    {
        if (DateTime.UtcNow > _time)
        {
            _time = DateTime.UtcNow.AddDays(1);

            var cb = new ComponentBuilder()
                .WithButton("Click here to vote!", style: ButtonStyle.Link, url: "https://top.gg/bot/899780741590810645/vote")
                .WithButton("Get daily vote reminders", "vote", ButtonStyle.Success);

            var eb = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithDescription("Vote for Barriot to support development and boost its server count!");

            await _client.GetVoteChannel()
                .SendMessageAsync(
                    text: $":gem: <@&{Config.Settings.VoteRole}> **Vote for Barriot!**",
                    embed: eb.Build(),
                    components: cb.Build());
        }
    }

    private async Task MessageReceived(SocketMessage args)
    {
        if (Config.Settings.ChannelHandleCallback.TryGetValue(args.Channel.Id, out var value))
        {
            if (args is not SocketUserMessage message)
                return;

            foreach (var v in value)
                switch (v.ToLowerInvariant())
                {
                    case "thread":
                        if (message.Channel is SocketTextChannel textChannel)
                            await textChannel.CreateThreadAsync(
                                name: message.Author.ToString(),
                                type: ThreadType.PublicThread,
                                autoArchiveDuration: (textChannel.Guild.PremiumTier == PremiumTier.None)
                                    ? ThreadArchiveDuration.OneDay
                                    : ThreadArchiveDuration.ThreeDays);
                        break;
                    case "reaction":
                        await message.AddReactionsAsync(Config.Settings.Emotes.Select(x => Emote.Parse(x)));
                        break;
                    case "crosspost":
                        await message.CrosspostAsync();
                        break;
                }
        }
    }
}
