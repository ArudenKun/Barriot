namespace Barriot
{
    public record SelfAssignRole
    {
        /// <summary>
        ///     The link of the message this auto-role action will be triggered on.
        /// </summary>
        public string MessageLink { get; set; } = string.Empty;

        /// <summary>
        ///     The ID of the role this auto-role action will trigger.
        /// </summary>
        public ulong RoleId { get; set; }

        /// <summary>
        ///     The name of the role.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     The description of the role.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
