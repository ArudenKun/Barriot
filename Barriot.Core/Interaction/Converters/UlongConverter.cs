namespace Barriot.Interaction.Converters
{
    public class UlongConverter : TypeConverter<ulong>
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            string @string = (option.Value as string)!;

            if (ulong.TryParse(@string, out ulong value))
                return Task.FromResult(TypeConverterResult.FromSuccess(value));
            else
                return Task.FromResult(TypeConverterResult.FromSuccess(0ul));
        }
    }
}
