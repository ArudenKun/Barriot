namespace Barriot.Interactions.Modals
{
    public class SudoEvalModal : IModal
    {
        public string Title
            => "Evaluate or run a C# script.";

        [InputLabel("The script to run")]
        [ModalTextInput("script", TextInputStyle.Paragraph, "Print(1);")]
        public string Script { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("Additional usings")]
        [ModalTextInput("usings", TextInputStyle.Short, "Discord, Discord.Rest", 0)]
        public string Usings { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("Additional dll references")]
        [ModalTextInput("references", TextInputStyle.Short, "Discord.Net.Core.dll, Discord.Net.Rest.dll")]
        public string References { get; set; } = string.Empty;
    }
}
