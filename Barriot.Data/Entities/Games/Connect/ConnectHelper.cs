using Barriot.Data;
using MongoDB.Bson;

namespace Barriot.Entities.Connect
{
    public class ConnectHelper
    {
        private static readonly CollectionManager<ConnectEntity> _client = new("Connect");

        public static async Task<bool> UpdateAsync(ConnectEntity entity)
            => await _client.UpdateDocumentAsync(entity);

        public static async Task<ConnectEntity> GetAsync(DateTime? time = null)
            => await _client.FindDocumentAsync(x => x.Expiration <= (time ?? DateTime.UtcNow));

        public static async Task<ConnectEntity> GetAsync(ulong id, ObjectId objectId)
            => await _client.FindDocumentAsync(x => x.Players.Any(p => p.Equals(id)) && x.ObjectId.Equals(objectId));

        public static async Task<bool> DeleteManyAsync(DateTime time)
            => await _client.DeleteManyDocumentsAsync(x => x.Expiration <= time);

        public static async Task<ConnectEntity> CreateAsync(params ulong[] players)
        {
            var entity = new ConnectEntity
            {
                Expiration = DateTime.UtcNow.AddDays(1),
                Players = players
            };

            entity.Positions = new ConnectPosition[entity.MaxX, entity.MaxY];

            short x = 0;
            short y = 0;
            for (int i = 0; i < (entity.MaxX * entity.MaxY); i++)
            {
                entity.Positions[x, y] = ConnectPosition.Build(x, y);
                x += 1;
                if (x == entity.MaxX)
                {
                    x = 0;
                    y += 1;
                }
            }
            await _client.InsertDocumentAsync(entity);
            return entity;
        }

        public static async Task<bool> DeleteAsync(ConnectEntity entity)
            => await _client.DeleteDocumentAsync(entity);
    }
}
