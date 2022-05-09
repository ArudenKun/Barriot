using Barriot.Data;

namespace Barriot.Entities.Polls
{
    internal static class PollHelper
    {
        private static readonly CollectionManager<PollEntity> _client = new("Polls");

        public static async Task<bool> UpdateAsync(PollEntity entity)
            => await _client.UpdateDocumentAsync(entity);

        public static async Task<PollEntity> GetAsync(ulong messageId)
            => await _client.FindDocumentAsync(x => x.MessageId == messageId);

        public static async Task<PollEntity> CreateAsync(ulong messageId, IEnumerable<PollOption> options, DateTime? expiration = null)
        {
            var entity = new PollEntity
            {
                MessageId = messageId,
                Options = options.ToList(),
                Expiration = expiration ?? DateTime.UtcNow.AddDays(15)
            };
            await _client.InsertDocumentAsync(entity);
            return entity;
        }

        public static async Task<bool> DeleteAsync(PollEntity entity)
            => await _client.DeleteDocumentAsync(entity);

        public static async Task<bool> DeleteManyAsync(DateTime time)
            => await _client.DeleteManyDocumentsAsync(x => x.Expiration <= time);
    }
}
