﻿using Barriot.Interaction.Attributes;
using MongoDB.Bson;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class Connect4Module : BarriotModuleBase
    {
        [DoUserCheck]
        [DisableSource]
        [ComponentInteraction("connect-init:*,*")]
        public async Task ConnectInitialAsync(ulong userId, ulong targetId)
        {
            var game = await ConnectEntity.CreateAsync(userId, targetId);

            var sb = new SelectMenuBuilder()
                .WithPlaceholder("Select a column to drop a chip into!")
                .WithCustomId($"connect:{targetId},{game.ObjectId}")
                .WithMinValues(1)
                .WithMaxValues(1);

            short x = 0;
            short y = 0;
            string response = $"**<@{targetId}>'s turn:**\n> *Play by selecting a row in the dropdown, a coin will drop down on that column.*\n\n";

            for (int i = 0; i < (game.MaxX * game.MaxY); i++)
            {
                response += game.Positions[x, y].Emoji;
                x++;
                if (x == game.MaxX)
                {
                    response += "\n";
                    y++;
                    x = 0;
                }
            }

            for (int i = 0; i <= game.MaxY; i++)
                sb.AddOption($"Column {i + 1}", $"{i + 1}");

            var cb = new ComponentBuilder()
                .WithSelectMenu(sb)
                .WithButton("Forfeit", $"challenge-f:{targetId},{userId}", ButtonStyle.Danger, row: 2);

            await RespondAsync(
                text: response,
                components: cb.Build(),
                ephemeral: false);
        }

        [DoUserCheck]
        [ComponentInteraction("connect:*,*")]
        public async Task ConnectAsync(ulong userId, ObjectId objectId, string[] selectedRows)
        {
            using var game = await ConnectEntity.GetAsync(userId, objectId);

            if (game is null) // The game can be deleted from the DB as we try to get it. This way we prevent this from happening.
            {
                await RespondAsync(
                    text: ":x: **This game has been abandoned!** \n\n> Due to being live for longer than a day, it's been pruned from the database and can no longer be played.",
                    ephemeral: true);
                return;
            }

            using var opponent = await UserEntity.GetAsync(game.Players.First(x => x != userId));

            bool won = game.SetAndCheck(userId, short.Parse(selectedRows[0]));

            bool cont = false;
            foreach (var pos in game.Positions)
            {
                if (pos.SetBy == 0)
                {
                    cont = true;
                    break;
                }
            }

            if (!cont)
            {
                await UpdateAsync(
                    text: $":military_medal: **The game is a tie!** No points have been distributed.",
                    components: new ComponentBuilder().Build());

                await game.DeleteAsync();
                return;
            }

            short x = 0;
            short y = 0;
            string response = won
                ? $":trophy: **<@{userId}> has won!**\n"
                : $"**<@{opponent.UserId}>'s turn:**\n> *Play by selecting a row in the dropdown, a coin will drop down on that column.*\n\n";

            for (int i = 0; i < (game.MaxX * game.MaxY); i++)
            {
                response += game.Positions[x, y].Emoji;
                x++;
                if (x == game.MaxX)
                {
                    response += "\n";
                    y++;
                    x = 0;
                }
            }

            if (!won)
            {
                var sb = new SelectMenuBuilder()
                    .WithPlaceholder("Select a column to drop a chip into!")
                    .WithCustomId($"connect:{opponent.UserId},{game.ObjectId}")
                    .WithMinValues(1)
                    .WithMaxValues(1);
                for (int i = 0; i <= game.MaxY; i++)
                {
                    if (!game.IsFilled(i))
                        sb.AddOption($"Column {i + 1}", $"{i + 1}");
                }

                var cb = new ComponentBuilder()
                    .WithSelectMenu(sb)
                    .WithButton("Forfeit", $"challenge-f:{opponent.UserId},{userId}", ButtonStyle.Danger, row: 2);

                await UpdateAsync(
                    text: response,
                    components: cb.Build());
            }

            else
            {
                await UpdateAsync(
                    text: response,
                    components: new ComponentBuilder().Build());
                Context.WonGameInSession = true;

                await game.DeleteAsync();
            }
        }
    }
}