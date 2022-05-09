using Barriot.Data;

namespace Barriot.Entities.Reminders
{
    internal static class RemindHelper
    {
        private static readonly CollectionManager<RemindEntity> _client = new("Reminders");

        public static async Task<bool> UpdateAsync(RemindEntity entity)
            => await _client.UpdateDocumentAsync(entity);

        public static async Task<RemindEntity> GetAsync(DateTime? time = null)
            => await _client.FindDocumentAsync(x => x.Expiration <= (time ?? DateTime.UtcNow));

        public static async Task<IAsyncEnumerable<RemindEntity>> GetManyAsync(ulong id)
            => await _client.FindManyDocumentsAsync(x => x.UserId == id);

        public static async Task<IAsyncEnumerable<RemindEntity>> GetManyAsync(DateTime time)
            => await _client.FindManyDocumentsAsync(x => x.Expiration <= time);

        public static async Task<RemindEntity> CreateAsync(string message, TimeSpan span, ulong id, int frequency, TimeSpan toRepeat)
        {
            var entity = new RemindEntity
            {
                UserId = id,
                Message = message,
                Frequency = frequency,
                SpanToRepeat = toRepeat,
                Expiration = DateTime.UtcNow + span
            };
            await _client.InsertDocumentAsync(entity);
            return entity;
        }

        public static async Task<bool> DeleteAsync(RemindEntity entity)
            => await _client.DeleteDocumentAsync(entity);
    }
}
