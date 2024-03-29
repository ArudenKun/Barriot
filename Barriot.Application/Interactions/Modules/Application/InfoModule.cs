﻿using Barriot.Extensions;
using Barriot.Application.Interactions.Attributes;
using Barriot.Application.Services;

namespace Barriot.Application.Interactions.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class InfoModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;
        private readonly InfoService _service;

        public InfoModule(IConfiguration configuration, InfoService service, ILogger<BarriotModuleBase> logger) : base(logger)
        {
            _configuration = configuration;
            _service = service;
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
