using Barriot.Entities.Reminders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Barriot
{
    [BsonIgnoreExtraElements]
    public class RemindEntity : IMutableEntity
    {
        /// <inheritdoc/>
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <summary>
        ///     The ID of the user that made this reminder.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        ///     The message the user wants to be reminded about.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        ///     The time when this reminder should be sent initially.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        ///     The amount of times the reminder should be repeated.
        /// </summary>
        public int Frequency { get; set; } = 1;

        /// <summary>
        ///     The timespan for which this reminder should be repeated.
        /// </summary>
        public TimeSpan SpanToRepeat { get; set; } = TimeSpan.FromDays(1);

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync()
            => await RemindHelper.DeleteAsync(this);

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync()
            => await RemindHelper.UpdateAsync(this);

        public static async Task<RemindEntity> GetAsync(DateTime time)
            => await RemindHelper.GetAsync(time);

        public static async Task<IAsyncEnumerable<RemindEntity>> GetManyAsync(IUser user)
            => await RemindHelper.GetManyAsync(user.Id);

        public static async Task<IAsyncEnumerable<RemindEntity>> GetManyAsync(DateTime time)
            => await RemindHelper.GetManyAsync(time);

        public static async Task<RemindEntity> CreateAsync(string message, TimeSpan span, ulong target, int frequency = 1, TimeSpan? toRepeat = null)
            => await RemindHelper.CreateAsync(message, span, target, frequency, toRepeat ?? TimeSpan.FromDays(1));

        async void IDisposable.Dispose()
            => await UpdateAsync();

        public override string ToString()
            => Message;
    }
}

