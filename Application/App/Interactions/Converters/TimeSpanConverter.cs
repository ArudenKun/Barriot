using Barriot.Extensions;

namespace Barriot.Interactions.Converters
{
    public class TimeSpanConverter : TypeConverter<TimeSpan>
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
            => Task.FromResult(TypeConverterResult.FromSuccess(TimeExtensions.GetTimeSpan((option.Value as string)!)));
    }
}
