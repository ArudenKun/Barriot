using Barriot.Entities.Pins;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Barriot
{
    [BsonIgnoreExtraElements]
    public class PinEntity : IMutableEntity, IConcurrentlyAccessible<PinEntity>
    {
        /// <inheritdoc/>
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <inheritdoc/>
        [BsonIgnore]
        public EntityState State { get; set; }

        internal PinEntity(ulong userId, string messageUrl)
        {
            UserId = userId;
            _url = messageUrl;
            _pinDate = DateTime.UtcNow;
        }

        #region PinEntity

        /// <summary>
        ///     The ID of the user that pinned this message for themselves.
        /// </summary>
        public ulong UserId { get; set; }

        private string _url;
        /// <summary>
        ///     The link to the message this pin refers to.
        /// </summary>
        public string Url
        {
            get
                => _url;
            set
            {
                _url = value;
                _ = ModifyAsync(Builders<PinEntity>.Update.Set(x => x.Url, value));
            }
        }

        private DateTime _pinDate;
        /// <summary>
        ///     The date at which this message was pinned.
        /// </summary>
        public DateTime PinDate
        {
            get
                => _pinDate;
            set
            {
                _pinDate = value;
                _ = ModifyAsync(Builders<PinEntity>.Update.Set(x => x.PinDate, value));
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ModifyAsync(UpdateDefinition<PinEntity> update)
            => await PinHelper.ModifyAsync(this, update);

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync()
            => await PinHelper.DeleteAsync(this);

        public static async Task<List<PinEntity>> GetManyAsync(DateTime time)
            => await PinHelper.GetManyAsync(time).ToListAsync();

        public static async Task<List<PinEntity>> GetManyAsync(ulong userId)
            => await PinHelper.GetManyAsync(userId).ToListAsync();

        public static async Task<PinEntity> CreateAsync(ulong userId, string messageUrl)
            => await PinHelper.CreateAsync(userId, messageUrl);

        #endregion

        #region IDisposable
        void IDisposable.Dispose() { }
        #endregion

        #region IMutableEntity
        Task<bool> IMutableEntity.UpdateAsync()
            => throw new NotSupportedException();
        #endregion

        public override string ToString()
            => Url;
    }
}
