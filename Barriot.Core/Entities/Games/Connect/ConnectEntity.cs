using Barriot.Entities.Connect;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Barriot
{
    [BsonIgnoreExtraElements]
    public class ConnectEntity : IGameEntity, IDisposable
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <inheritdoc/>
        public DateTime Expiration { get; set; }

        /// <inheritdoc/>
        public ulong[] Players { get; set; } = Array.Empty<ulong>();

        /// <summary>
        ///     The positions in this game.
        /// </summary>
        public ConnectPosition[,] Positions { get; set; } = null!;

        /// <inheritdoc/>
        public short MaxX { get; set; } = 7;

        /// <inheritdoc/>
        public short MaxY { get; set; } = 6;

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync()
            => await ConnectHelper.UpdateAsync(this);

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync()
            => await ConnectHelper.DeleteAsync(this);

        /// <summary>
        ///     Creates a game based on the players provided.
        /// </summary>
        /// <param name="players">The players to create the game with.</param>
        /// <returns>An initialized instance of the game.</returns>
        public async static Task<ConnectEntity> CreateAsync(params ulong[] players)
            => await ConnectHelper.CreateAsync(players);

        /// <summary>
        ///     Gets an already existing game based on its object ID & player(s).
        /// </summary>
        /// <param name="userId">The player to get a game for.</param>
        /// <param name="objectId">The ID of the game.</param>
        /// <returns>The instance of the game stored in DB.</returns>
        public async static Task<ConnectEntity> GetAsync(ulong userId, ObjectId objectId)
            => await ConnectHelper.GetAsync(userId, objectId);

        /// <summary>
        ///     Deletes all instances of this gamemode that have expired.
        /// </summary>
        /// <param name="expiration">The date of expiration.</param>
        /// <returns>True if successfull. False if not.</returns>
        public async static Task<bool> DeleteManyAsync(DateTime? expiration = null)
            => await ConnectHelper.DeleteManyAsync(expiration ?? DateTime.UtcNow);

        /// <summary>
        ///     Checks if a position is filled.
        /// </summary>
        /// <param name="x"></param>
        /// <returns>True if the position is filled. False if not.</returns>
        public bool IsFilled(int x)
            => Positions[x, 0].SetBy != 0;

        /// <summary>
        ///     Sets and checks the position provided.
        /// </summary>
        /// <param name="userId">The ID to set the tile for.</param>
        /// <param name="xx">The X value of this column.</param>
        /// <returns>True if a player has won the game. False if not.</returns>
        public bool SetAndCheck(ulong userId, short xx)
        {
            xx -= 1;
            var yy = 0;
            for (int i = 0; i < MaxY; i++)
            {
                if (Positions[xx, yy].SetBy == 0)
                    yy++;
            }
            yy--;
            Positions[xx, yy].SetBy = userId;
            Positions[xx, yy].Emoji = (Players.First() != userId) ? ":yellow_square:" : ":red_square:";

            var x = xx;
            var y = yy;

            // x calc
            for (int i = 0; i < MaxX; i++)
            {
                x += 1;
                if (x == MaxX || Positions[x, y].SetBy != userId)
                    break;
            }
            short count = 0;
            for (int i = MaxX; i > 0; i--)
            {
                x--;
                if (x == -1)
                    break;
                if (Positions[x, y].SetBy != userId)
                    break;
                count++;
                if (count == 4)
                    return true;
            }

            x = xx;
            y = yy;
            //y calc
            for (int i = 0; i < MaxY; i++)
            {
                y++;
                if (y == MaxY)
                    break;
                if (Positions[x, y].SetBy != userId)
                    break;
            }
            count = 0;
            for (int i = MaxY; i > 0; i--)
            {
                y--;
                if (y == -1 || x == -1)
                    break;
                if (Positions[x, y].SetBy != userId)
                    break;
                count++;
                if (count == 4)
                    return true;
            }

            x = xx;
            y = yy;
            for (int i = 0; i < MaxX; i++)
            {
                y++;
                x++;
                if (y == MaxY || x == MaxX)
                    break;
                if (Positions[x, y].SetBy != userId)
                    break;
            }
            count = 0;
            for (int i = MaxX; i > 0; i--)
            {
                y--;
                x--;
                if (x == -1 || y == -1)
                    break;
                if (Positions[x, y].SetBy != userId)
                    break;
                count += 1;
                if (count == 4)
                    return true;
            }

            x = xx;
            y = yy;
            for (int i = 0; i < MaxX; i++)
            {
                y++;
                x--;
                if (y == MaxY || x == -1)
                    break;
                if (Positions[x, y].SetBy != userId)
                    break;
            }
            count = 0;
            for (int i = MaxX; i > 0; i--)
            {
                y--;
                x++;
                if (x == MaxX || y == -1)
                    break;
                if (Positions[x, y].SetBy != userId)
                    break;
                count += 1;
                if (count == 4)
                    return true;
            }

            return false;
        }

        async void IDisposable.Dispose()
            => await UpdateAsync();
    }
}
