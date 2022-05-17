using Barriot.Data;
using MongoDB.Driver;

namespace Barriot.Entities.Users
{
    internal static class UserHelper
    {
        private static readonly CollectionManager<UserEntity> _client = new("Users");

        public static async Task<bool> ModifyAsync(UserEntity user, UpdateDefinition<UserEntity> update)
        {
            if (user.State is EntityState.Deserializing)
                return false;

            if (user.State is EntityState.Deleted)
                throw new InvalidOperationException($"{nameof(user)} cannot be modified post-deletion.");

            return await _client.ModifyDocumentAsync(user, update);
        }

        public static async Task<bool> DeleteAsync(UserEntity user)
        {
            user.State = EntityState.Deleted;

            return await _client.DeleteDocumentAsync(user);
        }

        public static async Task<UserEntity> GetAsync(ulong id)
        {
            var document = await _client.FindDocumentAsync(x => x.UserId == id) ?? await CreateAsync(id);
            document.State = EntityState.Initialized;
            return document;
        }

        private static async Task<UserEntity> CreateAsync(ulong id)
        {
            var entity = new UserEntity(id);
            await _client.InsertDocumentAsync(entity);
            return entity;
        }

        public static async IAsyncEnumerable<UserEntity> GetAllAsync()
        {
            var documents = await _client.GetAllDocumentsAsync();

            await foreach (var document in documents)
            {
                document.State = EntityState.Initialized;
                yield return document;
            }
        }
    }
}