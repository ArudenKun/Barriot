using Barriot.Data;
using MongoDB.Driver;

namespace Barriot.Entities.Pins
{
    internal static class PinHelper
    {
        private static readonly CollectionManager<PinEntity> _client = new("Reminders");

        public static async Task<bool> ModifyAsync(PinEntity reminder, UpdateDefinition<PinEntity> update)
        {
            if (reminder.State is EntityState.Deserializing)
                return false;

            if (reminder.State is EntityState.Deleted)
                throw new InvalidOperationException($"{nameof(reminder)} cannot be modified post-deletion.");

            return await _client.ModifyDocumentAsync(reminder, update);
        }

        public static async IAsyncEnumerable<PinEntity> GetManyAsync(ulong id)
        {
            var documents = await _client.FindManyDocumentsAsync(x => x.UserId == id);

            await foreach (var document in documents)
            {
                document.State = EntityState.Initialized;
                yield return document;
            }
        }

        public static async IAsyncEnumerable<PinEntity> GetManyAsync(DateTime time)
        {
            var documents = await _client.FindManyDocumentsAsync(x => x.PinDate <= time);

            await foreach (var document in documents)
            {
                document.State = EntityState.Initialized;
                yield return document;
            }
        }

        public static async Task<PinEntity> CreateAsync(ulong userId, string messageUrl)
        {
            var entity = new PinEntity(userId, messageUrl);

            await _client.InsertDocumentAsync(entity);
            entity.State = EntityState.Initialized;
            return entity;
        }

        public static async Task<bool> DeleteAsync(PinEntity entity)
        {
            entity.State = EntityState.Deleted;
            return await _client.DeleteDocumentAsync(entity);
        }
    }
}
