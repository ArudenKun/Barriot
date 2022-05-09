using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class SettingsModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;

        public SettingsModule(IConfiguration config)
        {
            _configuration = config;
        }

        [SlashCommand("settings", "Change your own Barriot settings.")]
        public async Task SettingsAsync()
        {
            var eb = new EmbedBuilder()
                .WithColor(Context.UserData.Color)
                .AddField("Current embed color", new Color(Context.UserData.Color).ToString())
                .AddField("Hiding commands", Context.UserData.DoEphemeral);

            var cb = new ComponentBuilder()
                .WithButton($"{(Context.UserData.DoEphemeral ? "Unhide" : "Hide")} commands", $"command-toggle:{Context.User.Id}", style: Context.UserData.DoEphemeral ? ButtonStyle.Danger : ButtonStyle.Success);

            if (Context.UserData.HasVoted())
                cb.WithButton($"Set custom embed color", $"embed-creating:{Context.User.Id}");

            else
            {
                eb.WithFooter("Vote for Barriot to modify your embed color!");
                cb.WithButton("Vote now!", style: ButtonStyle.Link, url: _configuration["Domain"] + "vote");
            }

            await RespondAsync(
                text: ":gear: **Your personal settings.** *A range of buttons is defined to modify any setting.*",
                embed: eb.Build(),
                components: cb.Build(),
                ephemeral: Context.UserData.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("command-toggle:*")]
        public async Task CommandSettingAsync(ulong _)
        {
            Context.UserData.DoEphemeral = !Context.UserData.DoEphemeral;

            await RespondAsync(
                text: Context.UserData.DoEphemeral
                    ? ":white_check_mark: **Commands are now hidden.** Commands you execute will now be hidden for everyone but you."
                    : ":no_entry_sign: **Commands are now openly displayed.** Commands you execute will now be visible to everyone.",
                ephemeral: Context.UserData.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("embed-creating:*")]
        public async Task EmbedSettingAsync(ulong _)
            => await RespondWithModalAsync<EmbedColorModal>("embed-created");

        [ModalInteraction("embed-created")]
        public async Task FinalizeEmbedSettingAsync(EmbedColorModal modal)
        {
            if (modal.Color == Color.Default)
            {
                await RespondAsync(
                    text: ":x: **The color you supplied is an invalid color!** Please define exclusively hex-color codes like '#ffffff'.",
                    ephemeral: true);
            }

            else
            {
                Context.UserData.Color = modal.Color;

                var eb = new EmbedBuilder()
                    .WithColor(modal.Color)
                    .WithDescription("*This is an example embed to show your new embed color.*");

                await RespondAsync(
                    text: $":art: **Successfully changed your embed color to {modal.Color}!** All embeds will now display in this color.",
                    embed: eb.Build(),
                    ephemeral: Context.UserData.DoEphemeral);
            }
        }
    }
}
