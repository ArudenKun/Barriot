using Barriot.Entities.TicTacToe;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Barriot
{
    [BsonIgnoreExtraElements]
    public class TicTacToeEntity : IGameEntity, IDisposable
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <inheritdoc/>
        public DateTime Expiration { get; set; }

        /// <summary>
        ///     The players participating in this game.
        /// </summary>
        public ulong[] Players { get; set; } = Array.Empty<ulong>();

        /// <summary>
        ///     The positions of this game.
        /// </summary>
        public List<TicTacToePosition> Positions { get; set; } = new();

        /// <inheritdoc/>
        public short MaxX { get; set; } = 3;

        /// <inheritdoc/>
        public short MaxY { get; set; } = 3;

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync()
            => await TicTacToeHelper.UpdateAsync(this);

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync()
            => await TicTacToeHelper.DeleteAsync(this);

        public static async Task<TicTacToeEntity> GetAsync(ulong userId, ObjectId objectId)
            => await TicTacToeHelper.GetAsync(userId, objectId);

        public async static Task<bool> DeleteManyAsync(DateTime? time = null)
            => await TicTacToeHelper.DeleteManyAsync(time ?? DateTime.UtcNow);

        public static async Task<TicTacToeEntity> CreateAsync(params ulong[] players)
            => await TicTacToeHelper.CreateAsync(players);

        public bool RowComplete(ulong userId)
        {
            for (int i = 0; i < MaxX; i++)
            {
                if (Positions.Where(pos => pos.X == i).All(x => x.SetBy == userId))
                    return true;

                if (Positions.Where(pos => pos.Y == i).All(x => x.SetBy == userId))
                    return true;
            }

            if (Positions.Where(pos => pos.X == pos.Y).All(x => x.SetBy == userId))
                return true;

            if (Positions.Where(pos => (pos.X + pos.Y) == (MaxX - 1)).All(x => x.SetBy == userId))
                return true;

            return false;
        }

        async void IDisposable.Dispose()
            => await UpdateAsync();
    }
}