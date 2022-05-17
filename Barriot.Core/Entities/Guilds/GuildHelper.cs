using Barriot.Data;
using MongoDB.Driver;

namespace Barriot.Entities.Guilds
{
    internal static class GuildHelper
    {
        private static readonly CollectionManager<GuildEntity> _client = new("Guilds");

        public static async Task<bool> ModifyAsync(GuildEntity guild, UpdateDefinition<GuildEntity> update)
        {
            if (guild.State is EntityState.Deserializing)
                return false;

            if (guild.State is EntityState.Deleted)
                throw new InvalidOperationException($"{nameof(guild)} cannot be modified post-deletion.");

            return await _client.ModifyDocumentAsync(guild, update);
        }

        public static async Task<bool> DeleteAsync(GuildEntity user)
        {
            user.State = EntityState.Deleted;

            return await _client.DeleteDocumentAsync(user);
        }

        public static async Task<GuildEntity> GetAsync(ulong id)
        {
            var document = await _client.FindDocumentAsync(x => x.GuildId == id) ?? await CreateAsync(id);
            document.State = EntityState.Initialized;
            return document;
        }

        private static async Task<GuildEntity> CreateAsync(ulong id)
        {
            var entity = new GuildEntity(id);

            await _client.InsertDocumentAsync(entity);
            return entity;
        }
    }
}
