using Barriot.Extensions;
using Barriot.Interaction.Attributes;
using Barriot.Interaction.Services;

namespace Barriot.Interaction.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class InfoModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;
        private readonly InfoService _service;

        public InfoModule(IConfiguration configuration, InfoService service)
        {
            _configuration = configuration;
            _service = service;
        }

        [SlashCommand("help", "Barriot help & Support.")]
        public async Task HelpAsync()
        {
            var sb = new SelectMenuBuilder()
                .WithMaxValues(1)
                .WithMinValues(1)
                .WithCustomId($"help:{Context.User.Id}")
                .AddOption("Slash commands", "1")
                .AddOption("User commands", "2")
                .AddOption("Message commands", "3")
                .WithPlaceholder("How to: Commands");

            var cb = new ComponentBuilder()
                .WithSelectMenu(sb)
                .WithButton("Get support", style: ButtonStyle.Link, url: _configuration["Domain"] + "discord", row: 1)
                .WithButton("Invite Barriot", style: ButtonStyle.Link, url: _configuration["Domain"] + "invite", row: 1)
                .WithButton("Privacy Policy", style: ButtonStyle.Link, url: _configuration["Domain"] + "privacy", row: 1);

            var eb = new EmbedBuilder()
                .WithColor(Context.Member.Color)
                .WithThumbnailUrl("https://rozen.one/Files/B_monogram.png")
                .AddField("Commands", "Click on the buttons below to navigate through command examples & to get further support if required!")
                .AddField("Note", "Discord currently does not support Message or User commands on mobile devices!")
                .WithDescription(string.Join("\n", FileExtensions.GetDataFromFile("HelpText").Lines));

            await RespondAsync(
                text: ":question: **Barriot help & support**",
                components: cb.Build(),
                embed: eb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("help:*")]
        public async Task HelpAsync(ulong _, string[] selectedExample)
        {
            await RespondAsync(
                text: $"https://rozen.one/Files/" + $"BarriotAnim{selectedExample[0]}.gif",
                ephemeral: true);
        }

        [SlashCommand("vote", "Vote for Barriot!")]
        public async Task VoteAsync()
        {
            var cb = new ComponentBuilder()
                .WithButton("Click here to vote!", style: ButtonStyle.Link, url: _configuration["Domain"] + "vote");

            await RespondAsync(
                text: ":heart: **Vote for Barriot through the link below!**",
                components: cb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("changelog", "Views current Barriot version & changelog.")]
        public async Task ChangelogAsync()
        {
            var eb = new EmbedBuilder()
                .WithColor(Context.Member.Color)
                .WithDescription(string.Join("\n", FileExtensions.GetDataFromFile("Changelog").Lines));

            var cb = new ComponentBuilder()
                .WithButton("Get new version notifications", style: ButtonStyle.Link, url: _configuration["Domain"] + "discord")
                .WithButton("Invite Barriot", style: ButtonStyle.Link, url: _configuration["Domain"] + "invite")
                .WithButton("Privacy Policy", style: ButtonStyle.Link, url: _configuration["Domain"] + "privacy");

            await RespondAsync(
                text: $":newspaper: **Changelog for version {typeof(Program).Assembly.GetName().Version}**",
                embed: eb.Build(),
                components: cb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("invite", "Invite Barriot to your own servers.")]
        public async Task InviteAsync()
        {
            var cb = new ComponentBuilder()
                .WithButton("Invite Barriot", style: ButtonStyle.Link, url: _configuration["Domain"] + "invite");

            await RespondAsync(
                text: ":chart_with_upwards_trend: **Invite Barriot through the button below!**",
                components: cb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("about", "Barriot statistics and more!")]
        public async Task AboutAsync()
        {
            var cb = new ComponentBuilder()
                .WithButton("View total guild count", $"data-stats:{Context.User.Id}")
                .WithButton("Total uptime", $"data-uptime:{Context.User.Id}");

            await RespondAsync(
                text: ":robot: **Everything about Barriot!**",
                components: cb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("data-stats:*")]
        public async Task StatsAsync(ulong _)
        {
            var eb = new EmbedBuilder()
                .WithColor(Context.Member.Color)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .AddField("Total Guilds", _service.GuildCount);

            await RespondAsync(
                text: ":bar_chart: **Guild count** This may not be fully accurate, as this data is not updated continuously.",
                embed: eb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("data-uptime:*")]
        public async Task UptimeAsync(ulong _)
        {
            var eb = new EmbedBuilder()
                .WithColor(Context.Member.Color)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .AddField("Start Time", _service.OnlineSince)
                .AddField("Total Uptime", DateTime.UtcNow - _service.OnlineSince);

            await RespondAsync(
                text: ":clock1: **Uptime & start time.** This information is in UTC.",
                embed: eb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

    }
}
