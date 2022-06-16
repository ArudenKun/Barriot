using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Barriot.Data
{
    public sealed class DatabaseManager
    {
        private static MongoClient? _client;
        private static MongoDatabaseBase? _database;

        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseManager> _logger;

        public DatabaseManager(MongoClient client, IConfiguration configuration, ILogger<DatabaseManager> logger)
        {
            _logger = logger;
            _client = client;
            _configuration = configuration;
        }

        public async Task<bool> ConfigureAsync()
        {
            if (_client != null)
            {
                _database = _client.GetDatabase(_configuration["DbName"]) as MongoDatabaseBase;

                if (!IsConnected)
                    _logger.LogError("Database connection unsuccesful.");

                else
                {
                    _logger.LogInformation("Database connection succesful.");
                    return true;
                }
            }
            else
                _logger.LogError("Database client is null.");

            await Task.CompletedTask;
            return false;
        }

        /// <summary>
        ///     Checks if the database is connected or not.
        /// </summary>
        public static bool IsConnected
        {
            get
            {
                try
                {
                    _client?.ListDatabaseNames();
                    return true;
                }
                catch (MongoException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Runs mongo commands as if in shell.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <returns>The resulting string.</returns>
        public static string RunCommand(string command)
        {
            try
            {
                var result = _database?.RunCommand<BsonDocument>(BsonDocument.Parse(command));
                return result.ToJson();
            }
            catch (Exception ex) when (ex is FormatException or MongoCommandException)
            {
                throw;
            }
        }

        /// <summary>
        ///     Extracts a collection and its current data from the database collection.
        /// </summary>
        /// <typeparam name="TEntity">The entity to base this collection on.</typeparam>
        /// <param name="collection">The name of the collection.</param>
        /// <returns>An instance of <see cref="MongoCollectionBase{TDocument}"/></returns>
        /// <exception cref="NullReferenceException">Thrown if no collection was found.</exception>
        public static MongoCollectionBase<TEntity> GetCollection<TEntity>(string collection) where TEntity : IEntity
            => _database?.GetCollection<TEntity>(collection) as MongoCollectionBase<TEntity>
            ?? throw new NullReferenceException();
    }
}
