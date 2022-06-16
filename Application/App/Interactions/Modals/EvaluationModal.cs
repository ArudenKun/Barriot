namespace Barriot.Interactions.Modals
{
    public class EvaluationModal : IModal
    {
        public string Title
            => "";

        [ModalTextInput("verification")]
        public string Verification { get; set; } = string.Empty;

        [ModalTextInput("code")]
        public string Script { get; set; } = string.Empty;

        [ModalTextInput("imports")]
        public string Imports { get; set; } = string.Empty;

        [ModalTextInput("references")]
        public string References { get; set; } = string.Empty;
    }
}
