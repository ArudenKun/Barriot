namespace Barriot.Interaction.Modals
{
    public class FlagModal : IModal
    {
        public string Title
            => "Create a custom acknowledgement";

        [InputLabel("The acknowledgement title.")]
        [ModalTextInput("ack-title", TextInputStyle.Paragraph, "Friend", 1, 40)]
        public string Name { get; set; } = string.Empty;

        [InputLabel("An emote to match the title.")]
        [ModalTextInput("ack-emoji", TextInputStyle.Short, ":heart:", 1, 40)]
        public string Emoji { get; set; } = string.Empty;

        [InputLabel("The description of this emote.")]
        [ModalTextInput("ack-desc", TextInputStyle.Paragraph, "A cool user", 1, 1900)]
        public string Description { get; set; } = string.Empty;
    }
}
