using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;
using Barriot.Extensions;

namespace Barriot.Interaction.Modules.Channels
{
    [IgnoreBlacklistedUsers]
    public class PinModule : BarriotModuleBase
    {
        public async Task PersonalPinAsync()
        {

        }

        public async Task FinalizePinAsync(QueryModal<string> modal)
        {
            if (!StringExtensions.TryGetLinkData(modal.Result, out var data))
            {
                await RespondAsync(
                    error: "Provided link is not a valid message link!",
                    context: $"Input: {modal.Result}");
            }

            if ()

            var channel = await Context.Guild.GetChannelAsync(data[1]);

            if (channel is null || channel is not RestTextChannel textChannel)
            {
                await RespondAsync(
                    error: "The message link does not lead to a usable/viewable channel.",
                    context: $"Input: ({modal.Result})");
                return;
            }

            var message = await textChannel.GetMessageAsync(data[2]);

            if (message is null || message is not RestUserMessage userMessage)
            {
                await RespondAsync(
                    error: "The message link does not lead to a usable message.",
                    context: $"Input: ({modal.Result})");
                return;
            }

            if (userMessage.Interaction is not null || userMessage.Author.Id != Context.Client.CurrentUser.Id)
            {
                await RespondAsync(
                    error: "This message is not a valid self-assign role message!",
                    context: "The message has to be created by Barriot and not be part of an interaction.");
                return;
            }
        }

        public async Task ListPinsAsync()
        {

        }

        public async Task ListPinsAsync()
        {

        }
    }
}
