using Barriot.Data;
using MongoDB.Bson;

namespace Barriot.Entities.TicTacToe
{
    public class TicTacToeHelper
    {
        private static readonly CollectionManager<TicTacToeEntity> _client = new("TicTacToe");

        public static async Task<bool> UpdateAsync(TicTacToeEntity entity)
            => await _client.UpdateDocumentAsync(entity);

        public static async Task<TicTacToeEntity> GetAsync(DateTime? time = null)
            => await _client.FindDocumentAsync(x => x.Expiration <= (time ?? DateTime.UtcNow));

        public static async Task<TicTacToeEntity> GetAsync(ulong id, ObjectId objectId)
            => await _client.FindDocumentAsync(x => x.Players.Any(p => p.Equals(id)) && x.ObjectId.Equals(objectId));

        public static async Task<bool> DeleteManyAsync(DateTime time)
            => await _client.DeleteManyDocumentsAsync(x => x.Expiration <= time);

        public static async Task<TicTacToeEntity> CreateAsync(params ulong[] players)
        {
            var entity = new TicTacToeEntity
            {
                Expiration = DateTime.UtcNow.AddDays(1),
                Players = players
            };

            short x = 0;
            short y = 0;

            for (int i = 0; i < (entity.MaxX * entity.MaxY); i++)
            {
                entity.Positions.Add(TicTacToePosition.Build(x, y));
                x += 1;
                if (x == entity.MaxX)
                {
                    y += 1;
                    x = 0;
                }
            }
            await _client.InsertDocumentAsync(entity);
            return entity;
        }

        public static async Task<bool> DeleteAsync(TicTacToeEntity entity)
            => await _client.DeleteDocumentAsync(entity);
    }
}
