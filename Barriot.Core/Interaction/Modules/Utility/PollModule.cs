using Barriot.Entities.Polls;
using Barriot.Extensions;
using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class PollModule : BarriotModuleBase
    {
        [SlashCommand("poll", "Create a poll for the community to answer to.")]
        public async Task PollAsync()
            => await RespondWithModalAsync<PollModal>("poll:1");

        [ModalInteraction("poll:*")]
        public async Task FinalizePollAsync(string _, PollModal modal)
        {
            await RespondAsync(
                format: "bar_chart",
                header: "Poll:",
                description: modal.Description,
                ephemeral: false);

            var response = await Context.Interaction.GetOriginalResponseAsync();

            var cb = new ComponentBuilder();

            string[] options = new string[]
            {
                modal.Option1,
                modal.Option2,
                modal.Option3 ?? "",
                modal.Option4 ?? "",
            };

            List<PollOption> optionsList = new();
            int id = 0;
            foreach (var opt in options)
            {
                if (!string.IsNullOrEmpty(opt))
                {
                    id++;
                    var value = opt.Reduce(80);
                    cb.WithButton(value, $"poll:{response.Id},{id}", ButtonStyle.Secondary);
                    optionsList.Add(new(id, value));
                }
            }
            cb.WithButton("Results", $"pollresults:{response.Id}", row: 1);

            await Context.Interaction.ModifyOriginalResponseAsync(x => x.Components = cb.Build());

            await PollEntity.CreateAsync(response.Id, optionsList);
        }

        [ComponentInteraction("poll:*,*")]
        public async Task VoteForPollAsync(ulong messageId, int pollId)
        {
            using var poll = await PollEntity.GetAsync(messageId);

            if (poll != null)
            {
                if (!poll.AlreadyReplied.Any(x => x == Context.User.Id))
                {
                    poll.Options.First(x => x.Id == pollId).Votes++;
                    poll.AlreadyReplied.Add(Context.User.Id);

                    await RespondAsync(
                        format: ResultFormat.Success,
                        header: "Thank you for voting!",
                        context: "Your vote has been registered.",
                        ephemeral: true);
                }
                else
                    await RespondAsync(
                        error: "You already responded to this poll!");
            }
            else
                await RespondAsync(
                    error: "Unable to register vote!",
                    context: "This poll is more than 15 days old and has been pruned from the database.");
        }

        [ComponentInteraction("pollresults:*")]
        public async Task GetPollResultsAsync(ulong messageId)
        {
            var results = await PollEntity.GetAsync(messageId);

            if (results.Options.Any())
            {
                var eb = new EmbedBuilder()
                    .WithColor(Context.User.AccentColor ?? Color.Blue);
                foreach (var r in results.Options)
                    eb.AddField($"{r.Label} [{r.Id}]", $"Amount of votes: {r.Votes}");

                await RespondAsync(
                    format: "chart_with_upwards_trend",
                    header: "Results for this poll:",
                    embed: eb.Build(),
                    ephemeral: true);
            }
            else
                await RespondAsync(
                    error: "No results found!",
                    context: "This poll is more than 15 days old and has been pruned from the database.");
        }
    }
}
