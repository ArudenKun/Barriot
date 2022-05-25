using Barriot.Models.Files;
using Barriot.Pagination;
using Barriot.Extensions;

namespace Barriot.Interaction
{
    /// <summary>
    ///     Represents a non-generic Barriot module base that implements <see cref="BarriotInteractionContext"/>.
    /// </summary>
    public class BarriotModuleBase : RestInteractionModuleBase<BarriotInteractionContext>
    {
        #region UpdateAsync

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
                else if (x.Components.IsSpecified)
                    x.Components = new ComponentBuilder().Build();

                if (embed is not null)
                    x.Embed = embed;
                else if (x.Embed.IsSpecified)
                    x.Embed = null;
            });
            await Context.InteractionResponseCallback(payload);
        }

        /// <summary>
        ///     Updates the interaction the current <see cref="ComponentInteractionAttribute"/> marked method sources from.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="header"></param>
        /// <param name="context"></param>
        /// <param name="description"></param>
        /// <param name="components">The components to update to.</param>
        /// <param name="embed">The embed to update to.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        /// <exception cref="InvalidCastException">Thrown if this method is called on an unsupported type of interaction.</exception>
        public async Task UpdateAsync(ResultFormat format, string header, string? context = null, string? description = null, MessageComponent? components = null, Embed? embed = null)
        {
            var tb = new TextBuilder()
                .WithResult(format)
                .WithHeader(header)
                .WithDescription(description)
                .WithContext(context);

            await UpdateAsync(
                text: tb.Build(),
                components: components,
                embed: embed);
        }

        /// <summary>
        ///     Updates the interaction the current <see cref="ComponentInteractionAttribute"/> marked method sources from with an error.
        /// </summary>
        /// <param name="error">The error message to send.</param>
        /// <param name="context"></param>
        /// <param name="description"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        /// <exception cref="InvalidCastException">Thrown if this method is called on an unsupported type of interaction.</exception>
        public async Task UpdateAsync(string error, string? context = null, string? description = null)
        {
            var tb = new TextBuilder()
                .WithResult(ResultFormat.Failure)
                .WithHeader(error)
                .WithContext(context)
                .WithDescription(description);

            await UpdateAsync(
                text: tb.Build());
        }

        public async Task UpdateAsync(ErrorInfo error, string parameter = "")
        {
            var text = FileExtensions.GetError(error, parameter);

            await UpdateAsync(
                text: text);
        }

        /// <summary>
        ///     Updates the interaction the current <see cref="ComponentInteractionAttribute"/> marked method sources from with a page.
        /// </summary>
        /// <param name="page">The page to send.</param>
        /// <param name="context"></param>
        /// <param name="description"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public async Task UpdateAsync(Page page, string header, string? context = null)
        {
            var tb = new TextBuilder()
                .WithResult(ResultFormat.List)
                .WithHeader(header)
                .WithContext(context);

            await UpdateAsync(
                text: tb.Build(),
                components: page.Component.Build(),
                embed: page.Embed.Build());
        }

        #endregion

        #region RespondAsync

        /// <summary>
        ///     Responds to the current <see cref="RestInteraction"/>.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="header"></param>
        /// <param name="context"></param>
        /// <param name="description"></param>
        /// <param name="components">The components to send.</param>
        /// <param name="embed">The embed to send.</param>
        /// <param name="ephemeral">If the message should be ephemerally sent.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public async Task RespondAsync(ResultFormat format, string header, string? context = null, string? description = null, MessageComponent? components = null, Embed? embed = null, bool? ephemeral = null)
        {
            var tb = new TextBuilder()
                .WithResult(format)
                .WithHeader(header)
                .WithDescription(description)
                .WithContext(context);

            if (format == ResultFormat.Failure || format == ResultFormat.List)
                ephemeral = true;

            else
                ephemeral ??= Context.Member.DoEphemeral;

            await base.RespondAsync(
                text: tb.Build(),
                components: components,
                embed: embed,
                ephemeral: ephemeral.Value);
        }

        /// <summary>
        ///     Responds to the current <see cref="RestInteraction"/> with an error.
        /// </summary>
        /// <param name="error">The error to send.</param>
        /// <param name="context">Error context if applicable.</param>
        /// <param name="description"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public async Task RespondAsync(string error, string? context = null, string? description = null)
        {
            var tb = new TextBuilder()
                .WithResult(ResultFormat.Failure)
                .WithHeader(error)
                .WithContext(context)
                .WithDescription(description);

            await base.RespondAsync(
                text: tb.Build(),
                ephemeral: true);
        }

        /// <summary>
        ///     Responds to the current <see cref="RestInteraction"/> with a page.
        /// </summary>
        /// <param name="page">The page to send.</param>
        /// <param name="header"></param>
        /// <param name="context"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public async Task RespondAsync(Page page, string header, string? context = null)
        {
            var tb = new TextBuilder()
                .WithResult(ResultFormat.List)
                .WithHeader(header)
                .WithContext(context);

            await base.RespondAsync(
                text: tb.Build(),
                components: page.Component.Build(),
                embed: page.Embed.Build(),
                ephemeral: true);
        }

        #endregion

        #region FollowupAsync

        /// <summary>
        ///     Follows up to the current <see cref="RestInteraction"/>.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="header"></param>
        /// <param name="context"></param>
        /// <param name="description"></param>
        /// <param name="components">The components to send.</param>
        /// <param name="embed">The embed to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public async Task FollowupAsync(ResultFormat format, string header, string? context = null, string? description = null, MessageComponent? components = null, Embed? embed = null)
        {
            var tb = new TextBuilder()
                .WithResult(format)
                .WithHeader(header)
                .WithDescription(description)
                .WithContext(context);

            await base.FollowupAsync(
                text: tb.Build(),
                components: components,
                embed: embed);
        }

        /// <summary>
        ///     Follows up to the current <see cref="RestInteraction"/> with an error.
        /// </summary>
        /// <param name="error">The error to send.</param>
        /// <param name="context"></param>
        /// <param name="description"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public async Task FollowupAsync(string error, string? context = null, string? description = null)
        {
            var tb = new TextBuilder()
                .WithResult(ResultFormat.Failure)
                .WithHeader(error)
                .WithContext(context)
                .WithDescription(description);

            await base.FollowupAsync(
                text: tb.Build());
        }

        #endregion

        #region Post-processing

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
                        text: $":star: **Congratulations!** *You have won over ` {ranking} ` challenges and have been granted a new acknowledgement!*" +
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
                        text: $":star: **Congratulations!** *You have pressed over ` {ranking} ` buttons and have been granted a new acknowledgement!*" +
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
                        text: $":star: **Congratulations!** *You have executed over ` {ranking} ` commands and been granted a new acknowledgement!*" +
                        $"\n\n> You can find your acknowledgements by executing ` /statistics `",
                        ephemeral: true);
            }
        }

        #endregion
    }
}
