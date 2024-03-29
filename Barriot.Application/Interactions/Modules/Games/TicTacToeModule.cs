﻿using Barriot.Application.Interactions.Attributes;
using MongoDB.Bson;

namespace Barriot.Application.Interactions.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class TicTacToeModule : BarriotModuleBase
    {
        public TicTacToeModule(ILogger<BarriotModuleBase> logger) : base(logger)
        {

        }

        [DoUserCheck]
        [ComponentInteraction("tictactoe-init:*,*")]
        public async Task TicTacToeInitialAsync(ulong userId, ulong targetId)
        {
            var id = $"tictactoe:{targetId},";
            var cb = new ComponentBuilder();

            var game = await TicTacToeEntity.CreateAsync(userId, targetId);

            foreach (var pos in game.Positions)
                cb.WithButton(pos.Icon, id + $"{pos.X},{pos.Y},{game.ObjectId}", pos.Style, null, null, pos.SetBy != 0, pos.Y);

            cb.WithButton("Forfeit", $"challenge-f:{targetId},{userId}", ButtonStyle.Danger, row: 3);

            await UpdateAsync(
                format: "video_game",
                header: $"<@{targetId}>'s turn:",
                context: "Buttons are only available to whose turn it is!",
                components: cb);
        }

        [DoUserCheck]
        [ComponentInteraction("tictactoe:*,*,*,*")]
        public async Task TicTacToeAsync(ulong userId, short x, short y, ObjectId objectId)
        {
            using var game = await TicTacToeEntity.GetAsync(userId, objectId);

            if (game is null) // The game can be deleted from the DB as we try to get it. This way we prevent this from happening.
            {
                await RespondAsync(
                    error: "This game has been abandoned!",
                    description: "Due to being live for longer than a day, it's been pruned from the database and can no longer be played.");
                return;
            }

            using var opponent = await UserEntity.GetAsync(game.Players.First(x => x != userId));

            var id = $"tictactoe:{opponent.UserId},";
            var cb = new ComponentBuilder();

            game.Positions.First(pos => pos.X == x && pos.Y == y)
                .Modify(x =>
                {
                    x.Style = (game.Players.First() == userId)
                        ? ButtonStyle.Danger
                        : ButtonStyle.Success;
                    x.Icon = (game.Players.First() == userId)
                        ? "🔴"
                        : "🟢";
                    x.SetBy = opponent.UserId;
                });

            foreach (var pos in game.Positions)
                cb.WithButton(pos.Icon, id + $"{pos.X},{pos.Y},{game.ObjectId}", pos.Style, null, null, pos.SetBy != 0, pos.Y);

            if (!game.RowComplete(opponent.UserId))
            {
                if (game.Positions.Any(x => x.SetBy == 0))
                {
                    cb.WithButton("Forfeit", $"challenge-f:{opponent.UserId},{userId}", ButtonStyle.Danger, row: 3);
                    await UpdateAsync(
                        format: "video_game",
                        header: $"<@{opponent.UserId}>'s turn:",
                        components: cb);
                }

                else
                {
                    await UpdateAsync(
                        format: "military_medal",
                        header: "The result is a tie!",
                        context: "There are no points distributed.",
                        components: cb);

                    await game.DeleteAsync();
                }
            }

            else
            {
                cb = new ComponentBuilder();
                foreach (var pos in game.Positions)
                    cb.WithButton(pos.Icon, id + $"{pos.X},{pos.Y}", pos.Style, null, null, true, pos.Y);
                await UpdateAsync(
                    format: "trophy",
                    header: $"<@{userId}> has won.",
                    context: "Congratulations!",
                    components: cb);

                WonGame();
                await game.DeleteAsync();
            }
        }
    }
}
