using Barriot.Extensions;
using Discord;
using Discord.Interactions;

namespace Barriot.Extensions.Converters
{
    public class TimeSpanConverter : TypeConverter<TimeSpan>
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
            => Task.FromResult(TypeConverterResult.FromSuccess(TimeSpanExtensions.GetSpan((option.Value as string)!)));
    }
}
