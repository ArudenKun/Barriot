namespace Barriot.Interaction.Modals
{
    public class EmbedColorModal : IModal
    {
        public string Title
            => "Set embed color";

        [InputLabel("Please choose a hex color.")]
        [ModalTextInput(customId: "embed-color", style: TextInputStyle.Short, placeholder: "#11806A", minLength: 1, maxLength: 7)]
        public Color Color { get; set; } = Color.Default;
    }
}
