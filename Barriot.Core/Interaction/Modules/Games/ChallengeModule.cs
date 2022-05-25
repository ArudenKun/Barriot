using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class ChallengeModule : BarriotModuleBase
    {
        [SlashCommand("challenge", "Challenges another user to a minigame.")]
        public async Task SlashChallengeAsync([Summary("target", "The user to target!")] RestUser user)
            => await ChallengeAsync(user);

        [UserCommand("challenge")]
        public async Task ChallengeAsync(RestUser target)
        {
            if (target.IsBot || target.IsWebhook)
                await RespondAsync(
                    text: ":x: **You cannot challenge bots!**",
                    ephemeral: Context.Member.DoEphemeral);

            else if (target.Id == Context.User.Id)
                await RespondAsync(
                    text: ":x: **You cannot challenge yourself!**",
                    ephemeral: Context.Member.DoEphemeral);

            else
            {
                var cb = new ComponentBuilder()
                    .WithButton("Tic Tac Toe", $"challenge-s:{Context.User.Id},{target.Id},ttt", ButtonStyle.Primary)
                    .WithButton("Connect 4", $"challenge-s:{Context.User.Id},{target.Id},con", ButtonStyle.Primary)
                    .WithButton("Nevermind", $"challenge-n:{Context.User.Id}", ButtonStyle.Danger);

                await RespondAsync(
                    text: $":video_game: **What game do you want to play against {target.Username}#{target.Discriminator}?**",
                    components: cb.Build(),
                    ephemeral: false);
            }
        }

        [DoUserCheck]
        [DisableSource]
        [ComponentInteraction("challenge-d:*,*")]
        public async Task DeniedChallengeAsync(ulong userId, ulong targetId)
        {
            await RespondAsync(
                text: $":x: **<@{targetId}>! <@{userId}> has denied your challenge.**",
                ephemeral: false);
        }

        [DoUserCheck]
        [DisableSource]
        [ComponentInteraction("challenge-n:*")]
        public async Task NevermindChallengeAsync(ulong _)
        {
            await RespondAsync(
                text: $":x: **Okay, challenge ignored!**",
                ephemeral: true);
        }

        [DoUserCheck]
        [DisableSource]
        [ComponentInteraction("challenge-f:*,*")]
        public async Task ForfeitChallengeAsync(ulong _, ulong targetId)
        {
            using var tUser = await UserEntity.GetAsync(targetId);
            tUser.GamesWon++;

            await RespondAsync(
                text: $":x: **Quit challenge!** <@{targetId}> will be rewarded because of your forfeit.",
                ephemeral: false);
        }

        [DoUserCheck]
        [DisableSource]
        [ComponentInteraction("challenge-s:*,*,*")]
        public async Task StartChallengeAsync(ulong userId, ulong targetId, string type)
        {
            string[] gameData;
            switch (type)
            {
                case "ttt":
                    gameData = new string[]
                    {
                        "tictactoe-init",
                        "Tic Tac Toe"
                    };
                    break;
                case "con":
                    gameData = new string[]
                    {
                        "connect-init",
                        "Connect 4"
                    };
                    break;
                default:
                    return;
            }
            var cb = new ComponentBuilder()
                .WithButton("Yes!", $"{gameData[0]}:{targetId},{userId}", ButtonStyle.Success)
                .WithButton("Nope.", $"challenge-d:{targetId},{userId}", ButtonStyle.Danger);

            await RespondAsync(
                text: $":crossed_swords: **<@{targetId}>! You have been challenged by <@{userId}> to a game of {gameData[1]}!** Are you up to the challenge?",
                components: cb.Build(),
                ephemeral: false);
        }
    }
}
