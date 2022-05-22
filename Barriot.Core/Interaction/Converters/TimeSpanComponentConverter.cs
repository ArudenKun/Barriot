using Barriot.Extensions;

namespace Barriot.Interaction.Converters
{
    public class TimeSpanComponentConverter : ComponentTypeConverter<TimeSpan>
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
            => Task.FromResult(TypeConverterResult.FromSuccess(TimeSpanExtensions.GetSpan(option.Value)));
    }
}
