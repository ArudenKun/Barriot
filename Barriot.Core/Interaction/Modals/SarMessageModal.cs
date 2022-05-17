namespace Barriot.Interaction.Modals
{
    public class SarMessageModal : IModal
    {
        public string Title
            => "Add a new self-assign role to this server";

        public string MessageContent { get; set; } = string.Empty;

        public ulong Role { get; set; }

        public string Label { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
