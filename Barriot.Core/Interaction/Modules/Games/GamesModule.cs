using Barriot.Extensions.Files;
using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class GamesModule : BarriotModuleBase
    {
        [SlashCommand("riddle", "Gets a random riddle.")]
        public async Task RiddleAsync()
        {
            var file = FileHelper.GetDataFromFile("Riddles");

            var cb = new ComponentBuilder()
                .WithButton("Answer", $"riddle:{Context.User.Id},{file.Index}");

            await RespondAsync(
                text: $":question: **Answer me this:** {file.SelectedLine.Split('|').First()}",
                ephemeral: Context.Member.DoEphemeral,
                components: cb.Build());
        }

        [DoUserCheck]
        [ComponentInteraction("riddle:*,*")]
        public async Task RiddleAsync(ulong _, int riddleId)
        {
            await RespondAsync(
                text: $":eyes: **The answer to your riddle is:** {FileHelper.GetDataFromFile("Riddles").Lines[riddleId].Split('|').Last()}",
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("question", "Give me a question, I'll answer!")]
        public async Task QuestionAsync(
            [Summary("question", "The question to ask")] string? _ = null)
        {
            await RespondAsync(
                text: $":speech_balloon: **{FileHelper.GetDataFromFile("Answers").SelectedLine}**",
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("random-fact", "A random fact")]
        public async Task RandomFactAsync()
        {
            await RespondAsync(
                text: $":bulb: **Did you know that:** {FileHelper.GetDataFromFile("Facts").SelectedLine}",
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("showerthought", "Ever thought about something odd in the shower? I certainly did!")]
        public async Task ShowerThoughtsAsync()
        {
            await RespondAsync(
                text: $":thinking: **Have you ever thought about:** {FileHelper.GetDataFromFile("Thoughts").SelectedLine}",
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("dadjoke", "They're pretty bad tbh...")]
        public async Task DadJokeAsync()
        {
            await RespondAsync(
                text: $":man_facepalming: **Heres a good one:** {FileHelper.GetDataFromFile("Jokes").SelectedLine}",
                ephemeral: Context.Member.DoEphemeral);
        }
    }
}
