namespace Barriot.Interaction.Modals
{
    public class SarMakeModal : IModal
    {
        public string Title
            => "Add a new self-assign role";

        [InputLabel("Role ID:")]
        [ModalTextInput("id", TextInputStyle.Short, "880140894983041085")]
        public string Role { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("Role name:")]
        [ModalTextInput("name", TextInputStyle.Short, "Announcements", 1, 90)]
        public string Label { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("Role description:")]
        [ModalTextInput("description", TextInputStyle.Short, "Receive announcement pings.", 1, 120)]
        public string Description { get; set; } = string.Empty;
    }
}
