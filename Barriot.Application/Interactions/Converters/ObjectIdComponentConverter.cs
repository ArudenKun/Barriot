﻿using MongoDB.Bson;

namespace Barriot.Application.Interactions.Converters
{
    public class ObjectIdComponentConverter : TypeReader<ObjectId>
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services)
        {
            if (ObjectId.TryParse(option, out ObjectId id))
                return Task.FromResult(TypeConverterResult.FromSuccess(id));

            else
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "Unable to parse string to Object ID."));
        }
    }
}
