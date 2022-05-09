using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class StatsModule : BarriotModuleBase
    {
        [SlashCommand("stats", "Views your or another user's statistics.")]
        public async Task SlashStatisticsAsync(RestUser? user = null)
            => await StatisticsAsync(user ?? Context.User);

        [UserCommand("Stats")]
        public async Task StatisticsAsync(RestUser user)
        {
            if (user.Id == Context.Client.CurrentUser.Id)
            {
                var eb = new EmbedBuilder()
                    .WithColor(Context.UserData.Color)
                    .AddField("Votes", $"Voted ` 69 ` times.")
                    .AddField("Commands", $"Executed ` 420 ` commands.")
                    .AddField("Components", $"Clicked ` 1337 ` components.")
                    .AddField("Challenges", $"Won ` every ` challenge.");

                await RespondAsync(
                    text: $":sunglasses: **Barriot's statistics.** *Cool, you found an easter egg!*",
                    embed: eb.Build(),
                    ephemeral: Context.UserData.DoEphemeral);
            }

            else if (user.IsBot || user.IsWebhook)
                await RespondAsync(
                    text: $":x: **This user cannot interact with Barriot!**",
                    ephemeral: Context.UserData.DoEphemeral);

            else
            {
                var cb = new ComponentBuilder()
                    .WithButton("View acknowledgements", $"ack:{Context.User.Id},{user.Id}");

                if (Context.UserData.Flags.Any(x => x.Type is FlagType.Developer))
                {
                    if (user.Id != Context.User.Id)
                        cb.WithButton("Blacklist user", $"blacklist:{Context.User.Id},{user.Id}", ButtonStyle.Danger);
                }

                var target = await UserEntity.GetAsync(user);

                var eb = new EmbedBuilder()
                    .WithColor(Context.UserData.Color)
                    .AddField("Votes", $"Voted ` {target.Votes} ` time{((target.Votes != 1) ? "s" : "")}.")
                    .AddField("Latest command", $"` {target.LastCommand} `")
                    .AddField("Commands", $"Executed ` {target.CommandsExecuted} ` command{((target.CommandsExecuted != 1) ? "s" : "")}.")
                    .AddField("Components", $"Clicked ` {target.ButtonsPressed} ` component{((target.ButtonsPressed != 1) ? "s" : "")}.")
                    .AddField("Challenges", $"Won ` {target.GamesWon} ` challenge{((target.GamesWon != 1) ? "s" : "")}.");

                await RespondAsync(
                    text: $":bar_chart: **<@{user.Id}>'s Barriot statistics**",
                    embed: eb.Build(),
                    components: cb.Build(),
                    ephemeral: Context.UserData.DoEphemeral);
            }
        }

        [DoUserCheck]
        [ComponentInteraction("ack:*,*")]
        public async Task Acknowledgements(ulong userId, ulong targetId)
        {
            var user = await UserEntity.GetAsync(targetId);

            var eb = new EmbedBuilder()
                .WithColor(Color.Gold);

            if (user.Flags.Any())
                foreach (var flag in user.Flags)
                {
                    bool inline = true;
                    switch (flag.Type)
                    {
                        case FlagType.Component:
                        case FlagType.Command:
                        case FlagType.Champion:
                            inline = false;
                            break;
                        default:
                            break;
                    }
                    eb.AddField(flag.ToString(), flag.Description, inline);
                }
            else
                eb.WithDescription("This user has no acknowledgments.");

            ComponentBuilder? cb = null;
            if ((await Context.Client.GetApplicationInfoAsync()).Owner.Id == Context.User.Id)
            {
                cb = new();
                cb.WithButton("Add acknowledgement", $"flag-creating:{userId},{targetId}", ButtonStyle.Success);
                cb.WithButton("Remove acknowledgement(s)", $"flag-deleting:{userId},{targetId}", ButtonStyle.Danger);
            }

            await RespondAsync(
                text: $":medal: **<@{targetId}>'s Acknowledgements.** Rewarded for contributions, regular use of the bot, donations and more.",
                embed: eb.Build(),
                components: cb?.Build(),
                ephemeral: Context.UserData.DoEphemeral);
        }
    }
}
