using Barriot.Interaction.Attributes;
using Barriot.Interaction.Modals;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class FlagsModule : BarriotModuleBase
    {
        [DoUserCheck]
        [ComponentInteraction("flag-creating:*,*")]
        public async Task FlagAsync(ulong _, ulong targetId)
        {
            // We're hardcoding this as I don't want to make a slow reflection based processor for these properties :'')
            var sb = new SelectMenuBuilder()
                .WithPlaceholder("Select 1 or more acknowledgements to add.")
                .WithCustomId($"flag-created:{Context.User.Id},{targetId}")
                .WithMinValues(1);

            var user = await UserEntity.GetAsync(targetId);

            if (!user.Flags.Any(x => x.Type is FlagType.Support))
            {
                var support = UserFlag.Support;
                sb.AddOption(support.Title, "support", support.Description, Emoji.Parse(support.Emoji));
            }

            if (!user.Flags.Any(x => x.Type is FlagType.Contributor))
            {
                var contrib = UserFlag.Contributor;
                sb.AddOption(contrib.Title, "contributor", contrib.Description, Emoji.Parse(contrib.Emoji));
            }

            if (!user.Flags.Any(x => x.Type is FlagType.Developer))
            {
                var developer = UserFlag.Developer;
                sb.AddOption(developer.Title, "developer", developer.Description, Emoji.Parse(developer.Emoji));
            }

            var cb = new ComponentBuilder();
            if (sb.Options.Any())
                cb.WithSelectMenu(sb.WithMaxValues(sb.Options.Count));

            cb.WithButton("Create custom acknowledgement", $"cflag-creating:{Context.User.Id},{targetId}");

            await UpdateAsync(
                text: ":writing_hand: **Add acknowledgements:** *Select the acknowledgements you want to add or create a custom one below.*",
                components: cb.Build());
        }

        [DoUserCheck]
        [ComponentInteraction("flag-created:*,*")]
        public async Task FinalizeFlagAsync(ulong _, ulong targetId, string[] values)
        {
            List<UserFlag> flags = new();
            foreach (var value in values)
            {
                switch (value)
                {
                    case "developer":
                        flags.Add(UserFlag.Developer);
                        break;
                    case "contributor":
                        flags.Add(UserFlag.Contributor);
                        break;
                    case "support":
                        flags.Add(UserFlag.Support);
                        break;
                    default:
                        await RespondAsync(
                            text: ":x: **This acknowledgement does not exist!** Please report this error.",
                            ephemeral: true);
                        break;
                }
            }
            var user = await UserEntity.GetAsync(targetId);
            user.Flags = new(flags.Concat(user.Flags));

            await UpdateAsync(
                text: $":white_check_mark: **Successfully added {values.Length} acknowledgements.**");
        }

        [DoUserCheck]
        [ComponentInteraction("cflag-creating:*,*")]
        public async Task CustomFlagAsync(ulong _, ulong targetId)
            => await RespondWithModalAsync<FlagModal>($"cflag-created:{targetId}");

        [ModalInteraction("cflag-created:*")]
        public async Task FinalizeCustomFlagAsync(ulong targetId, FlagModal modal)
        {
            if (string.IsNullOrWhiteSpace(modal.Name))
                await RespondAsync(
                    text: ":x: **The acknowledgement name cannot be empty!**",
                    ephemeral: true);

            else if (string.IsNullOrWhiteSpace(modal.Emoji))
                await RespondAsync(
                    text: ":x: **The acknowledgement emoji cannot be empty!**",
                    ephemeral: true);

            else
            {

                UserFlag[] flags = { UserFlag.SetCustomFlag(modal.Name, modal.Emoji, modal.Description) };

                var user = await UserEntity.GetAsync(targetId);
                user.Flags = new(flags.Concat(user.Flags));

                await RespondAsync(
                    text: $":white_check_mark: **Custom acknowledgement successfully added.**",
                    ephemeral: Context.Member.DoEphemeral);
            }
        }

        [DoUserCheck]
        [ComponentInteraction("flag-deleting:*,*")]
        public async Task DeletingFlagsAsync(ulong _, ulong targetId)
        {
            var user = await UserEntity.GetAsync(targetId);

            var selection = user.Flags;

            var sb = new SelectMenuBuilder()
                .WithMinValues(1)
                .WithMaxValues(selection.Count)
                .WithCustomId($"flag-deleted:{Context.User.Id},{targetId}")
                .WithPlaceholder("Select 1 or more acknowledgements to delete.");

            for (int i = 0; i < selection.Count; i++)
            {
                var msg = selection[i].Description;
                if (msg.Length > 99)
                    msg = msg[..96] + "...";
                sb.AddOption(selection[i].Title, i.ToString(), msg, Emoji.Parse(selection[i].Emoji));
            }

            var cb = new ComponentBuilder()
                .WithSelectMenu(sb);

            await UpdateAsync(
                text: ":wastebasket: **Delete acknowledgements:** *Select the acknowledgements you want to delete in the dropdown below.*",
                components: cb.Build());
        }

        [DoUserCheck]
        [ComponentInteraction("flag-deleted:*,*")]
        public async Task DeletedFlagsAsync(ulong _, ulong targetId, string[] selectedAcks)
        {
            var set = selectedAcks.Select(x => int.Parse(x)).ToList();

            set.Sort();
            set.Reverse();

            var user = await UserEntity.GetAsync(targetId);

            var selection = user.Flags;

            foreach (var value in set)
                selection.RemoveAt(value);

            user.Flags = new(selection);

            await UpdateAsync(
                text: $":white_check_mark: **Successfully removed {selectedAcks.Length} acknowledgements.**");
        }
    }
}
