using Discord;
using Discord.Interactions;

namespace Barriot.Extensions.Converters
{
    public class TimeSpanComponentConverter : ComponentTypeConverter<TimeSpan>
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
            => Task.FromResult(TypeConverterResult.FromSuccess(TimeSpanExtensions.GetSpan(option.Value)));
    }
}
