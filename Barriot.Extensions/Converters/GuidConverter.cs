using Discord;
using Discord.Interactions;

namespace Barriot.Extensions.Converters
{
    public class GuidConverter : TypeReader<Guid>
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services)
        {
            if (Guid.TryParse(option, out Guid id))
                return Task.FromResult(TypeConverterResult.FromSuccess(id));
            else
                throw new InvalidOperationException("Invalid Guid passed to the component handler."); // wont happen since it's passing from a newly generated Guid
        }
    }
}
