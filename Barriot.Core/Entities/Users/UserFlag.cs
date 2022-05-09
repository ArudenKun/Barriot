namespace Barriot
{
    /// <summary>
    ///     Represents a user acknowledgement.
    /// </summary>
    public record class UserFlag
    {
        /// <summary>
        ///     The title of this acknowledgement.
        /// </summary>
        public string Title { get; private set; } = string.Empty;

        /// <summary>
        ///     The emoji of this acknowledgement.
        /// </summary>
        public string Emoji { get; private set; } = string.Empty;

        /// <summary>
        ///     The acknowledgement description.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        ///     The flags unique type.
        /// </summary>
        public FlagType Type { get; private set; }

        #region Creation

        /// <summary>
        ///     Top voter for last month.
        /// </summary>
        public static UserFlag TopVoter
            => new()
            {
                Emoji = ":crown:",
                Title = "Top voter",
                Description = $"Top voter of last month.",
                Type = FlagType.TopVoter,
            };

        /// <summary>
        ///     Someone who donated to Barriot.
        /// </summary>
        public static UserFlag Donator
            => new()
            {
                Emoji = ":heart:",
                Title = "Donator",
                Description = "A donator, making Barriot development possible!",
                Type = FlagType.Donator,
            };

        /// <summary>
        ///     A support person.
        /// </summary>
        public static UserFlag Support
            => new()
            {
                Emoji = ":gear:",
                Title = "Auxiliary Barriot staff",
                Description = "A support worker for Barriot, responsible for direct user support & managing users.",
                Type = FlagType.Support,
            };

        /// <summary>
        ///     A contributor title.
        /// </summary>
        public static UserFlag Contributor
            => new()
            {
                Emoji = ":man_technologist:",
                Title = "Contributor",
                Description = "A contributor to Barriots codebase.",
                Type = FlagType.Contributor
            };

        /// <summary>
        ///     A lead developer title.
        /// </summary>
        public static UserFlag Developer
            => new()
            {
                Emoji = ":crown:",
                Title = "Developer",
                Description = "A lead developer of Barriot.",
                Type = FlagType.Developer
            };

        /// <summary>
        ///     Creates a command tier acknowledgement.
        /// </summary>
        /// <param name="tier"></param>
        /// <param name="range"></param>
        /// <returns>A command acknowledgement with provided tier.</returns>
        public static UserFlag CreateCommand(int tier, long range)
            => new()
            {
                Emoji = ":star:",
                Title = $"Commander (Tier: {tier}/10)",
                Description = $"Executed over {range} commands in total! {((tier < 10) ? $"\n*Next tier at: {range * 2}" : "")}*",
                Type = FlagType.Command
            };

        /// <summary>
        ///     Creates a component tier acknowledgement.
        /// </summary>
        /// <param name="tier"></param>
        /// <param name="range"></param>
        /// <returns>A command acknowledgement with provided tier.</returns>
        public static UserFlag CreateComponent(int tier, long range)
            => new()
            {
                Emoji = ":mouse_three_button:",
                Title = $"Component clicker (Tier: {tier}/10)",
                Description = $"Clicked over {range} components in total! {((tier < 10) ? $"\n*Next tier at: {range * 2}" : "")}*",
                Type = FlagType.Component
            };

        /// <summary>
        ///     Creates a champion tier acknowledgement.
        /// </summary>
        /// <param name="tier"></param>
        /// <param name="range"></param>
        /// <returns>A champion acknowledgement with provided tier.</returns>
        public static UserFlag CreateChampion(int tier, long range)
            => new()
            {
                Emoji = ":trophy:",
                Title = $"Champion (Tier: {tier}/10)",
                Description = $"Won over {range} challenges in total! {((tier < 10) ? $"\n*Next tier at: {range * 2}" : "")}*",
                Type = FlagType.Champion
            };

        /// <summary>
        ///     Creates a custom acknowledgement.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="emoji"></param>
        /// <param name="description"></param>
        /// <returns>A custom acknowledgement with provided values.</returns>
        public static UserFlag SetCustomFlag(string title, string emoji, string description)
            => new()
            {
                Title = title,
                Emoji = emoji,
                Description = description,
                Type = FlagType.Custom
            };

        #endregion

        /// <summary>
        ///     Gets the full user acknowledgement in string form.
        /// </summary>
        /// <returns>The full user acknowledgement.</returns>
        public override string ToString()
            => $"{Emoji} {Title}";
    }
}
