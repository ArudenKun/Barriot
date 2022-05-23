using Barriot.Data;
using MongoDB.Driver;

namespace Barriot.Entities.Reminders
{
    internal static class RemindHelper
    {
        private static readonly CollectionManager<RemindEntity> _client = new("Reminders");

        public static async Task<bool> ModifyAsync(RemindEntity reminder, UpdateDefinition<RemindEntity> update)
        {
            if (reminder.State is EntityState.Deserializing)
                return false;

            if (reminder.State is EntityState.Deleted)
                throw new InvalidOperationException($"{nameof(reminder)} cannot be modified post-deletion.");

            return await _client.ModifyDocumentAsync(reminder, update);
        }

        public static async Task<RemindEntity> GetAsync(DateTime? time = null)
        {
            var entity = await _client.FindDocumentAsync(x => x.Expiration <= (time ?? DateTime.UtcNow));

            entity.State = EntityState.Initialized;

            return entity;
        }

        public static async IAsyncEnumerable<RemindEntity> GetManyAsync(ulong id)
        {
            var documents = await _client.FindManyDocumentsAsync(x => x.UserId == id);

            await foreach (var document in documents)
            {
                document.State = EntityState.Initialized;
                yield return document;
            }
        }

        public static async IAsyncEnumerable<RemindEntity> GetManyAsync(DateTime time)
        { 
            var documents = await _client.FindManyDocumentsAsync(x => x.Expiration <= time);

            await foreach (var document in documents)
            {
                document.State = EntityState.Initialized;
                yield return document;
            }
        }

        public static async Task<RemindEntity> CreateAsync(string message, TimeSpan span, ulong id, int frequency, TimeSpan toRepeat)
        {
            var entity = new RemindEntity(id, message, span, frequency, toRepeat);

            await _client.InsertDocumentAsync(entity);
            entity.State = EntityState.Initialized;
            return entity;
        }

        public static async Task<bool> DeleteAsync(RemindEntity entity)
        {
            entity.State = EntityState.Deleted;
            return await _client.DeleteDocumentAsync(entity);
        }
    }
}
