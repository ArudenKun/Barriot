using Barriot.Interactions.Attributes;

namespace Barriot.Interactions.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class InboxModule : BarriotModuleBase
    {
        [SlashCommand("inbox", "Read messages ment for you by Barriot!")]
        public async Task InboxAsync()
            => await InboxAsync(Context.User.Id);

        [DoUserCheck]
        [ComponentInteraction("inbox:*")]
        public async Task InboxAsync(ulong _)
        {
            if (!Context.Member.Inbox.Any())
                await RespondAsync(
                    text: ":x: **You have no mail!** *Mail is only sent by the developer of Barriot to let you know that stuff has been changed around.*");

            else
            {
                var cb = new ComponentBuilder()
                    .WithButton("Next message", $"inbox:{Context.User.Id}");

                bool count = Context.Member.Inbox.Count > 1;

                await RespondAsync(
                    text: $":speech_balloon: **You have {Context.Member.Inbox.Count} message{(count ? "(s)" : "")}!** Here {(count ? "is one of them:" : "it is:")}\n\n" +
                    $"> {Context.Member.Inbox[0]}",
                    components: count ? cb.Build() : null,
                    ephemeral: true);

                var inbox = Context.Member.Inbox;

                inbox.RemoveAt(0);

                Context.Member.Inbox = inbox;
            }
        }
    }
}
