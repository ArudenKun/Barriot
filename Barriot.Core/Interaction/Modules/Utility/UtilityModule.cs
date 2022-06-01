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
                    format: MessageFormat.Success,
                    header: "Sent message content in DM!");
            }
            catch
            {
                await RespondAsync(
                    error: "I was not able to DM you!",
                    context: " If you want to use this command, please make sure Barriot can DM you.");
            }
        }

        [SlashCommand("math", "Does math.")]
        public async Task MathAsync([Summary("equation", "The calculation to make")] Calculation calculation)
        {
            if (calculation.Result is double.NaN)
                await RespondAsync(
                    error: "Calculation failed!",
                    context: calculation.Error);

            else
                await RespondAsync(
                    format: "abacus",
                    header: "The result is:",
                    context: calculation.ToString());
        }
    }
}
