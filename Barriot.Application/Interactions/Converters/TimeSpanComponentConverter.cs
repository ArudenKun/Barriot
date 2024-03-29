﻿using Barriot.Extensions;

namespace Barriot.Application.Interactions.Converters
{
    public class TimeSpanComponentConverter : ComponentTypeConverter<TimeSpan>
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
            => Task.FromResult(TypeConverterResult.FromSuccess(TimeExtensions.GetTimeSpan(option.Value)));
    }
}
