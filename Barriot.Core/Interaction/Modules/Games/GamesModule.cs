using Barriot.Extensions;
using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class GamesModule : BarriotModuleBase
    {
        [SlashCommand("riddle", "Gets a random riddle.")]
        public async Task RiddleAsync()
        {
            var file = FileExtensions.GetDataFromFile("Riddles");

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
                text: $":eyes: **The answer to your riddle is:** {FileExtensions.GetDataFromFile("Riddles").Lines[riddleId].Split('|').Last()}",
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("question", "Give me a question, I'll answer!")]
        public async Task QuestionAsync(
            [Summary("question", "The question to ask")] string? _ = null)
        {
            await RespondAsync(
                text: $":speech_balloon: **{FileExtensions.GetDataFromFile("Answers").SelectedLine}**",
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("random-fact", "A random fact")]
        public async Task RandomFactAsync()
        {
            await RespondAsync(
                text: $":bulb: **Did you know that:** {FileExtensions.GetDataFromFile("Facts").SelectedLine}",
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("showerthought", "Ever thought about something odd in the shower? I certainly did!")]
        public async Task ShowerThoughtsAsync()
        {
            await RespondAsync(
                text: $":thinking: **Have you ever thought about:** {FileExtensions.GetDataFromFile("Thoughts").SelectedLine}",
                ephemeral: Context.Member.DoEphemeral);
        }

        [SlashCommand("dadjoke", "They're pretty bad tbh...")]
        public async Task DadJokeAsync()
        {
            await RespondAsync(
                text: $":man_facepalming: **Heres a good one:** {FileExtensions.GetDataFromFile("Jokes").SelectedLine}",
                ephemeral: Context.Member.DoEphemeral);
        }
    }
}
