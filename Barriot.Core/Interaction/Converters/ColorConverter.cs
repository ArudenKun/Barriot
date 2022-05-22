using System.Globalization;

namespace Barriot.Interaction.Converters
{
    public class ColorConverter : ComponentTypeConverter<Color>
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
        {
            if (uint.TryParse(option.Value.Replace("#", " ").Trim(), NumberStyles.HexNumber, null, out uint result))
                return Task.FromResult(TypeConverterResult.FromSuccess(new Color(result)));

            else
                return Task.FromResult(TypeConverterResult.FromSuccess(Color.Default));
        }
    }
}
