using Barriot.Entities.Reminders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Barriot
{
    [BsonIgnoreExtraElements]
    public class RemindEntity : IMutableEntity, IConcurrentlyAccessible<RemindEntity>
    {
        /// <inheritdoc/>
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <inheritdoc/>
        [BsonIgnore]
        public EntityState State { get; set; } = EntityState.Deserializing;

        internal RemindEntity(ulong userId, string message, TimeSpan span, int frequency, TimeSpan toRepeat)
        {
            UserId = userId;
            _message = message;
            _expiration = DateTime.UtcNow.Add(span);
            _frequency = frequency;
            _spanToRepeat = toRepeat;
        }

        #region RemindEntity

        /// <summary>
        ///     The ID of the user that made this reminder.
        /// </summary>
        public ulong UserId { get; set; }

        private string _message;
        /// <summary>
        ///     The message the user wants to be reminded about.
        /// </summary>
        public string Message
        {
            get
                => _message;
            set
            {
                _message = value;
                _ = ModifyAsync(Builders<RemindEntity>.Update.Set(x => x.Message, value));
            }
        }

        private DateTime _expiration;
        /// <summary>
        ///     The time when this reminder should be sent initially.
        /// </summary>
        public DateTime Expiration
        {
            get
                => _expiration;
            set
            {
                _expiration = value;
                _ = ModifyAsync(Builders<RemindEntity>.Update.Set(x => x.Expiration, value));
            }
        }

        private int _frequency;
        /// <summary>
        ///     The amount of times the reminder should be repeated.
        /// </summary>
        public int Frequency
        {
            get
                => _frequency;
            set
            {
                _frequency = value;
                _ = ModifyAsync(Builders<RemindEntity>.Update.Set(x => x.Frequency, value));
            }
        }

        private TimeSpan _spanToRepeat;
        /// <summary>
        ///     The timespan for which this reminder should be repeated.
        /// </summary>
        public TimeSpan SpanToRepeat
        {
            get
                => _spanToRepeat;
            set
            {
                _spanToRepeat = value;
                _ = ModifyAsync(Builders<RemindEntity>.Update.Set(x => x.SpanToRepeat, value));
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync()
            => await RemindHelper.DeleteAsync(this);

        public async Task<bool> ModifyAsync(UpdateDefinition<RemindEntity> reminder)
            => await RemindHelper.ModifyAsync(this, reminder);

        public static async Task<RemindEntity> GetAsync(DateTime time)
            => await RemindHelper.GetAsync(time);

        public static async Task<List<RemindEntity>> GetManyAsync(IUser user)
            => await RemindHelper.GetManyAsync(user.Id).ToListAsync();

        public static async Task<List<RemindEntity>> GetManyAsync(DateTime time)
            => await RemindHelper.GetManyAsync(time).ToListAsync();

        public static async Task<RemindEntity> CreateAsync(string message, TimeSpan span, ulong target, int frequency = 1, TimeSpan? toRepeat = null)
            => await RemindHelper.CreateAsync(message, span, target, frequency, toRepeat ?? TimeSpan.FromDays(1));

        #endregion

        #region IDisposable
        void IDisposable.Dispose() { }
        #endregion

        #region IMutableEntity
        Task<bool> IMutableEntity.UpdateAsync()
            => throw new NotSupportedException();
        #endregion

        public override string ToString()
            => Message;
    }
}

