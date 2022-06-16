using Barriot.Entities.Pins;
using Barriot.Models;
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
        public EntityState State { get; set; } = EntityState.Deserializing;

        internal PinEntity(ulong userId, JumpUrl url, string reason)
        {
            UserId = userId;
            _url = url.Url;
            _pinDate = DateTime.UtcNow;
            _channelId = url.ChannelId;
            _messageId = url.MessageId;
            _reason = reason;
        }

        #region PinEntity

        /// <summary>
        ///     The ID of the user that pinned this message for themselves.
        /// </summary>
        public ulong UserId { get; set; }

        private string _reason;
        /// <summary>
        ///     The reason of this pin.
        /// </summary>
        public string Reason
        {
            get
                => _reason;
            set
            {
                _reason = value;
                _ = ModifyAsync(Builders<PinEntity>.Update.Set(x => x.Reason, value));
            }
        }

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

        private ulong _channelId;
        /// <summary>
        /// 
        /// </summary>
        public ulong ChannelId
        {
            get
                => _channelId;
            set
            {
                _channelId = value;
                _ = ModifyAsync(Builders<PinEntity>.Update.Set(x => x.ChannelId, value));
            }
        }

        private ulong _messageId;
        /// <summary>
        /// 
        /// </summary>
        public ulong MessageId
        {
            get
                => _messageId;
            set
            {
                _messageId = value;
                _ = ModifyAsync(Builders<PinEntity>.Update.Set(x => x.MessageId, value));
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

        public static async Task<PinEntity> CreateAsync(ulong userId, JumpUrl messageUrl, string reason)
            => await PinHelper.CreateAsync(userId, messageUrl, reason);

        #endregion

        #region IDisposable
        void IDisposable.Dispose() { }
        #endregion

        #region IMutableEntity
        Task<bool> IMutableEntity.UpdateAsync()
            => throw new NotSupportedException();
        #endregion

        public override string ToString()
            => Url.ToString();
    }
}
