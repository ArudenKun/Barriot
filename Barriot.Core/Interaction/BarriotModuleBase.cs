namespace Barriot.Interaction
{
    /// <summary>
    ///     Represents a non-generic Barriot module base that implements <see cref="BarriotInteractionContext"/>.
    /// </summary>
    public class BarriotModuleBase : RestInteractionModuleBase<BarriotInteractionContext>
    {
        /// <summary>
        ///     Updates the interaction the current <see cref="ComponentInteractionAttribute"/> marked method sources from.
        /// </summary>
        /// <param name="text">The text to update to.</param>
        /// <param name="components">The components to update to.</param>
        /// <param name="embed">The embed to update to.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        /// <exception cref="InvalidCastException">Thrown if this method is called on an unsupported type of interaction.</exception>
        public async Task UpdateAsync(string? text = null, MessageComponent? components = null, Embed? embed = null)
        {
            if (Context.Interaction is not RestMessageComponent component)
                throw new InvalidCastException($"{nameof(UpdateAsync)} can only be executed for a {nameof(RestMessageComponent)}");

            var payload = component.Update(x =>
            {
                if (text is not null)
                    x.Content = text;

                if (components is not null)
                    x.Components = components;

                if (embed is not null)
                    x.Embed = embed;
                else // set embed to null due to relation with components.
                    x.Embed = null;
            });
            await Context.InteractionResponseCallback(payload);
        }

        private static int CalculateTier(long currentPoints, ref int ranking)
        {
            int tier = 0;
            while (currentPoints >= ranking)
            {
                ranking *= 2;
                tier++;
            }
            ranking /= 2;
            return tier;
        }

        private bool CanAssignFlag(UserFlag newFlag)
        {
            List<UserFlag> flags = Context.Member.Flags;
            UserFlag[] newFlags = { newFlag };

            if (!flags.Any(x => x.Title == newFlag.Title))
            {
                flags.RemoveAll(x => x.Type == newFlag.Type);
                Context.Member.Flags = new(flags.Concat(newFlags));
                return true;
            }
            return false;
        }

        public override async Task AfterExecuteAsync(ICommandInfo command)
        {
            if (Context.Member.UserName is "Unknown")
                Context.Member.UserName = $"{Context.User.Username}#{Context.User.Discriminator}";

            if (Context.WonGameInSession)
            {
                Context.Member.GamesWon++;

                int ranking = 10;
                int tier = CalculateTier(Context.Member.GamesWon, ref ranking);

                if (tier is not (0 or > 10) && CanAssignFlag(UserFlag.CreateChampion(tier, ranking)))
                    await FollowupAsync(
                        text: $":star: **Congratulations!** You have won over ` {ranking} ` challenges and have been granted a new acknowledgement!" +
                        $"\n\n> You can find your acknowledgements by executing ` /statistics `",
                        ephemeral: true);
            }

            if (command is ComponentCommandInfo or ModalCommandInfo)
            {
                Context.Member.ButtonsPressed++;

                int ranking = 300;
                int tier = CalculateTier(Context.Member.ButtonsPressed, ref ranking);

                if (tier is not (0 or > 10) && CanAssignFlag(UserFlag.CreateComponent(tier, ranking)))
                    await FollowupAsync(
                        text: $":star: **Congratulations!** You have pressed over ` {ranking} ` buttons and have been granted a new acknowledgement!" +
                        $"\n\n> You can find your acknowledgements by executing ` /statistics `",
                        ephemeral: true);
            }

            else
            {
                if (Context.Member.Inbox.Any() && command.Name != "inbox")
                    await FollowupAsync(
                        text: ":speech_balloon: **You have unread mail!** Please use ` /inbox ` to read this mail.",
                        ephemeral: true);

                Context.Member.LastCommand = command.Name;
                Context.Member.CommandsExecuted++;

                int ranking = 150;
                int tier = CalculateTier(Context.Member.CommandsExecuted, ref ranking);

                if (tier is not (0 or > 10) && CanAssignFlag(UserFlag.CreateCommand(tier, ranking)))
                    await FollowupAsync(
                        text: $":star: **Congratulations!** You have executed over ` {ranking} ` commands and been granted a new acknowledgement!" +
                        $"\n\n> You can find your acknowledgements by executing ` /statistics `",
                        ephemeral: true);
            }
        }
    }
}
