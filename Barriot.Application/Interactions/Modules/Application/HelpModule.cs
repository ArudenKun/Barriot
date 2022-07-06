using Barriot.Application.Interactions.Attributes;
using Barriot.Extensions;

namespace Barriot.Application.Interactions.Modules
{
    [IgnoreBlacklistedUsers]
    public class HelpModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;

        public HelpModule(IConfiguration config, ILogger<BarriotModuleBase> logger) : base(logger)
        {
            _configuration = config;
        }

        [SlashCommand("help", "Barriot help & Support.")]
        public async Task HelpAsync()
        {
            var sb = new SelectMenuBuilder()
                .WithMaxValues(1)
                .WithMinValues(1)
                .WithCustomId($"tutorial:{Context.User.Id}")
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
                .WithUrl(_configuration["Domain"] + "commands")
                .WithTitle("Click here to view all commands!")
                .WithThumbnailUrl(_configuration["Domain"] + "files/B_monogram.png")
                .AddField("Commands", "Click on the buttons below to navigate through command tutorials!")
                .AddField("Note", "Discord currently does not support Message or User commands on mobile devices!")
                .WithDescription(string.Join("\n", FileExtensions.GetDataFromFile("HelpText").Lines));

            await RespondAsync(
                text: ":question: **Barriot help & support**",
                components: cb.Build(),
                embed: eb.Build(),
                ephemeral: Context.Member.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("tutorial:*")]
        public async Task TutorialAsync(ulong _, string selectedValues)
        {
            await UpdateAsync(
                text: _configuration["Domain"] + $"files/BarriotAnim{selectedValues[0]}.gif");
        }
    }
}
