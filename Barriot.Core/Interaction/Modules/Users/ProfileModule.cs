using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class ProfileModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;

        public ProfileModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [SlashCommand("profile", "Views your or another user's statistics.")]
        public async Task SlashStatisticsAsync(RestUser? user = null)
            => await StatisticsAsync(user ?? Context.User);

        [UserCommand("Profile")]
        public async Task StatisticsAsync(RestUser user)
        {
            bool isSelfUser = user.Id == Context.User.Id;

            var member = isSelfUser ? Context.Member : await UserEntity.GetAsync(user.Id);

            var bumps = await BumpsEntity.GetAsync(user.Id);

            var eb = new EmbedBuilder();
            var cb = new ComponentBuilder();

            if (!isSelfUser)
            {
                var bumper = await BumpsEntity.GetAsync(Context.User.Id);

                if (bumper.BumpsToGive > 0 || (await Context.Client.GetApplicationInfoAsync()).Owner.Id == Context.User.Id)
                    cb.WithButton("Bump this user!", $"bump:{Context.User.Id},{user.Id}", ButtonStyle.Primary);
            }

            cb.WithButton("View stats", $"stats:{Context.User.Id},{user.Id}");
            cb.WithButton("View acknowledgements", $"ack:{Context.User.Id},{user.Id}");

            eb.AddField("Bumps:", $"Received ` {bumps.ReceivedBumps} `", true);
            eb.AddField("_ _", $"` {bumps.BumpsToGive} ` bumps to give out.", true);

            if (member.Flags.Any())
            {
                eb.AddField("Acknowledgements:", $"Owns ` {member.Flags.Count} ` acknowledgements in total.");

                if (member.FeaturedFlag is not null)
                    eb.AddField("Featured acknowledgement:", $"**{member.FeaturedFlag}**\n > {member.FeaturedFlag.Description}");
            }

            if (member.Votes > 0)
            {
                eb.AddField("Votes:", $"Voted ` {member.Votes} ` times in total.", true);

                if (member.MonthlyVotes > 0)
                    eb.AddField("_ _", $"` {member.MonthlyVotes} ` times in the last month.", true);
            }

            await RespondAsync(
                format: "bust_in_silhouette",
                header: $"{user.Username}#{user.Discriminator}'s profile:",
                embed: eb,
                components: cb);
        }

        [DoUserCheck]
        [ComponentInteraction("stats:*,*")]
        public async Task Stats(ulong _, ulong targetId)
        {
            var cb = new ComponentBuilder()
                .WithButton("View acknowledgements", $"ack:{Context.User.Id},{targetId}");

            if (Context.Member.Flags.Any(x => x.Type is FlagType.Developer))
            {
                if (targetId != Context.User.Id)
                    cb.WithButton("Blacklist user", $"blacklist:{Context.User.Id},{targetId}", ButtonStyle.Danger);
            }

            var target = await UserEntity.GetAsync(targetId);

            var eb = new EmbedBuilder()
                .AddField("Latest command", $"` {target.LastCommand} `")
                .AddField("Commands", $"Executed ` {target.CommandsExecuted} ` command{((target.CommandsExecuted != 1) ? "s" : "")}.")
                .AddField("Components", $"Clicked ` {target.ButtonsPressed} ` component{((target.ButtonsPressed != 1) ? "s" : "")}.")
                .AddField("Challenges", $"Won ` {target.GamesWon} ` challenge{((target.GamesWon != 1) ? "s" : "")}.");

            await UpdateAsync(
                format: "bar_chart",
                header: $"<@{targetId}>'s Barriot command statistics:",
                embed: eb,
                components: cb);
        }

        [DoUserCheck]
        [ComponentInteraction("ack:*,*")]
        public async Task AckmowledgementsAsync(ulong _, ulong targetId)
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

            var cb = new ComponentBuilder()
                .WithButton("View stats", $"stats:{Context.User.Id}:{targetId}");

            if ((await Context.Client.GetApplicationInfoAsync()).Owner.Id == Context.User.Id)
            {
                cb = new();
                cb.WithButton("Add acknowledgement", $"flag-creating:{Context.User.Id},{targetId}", ButtonStyle.Success);
                cb.WithButton("Remove acknowledgement(s)", $"flag-deleting:{Context.User.Id},{targetId}", ButtonStyle.Danger);
            }

            await UpdateAsync(
                format: "medal",
                header: $"<@{targetId}>'s Acknowledgements.",
                context: "Rewarded for contributions, regular use of the bot, donations and more.",
                embed: eb,
                components: cb);
        }

        [DoUserCheck]
        [ComponentInteraction("bump:*,*")]
        public async Task BumpAsync(ulong _, ulong targetId)
        {
            var target = await BumpsEntity.GetAsync(targetId);
            var origin = await BumpsEntity.GetAsync(Context.User.Id);

            target.ReceivedBumps++;

            if (origin.BumpsToGive > 0)
                origin.BumpsToGive--;

            bool canGiveMore = origin.BumpsToGive is > 0;

            ComponentBuilder? cb = null;
            if (canGiveMore)
                cb = new ComponentBuilder().WithButton("Bump again", $"bump:{Context.User.Id}:{targetId}", ButtonStyle.Success);

            if (!Context.Member.HasVoted())
            {
                if (cb is null)
                    cb = new ComponentBuilder();

                cb.WithButton("Vote to get more bumps", style: ButtonStyle.Link, url: _configuration["Domain"] + "vote");
            }

            await UpdateAsync(
                format: "thumbsup",
                header: $"Succesfully bumped <@{targetId}>!",
                context: canGiveMore
                    ? "Click on the button below to bump again!"
                    : null,
                components: cb);
        }
    }
}
