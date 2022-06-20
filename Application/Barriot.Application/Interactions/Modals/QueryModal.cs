namespace Barriot.Application.Interactions.Modals
{
    public class QueryModal<TInput> : IModal where TInput : notnull
    {
        public string Title
            => "";

        [ModalTextInput("entry")]
        public TInput? Result { get; set; }
    }
}
