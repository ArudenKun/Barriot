using Barriot.Interaction;

namespace Barriot.Models
{
    public class EvalContext
    {
        public BarriotInteractionContext Context { get; }

        public string? Result { get; private set; } = null;

        public string? Attachment { get; private set; } = null;

        public EvalContext(BarriotInteractionContext context)
            => Context = context;

        public void Print(object result)
            => Result = result.ToString();

        public void Attach(object attachment)
            => Attachment = attachment.ToString();
    }
}
