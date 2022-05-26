using Barriot.Interaction.Attributes;
using Barriot.Pagination;
using Barriot.Models;
using MongoDB.Bson;
using Barriot.Extensions;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class PinModule : BarriotModuleBase
    {
        [MessageCommand("Pin")]
        public async Task PinAsync(IMessage message)
        {
            if (!JumpUrl.TryParse(message.GetJumpUrl(), out var messageUrl))
            {
                await RespondAsync(
                    error: "An unexpected error occurred while parsing this message!",
                    context: "Please report this behavior.");
                return;
            }

            var pins = await PinEntity.GetManyAsync(Context.User.Id);

            if (pins.Any(x => x.MessageId == messageUrl.MessageId))
            {
                await RespondAsync(
                    error: "You already pinned this message!",
                    context: "View your messages by executing ` /pins `.");
                return;
            }

            await PinEntity.CreateAsync(Context.User.Id, messageUrl);

            await RespondAsync(
                format: ResultFormat.Success,
                header: "Succesfully created pin!",
                context: "View your pins by executing ` /pins `.",
                description: $"Message link: {messageUrl}");
        }

        [SlashCommand("pins", "View all your current pins.")]
        public async Task ListPinsAsync(
            [Summary("page", "The page you want to view")] int page = 1)
        {
            var value = await ListPinsInternalAsync(page);

            if (value is not null)
                await RespondAsync(
                    page: value.Value,
                    header: "A list of all your pins:",
                    context: "Click on the button below to remove any pins from this page.");

            else
                await RespondAsync(
                    error: "You currently have no pins!",
                    context: "Use ` Pin ` in message apps to add a new pin.");
        }

        [ComponentInteraction("pin-list:*")]
        public async Task ListPinsFromButtonAsync(int page)
        {
            var value = await ListPinsInternalAsync(page);

            if (value is not null)
                await UpdateAsync(
                    page: value.Value,
                    header: "A list of all your pins:");

            else
                await UpdateAsync(
                    error: "You currently have no pins!",
                    context: "Use ` Pin ` in message apps to add a new pin.");
        }

        private async Task<Page?> ListPinsInternalAsync(int page)
        {
            if (page < 1)
                page = 1;

            var pins = await PinEntity.GetManyAsync(Context.User.Id);

            if (pins.Any())
            {
                if (!Paginator<PinEntity>.TryGet(out var paginator))
                {
                    paginator = new PaginatorBuilder<PinEntity>()
                        .WithCustomId("pin-list")
                        .WithPages(x =>
                        {
                            var pinnedSince = DateTime.UtcNow - x.PinDate;

                            return new($"{pinnedSince.ToReadable()} ago.", x.Url);
                        })
                        .Build();
                }
                var value = paginator.GetPage(page, pins);

                value.Component.WithButton("Delete pins", $"pins-delete:{page}", ButtonStyle.Danger);

                return value;
            }

            return null;
        }

        [ComponentInteraction("pins-delete:*")]
        public async Task DeletePinsAsync(int page)
        {
            var pins = await PinEntity.GetManyAsync(Context.User.Id);

            if (pins.Any())
            {
                var sb = new SelectMenuBuilder()
                    .WithMinValues(1)
                    .WithCustomId("pins-deleted")
                    .WithPlaceholder("Select 1 or more pins to delete.");

                int index = page * 10 - 10;

                var range = pins.GetRange(index, pins.Count - index);
                for (int i = 0; i < range.Count; i++)
                {
                    if (i is 10)
                        break;
                    sb.AddOption(range[i].PinDate.ToString(), range[i].ObjectId.ToString(), range[i].MessageId.ToString());
                }

                sb.WithMaxValues(sb.Options.Count);

                var cb = new ComponentBuilder()
                    .WithSelectMenu(sb);

                await UpdateAsync(
                    format: ResultFormat.Deleting,
                    header: "Deleting pins:",
                    components: cb);
            }
            else
                await UpdateAsync(
                    error: "You have no pins to delete!",
                    context: "The page you selected from is outdated and does not contain any pins.");
        }

        [ComponentInteraction("pins-deleted")]
        public async Task DeletedPinsAsync(ObjectId[] selectedValues)
        {
            var pins = await PinEntity.GetManyAsync(Context.User.Id);

            if (!pins.Any())
                await UpdateAsync(
                    error: "You have no pins to delete!");

            else
            {
                foreach (var value in selectedValues)
                {
                    var pin = pins.First(x => x.ObjectId == value);

                    if (pin is not null)
                        await pin.DeleteAsync();
                }
                await UpdateAsync(
                    format: ResultFormat.Success,
                    header: $"Succesfully removed {selectedValues.Length} reminder(s).");
            }
        }
    }
}
