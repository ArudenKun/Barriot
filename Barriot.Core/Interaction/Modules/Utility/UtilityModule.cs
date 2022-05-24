﻿using Barriot.Extensions.Models;
using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
    /// <summary>
    ///     A module where stuff goes that isnt really big enough to be classified in its own module.
    /// </summary>
    [IgnoreBlacklistedUsers]
    public class UtilityModule : BarriotModuleBase
    {
        [MessageCommand("Quote")]
        public async Task QuoteAsync(IMessage message)
        {
            try
            {
                await Context.User.SendMessageAsync(
                    text: $":speech_balloon: **Sent by {message.Author.Username}#{message.Author.Discriminator} at {message.Timestamp}:**" +

                          $"{(!string.IsNullOrEmpty(message.Content) ? $"\n\n> {message.Content}" : "")} \n\n" +
                          $"{string.Join("\n", message.Attachments.Select(x => x.Url))}");

                await RespondAsync(
                    text: ":white_check_mark: **Sent message content in DM!**",
                    ephemeral: Context.Member.DoEphemeral);
            }
            catch
            {
                await RespondAsync(
                    text: ":x: **I was not able to DM you!** If you want to use this command, please make sure Barriot can DM you.",
                    ephemeral: true);
            }
        }

        [SlashCommand("math", "Does math.")]
        public async Task MathAsync([Summary("equation", "The calculation to make")] Calculation calculation)
        {
            if (calculation.Result is double.NaN)
                await RespondAsync(
                    text: $":x: **Calculation failed!** {calculation.Error}",
                    ephemeral: true);

            else
                await RespondAsync(
                    text: $":abacus: **The result is:** {calculation}",
                    ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("ping", "Pong! See if the bot works. If this command fails, all is lost...")]
        public async Task PingAsync()
            => await RespondAsync(
                text: $":ping_pong: **Pong!**",
                ephemeral: Context.Member.DoEphemeral);

        [SlashCommand("coinflip", "Flips a coin.")]
        public async Task CoinFlipAsync()
            => await RespondAsync(
                text: (new Random().Next(2) < 1) ? ":coin: **Heads!**" : ":coin: **Tails!**",
                ephemeral: Context.Member.DoEphemeral);
    }
}
