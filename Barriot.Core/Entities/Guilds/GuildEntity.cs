using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Barriot.Entities.Guilds;
using System.Collections.ObjectModel;

namespace Barriot
{
    [BsonIgnoreExtraElements]
    public class GuildEntity : IConcurrentlyAccessible<GuildEntity>, IMutableEntity
    {
        /// <inheritdoc/>
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <inheritdoc/>
        [BsonIgnore]
        public EntityState State { get; set; } = EntityState.Deserializing;

        internal GuildEntity(ulong id)
        {
            _guildId = id;
            _selfRoleMessages = new();
        }

        #region GuildEntity

        private ulong _guildId;
        /// <summary>
        ///     The ID of the guild in question.
        /// </summary>
        public ulong GuildId
        {
            get
                => _guildId;
            set
            {
                _guildId = value;
                _ = ModifyAsync(Builders<GuildEntity>.Update.Set(x => x.GuildId, value));
            }
        }

        private List<SelfAssignMessage> _selfRoleMessages;
        /// <summary>
        ///     The auto-role relations in this guild.
        /// </summary>
        public List<SelfAssignMessage> SelfRoleMessages
        {
            get
                => _selfRoleMessages;
            set
            {
                _selfRoleMessages = value;
                _ = ModifyAsync(Builders<GuildEntity>.Update.Set(x => x.SelfRoleMessages, value));
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync()
            => await GuildHelper.DeleteAsync(this);

        /// <inheritdoc/>
        public async Task<bool> ModifyAsync(UpdateDefinition<GuildEntity> update)
            => await GuildHelper.ModifyAsync(this, update);

        /// <summary>
        ///     Gets a <see cref="GuildEntity"/> based on provided guild id.
        /// </summary>
        /// <param name="id">The guild id to get an entity for.</param>
        /// <returns>A <see cref="GuildEntity"/>, will be a new instance if no instance was found in the DB.</returns>
        public static async Task<GuildEntity> GetAsync(ulong id)
            => await GuildHelper.GetAsync(id);

        /// <summary>
        ///     Gets a <see cref="GuildEntity"/> based on provided guild.
        /// </summary>
        /// <param name="id">The guild to get an entity for.</param>
        /// <returns>A <see cref="GuildEntity"/>, will be a new instance if no instance was found in the DB.</returns>
        public static async Task<GuildEntity> GetAsync(IGuild guild)
            => await GuildHelper.GetAsync(guild.Id);

        #endregion

        #region IDisposable
        void IDisposable.Dispose() { }
        #endregion

        #region IMutableEntity
        Task<bool> IMutableEntity.UpdateAsync()
            => throw new NotSupportedException();
        #endregion
    }
}
