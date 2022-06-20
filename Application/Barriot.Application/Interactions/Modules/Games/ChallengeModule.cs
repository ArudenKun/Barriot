using Barriot.Application.Interactions.Attributes;

namespace Barriot.Application.Interactions.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class ChallengeModule : BarriotModuleBase
    {
        [AllowAPI(true)]
        [SlashCommand("challenge", "Challenges another user to a minigame.")]
        public async Task SlashChallengeAsync([Summary("target", "The user to target!")] RestUser user)
            => await ChallengeAsync(user);

        [AllowAPI(true)]
        [UserCommand("Challenge")]
        public async Task ChallengeAsync(RestUser target)
        {
            if (target.IsBot || target.IsWebhook)
                await RespondAsync(
                    error: "You cannot challenge bots!");

            else if (target.Id == Context.User.Id)
                await RespondAsync(
                    error: "You cannot challenge yourself!");

            else
            {
                var cb = new ComponentBuilder()
                    .WithButton("Tic Tac Toe", $"challenge-s:{Context.User.Id},{target.Id},ttt", ButtonStyle.Primary)
                    .WithButton("Connect 4", $"challenge-s:{Context.User.Id},{target.Id},con", ButtonStyle.Primary)
                    .WithButton("Nevermind", $"challenge-n:{Context.User.Id}", ButtonStyle.Danger);

                await RespondAsync(
                    format: "video_game",
                    header: $"What game do you want to play against {target.Username}#{target.Discriminator}?",
                    components: cb,
                    ephemeral: false);
            }
        }

        [DoUserCheck]
        [ComponentInteraction("challenge-d:*,*")]
        public async Task DeniedChallengeAsync(ulong userId, ulong targetId)
        {
            await UpdateAsync(
                format: MessageFormat.NotAllowed,
                header: $"<@{targetId}>! <@{userId}> has denied your challenge.");
        }

        [DoUserCheck]
        [ComponentInteraction("challenge-n:*")]
        public async Task NevermindChallengeAsync(ulong _)
        {
            await UpdateAsync(
                error: "Okay, challenge ignored!");
        }

        [DoUserCheck]
        [ComponentInteraction("challenge-f:*,*")]
        public async Task ForfeitChallengeAsync(ulong _, ulong targetId)
        {
            using var tUser = await UserEntity.GetAsync(targetId);
            tUser.GamesWon++;

            await UpdateAsync(
                format: MessageFormat.NotAllowed,
                header: "Quit challenge!",
                context: $" <@{targetId}> will be rewarded because of your forfeit.");
        }

        [DoUserCheck]
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

            await UpdateAsync(
                format: "crossed_swords",
                header: $"<@{targetId}>! You have been challenged by <@{userId}> to a game of {gameData[1]}!", 
                context: "Are you up to the challenge?",
                components: cb);
        }
    }
}
