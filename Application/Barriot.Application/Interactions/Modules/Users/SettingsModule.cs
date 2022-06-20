using Barriot.Application.Interactions.Attributes;
using Barriot.Application.Interactions.Modals;
using Barriot.Extensions;

namespace Barriot.Application.Interactions.Modules
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
                .AddField("Current embed color", new Color(Context.Member.Color).ToString());

            var cb = new ComponentBuilder()
                .WithButton($"{(Context.Member.DoEphemeral ? "Unhide" : "Hide")} commands", $"command-toggle:{Context.User.Id}", Context.Member.DoEphemeral ? ButtonStyle.Danger : ButtonStyle.Success);

            if (Context.Member.Flags.Any())
            {
                eb.AddField("Featured acknowledgement:", Context.Member.FeaturedFlag is null ? "None" : $"**{Context.Member.FeaturedFlag}**\n> {Context.Member.FeaturedFlag.Description}");
                cb.WithButton("Change Featured", $"flag-setting:{Context.User.Id}", ButtonStyle.Secondary);
            }
                
            eb.AddField("Hiding commands", Context.Member.DoEphemeral);

            if (Context.Member.HasVoted())
                cb.WithButton($"Set custom embed color", $"embed-creating:{Context.User.Id}");

            else
            {
                eb.WithFooter("Vote for Barriot to modify your embed color!");
                cb.WithButton("Vote now!", style: ButtonStyle.Link, url: _configuration["Domain"] + "vote");
            }

            await RespondAsync(
                format: "gear",
                header: "Your personal settings.",
                context: "A range of buttons is defined to modify any setting.",
                embed: eb,
                components: cb);
        }

        [DoUserCheck]
        [ComponentInteraction("command-toggle:*")]
        public async Task CommandSettingAsync(ulong _)
        {
            Context.Member.DoEphemeral = !Context.Member.DoEphemeral;

            var tb = new TextBuilder();

            if (Context.Member.DoEphemeral)
                tb.WithResult(MessageFormat.Success)
                    .WithContext("Commands you execute will now be hidden for everyone but you.")
                    .WithHeader("Commands are now hidden.");

            else
                tb.WithResult(MessageFormat.NotAllowed)
                    .WithContext("Commands you execute will now be visible to everyone.")
                    .WithHeader("Commands are now openly displayed.");

            await UpdateAsync(
                text: tb.Build());
        }

        [DoUserCheck]
        [ComponentInteraction("flag-setting:*")]
        public async Task FlagSettingAsync(ulong _)
        {
            var sb = new SelectMenuBuilder()
                .WithMinValues(1)
                .WithMaxValues(1)
                .WithCustomId($"flag-set:{Context.User.Id}")
                .WithPlaceholder("Select an acknowledgement to continue");

            foreach (var flag in Context.Member.Flags)
                sb.AddOption(flag.Title, flag.Emoji.Trim(':'), flag.Description.Reduce(99));

            var cb = new ComponentBuilder()
                .WithSelectMenu(sb);


            await RespondAsync(
                format: MessageFormat.Question,
                header: "Choose an acknowledgement to feature in your profile!",
                components: cb);
        }

        [DoUserCheck]
        [ComponentInteraction("flag-set:*")]
        public async Task FlagSetAsync(ulong _, string[] selectedValues)
        {
            var value = selectedValues[0];

            var flag = Context.Member.Flags.FirstOrDefault(x => x is not null && x.Emoji.Trim(':') == value, null);

            if (flag is null)
                await UpdateAsync(
                    format: MessageFormat.Failure,
                    header: "Something went wrong when trying to match your value!",
                    context: "Please report this behavior to the Developers.");

            else
            {
                Context.Member.FeaturedFlag = flag;

                var eb = new EmbedBuilder()
                    .WithTitle("New featured acknowledgement:")
                    .AddField(flag.ToString(), flag.Description);

                await UpdateAsync(
                    format: MessageFormat.Success,
                    header: "Succesfully set acknowledgement!",
                    embed: eb);
            }
        }

        [DoUserCheck]
        [ComponentInteraction("embed-creating:*")]
        public async Task EmbedSettingAsync(ulong _)
        {
            var mb = new ModalBuilder()
                .WithTitle("Modify your embed color:")
                .WithCustomId("embed-created")
                .AddTextInput("Hex value:", "entry", TextInputStyle.Short, "#11806A", 1, 7, true);

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("embed-created")]
        public async Task FinalizeEmbedSettingAsync(QueryModal<Color> modal)
        {
            if (modal.Result == Color.Default)
                await RespondAsync(
                    error: "The color you supplied is an invalid color!",
                    context: "Please define exclusively hex-color codes like '#ffffff'.");

            else
            {
                Context.Member.Color = modal.Result;

                var eb = new EmbedBuilder()
                    .WithColor(modal.Result)
                    .WithDescription("*This is an example embed to show your new embed color.*");

                await RespondAsync(
                    format: "art",
                    header: $"Successfully changed your embed color to {modal.Result}!",
                    context: "All embeds will now display in this color.",
                    embed: eb);
            }
        }
    }
}
