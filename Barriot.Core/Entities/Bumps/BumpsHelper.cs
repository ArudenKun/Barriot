using Barriot.Data;
using MongoDB.Driver;

namespace Barriot.Entities.Bumps
{
    public static class BumpsHelper
    {
        private static readonly CollectionManager<BumpsEntity> _client = new("Bumps");

        public static async Task<bool> ModifyAsync(BumpsEntity bumps, UpdateDefinition<BumpsEntity> update)
        {
            if (bumps.State is EntityState.Deserializing)
                return false;

            if (bumps.State is EntityState.Deleted)
                throw new InvalidOperationException($"{nameof(bumps)} cannot be modified post-deletion.");

            return await _client.ModifyDocumentAsync(bumps, update);
        }

        public static async Task<bool> DeleteAsync(BumpsEntity user)
        {
            user.State = EntityState.Deleted;

            return await _client.DeleteDocumentAsync(user);
        }

        public static async Task<BumpsEntity> GetAsync(ulong id)
        {
            var document = await _client.FindDocumentAsync(x => x.UserId == id) ?? await CreateAsync(id);
            document.State = EntityState.Initialized;
            return document;
        }

        private static async Task<BumpsEntity> CreateAsync(ulong id)
        {
            var entity = new BumpsEntity(id);
            await _client.InsertDocumentAsync(entity);
            return entity;
        }
    }
}
