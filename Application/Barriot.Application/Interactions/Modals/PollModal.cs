namespace Barriot.Application.Interactions.Modals
{
    public class PollModal : IModal
    {
        public string Title
            => "Poll";

        [InputLabel("The question you want to ask the community.")]
        [ModalTextInput("ping-desc", TextInputStyle.Paragraph, "Do you like pineapple on pizza?", 1, 1900)]
        public string Description { get; set; } = "A question to the community!";

        [InputLabel("The first option.")]
        [ModalTextInput("ping-opt-1", TextInputStyle.Short, "Yes!", 1, 80)]
        public string Option1 { get; set; } = string.Empty;

        [InputLabel("The second option.")]
        [ModalTextInput("ping-opt-2", TextInputStyle.Short, "No way!", 1, 80)]
        public string Option2 { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("The third option.")]
        [ModalTextInput("ping-opt-3", TextInputStyle.Short, "Sometimes...", 1, 80)]
        public string? Option3 { get; set; } = null;

        [RequiredInput(false)]
        [InputLabel("The fourth option.")]
        [ModalTextInput("ping-opt-4", TextInputStyle.Short, "Don't want to say", 1, 80)]
        public string? Option4 { get; set; } = null;

        // TODO
        // Option 5 has been removed as modals do not support more than 5 text inputs currently.
        // If this is changed in the future, this should be re-implemented
    }
}
