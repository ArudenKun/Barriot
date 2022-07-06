using Barriot.Entities.Polls;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Barriot
{
    [BsonIgnoreExtraElements]
    public class PollEntity : IMutableEntity
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <summary>
        ///     The ID of the message this poll exists in.
        /// </summary>
        public ulong MessageId { get; set; }

        /// <summary>
        ///     The options of this poll, with required data.
        /// </summary>
        public List<PollOption> Options { get; set; } = new();

        /// <summary>
        ///     User IDs of users that already replied to this poll.
        /// </summary>
        public List<ulong> AlreadyReplied { get; set; } = new();

        /// <summary>
        ///     The expiration date of this poll.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync()
            => await PollHelper.UpdateAsync(this);

        /// <inheritdoc />
        public async Task<bool> DeleteAsync()
            => await PollHelper.DeleteAsync(this);

        public static async Task<bool> DeleteManyAsync(DateTime? time = null)
            => await PollHelper.DeleteManyAsync(time ?? DateTime.UtcNow);

        public static async Task<PollEntity> GetAsync(ulong messageId)
            => await PollHelper.GetAsync(messageId);

        public static async Task<PollEntity> CreateAsync(ulong messageId, IEnumerable<PollOption> replies, DateTime? expiration = null)
            => await PollHelper.CreateAsync(messageId, replies, expiration);

        async void IDisposable.Dispose()
            => await UpdateAsync();

        public override string ToString()
            => $"{Expiration}";
    }
}