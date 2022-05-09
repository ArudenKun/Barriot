using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Barriot.Data
{
    internal class CollectionManager<TEntity> where TEntity : IEntity
    {
        private readonly MongoCollectionBase<TEntity> _collection;

        public CollectionManager(string collection)
        {
            if (DatabaseManager.IsConnected)
                _collection = DatabaseManager.GetCollection<TEntity>(collection);
            else throw new ArgumentNullException(nameof(collection));
        }

        public async Task InsertDocumentAsync(TEntity document)
            => await _collection.InsertOneAsync(document);

        public async Task InsertDocumentsAsync(IEnumerable<TEntity> documents)
            => await _collection.InsertManyAsync(documents);

        public async Task InsertOrUpdateDocumentAsync(TEntity document)
        {
            if (document.ObjectId == ObjectId.Empty)
                await _collection.InsertOneAsync(document);
            else
                await _collection.ReplaceOneAsync(x => x.ObjectId == document.ObjectId, document);
        }

        public async Task<bool> UpdateDocumentAsync(TEntity document)
        {
            var entity = await (await _collection.FindAsync(x => x.ObjectId == document.ObjectId))
                .FirstOrDefaultAsync();

            if (entity is not null)
            {
                await _collection.ReplaceOneAsync(x => x.ObjectId == document.ObjectId, document);
                return true;
            }
            return false;
        }

        public async Task<bool> ModifyDocumentAsync(TEntity document, UpdateDefinition<TEntity> update)
            => (await _collection.UpdateOneAsync(x => x.ObjectId == document.ObjectId, update)).IsAcknowledged;

        public async Task<TEntity> GetFirstDocumentAsync()
            => await (await _collection.FindAsync(new BsonDocument())).FirstOrDefaultAsync();

        public async Task<IAsyncEnumerable<TEntity>> GetAllDocumentsAsync()
            => (await _collection.FindAsync(new BsonDocument())).ToEnumerable().ToAsyncEnumerable();

        public async Task<bool> DeleteDocumentAsync(TEntity document)
            => (await _collection.DeleteOneAsync(x => x.ObjectId == document.ObjectId)).IsAcknowledged;

        public async Task<bool> DeleteManyDocumentsAsync(Expression<Func<TEntity, bool>> filter)
            => (await _collection.DeleteManyAsync<TEntity>(filter)).IsAcknowledged;

        public async Task<TEntity> FindDocumentAsync(Expression<Func<TEntity, bool>> filter)
            => await (await _collection.FindAsync(filter)).FirstOrDefaultAsync();

        public async Task<IAsyncEnumerable<TEntity>> FindManyDocumentsAsync(Expression<Func<TEntity, bool>> filter)
            => (await _collection.FindAsync(filter)).ToEnumerable().ToAsyncEnumerable();
    }
}
