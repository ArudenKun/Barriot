namespace Barriot.Interactions.Services
{
    public class UserService
    {
        private readonly List<CachedUser> _users;

        private readonly DiscordRestClient _client;

        public UserService(DiscordRestClient client)
        {
            _client = client;
            _users = new();
        }

        private class CachedUser
        {
            /// <summary>
            ///     When this user was last cached.
            /// </summary>
            public DateTime LastChecked { get; set; }

            /// <summary>
            ///     The entity to cache.
            /// </summary>
            public RestUser User { get; set; }

            public CachedUser(RestUser user)
            {
                User = user;
                LastChecked = DateTime.UtcNow;
            }
        }

        /// <summary>
        ///     Gets a user from cache if available, otherwise makes a call to the API.
        /// </summary>
        /// <param name="userId">The ID of the user to grab cache for.</param>
        /// <returns>A <see cref="RestUser"/> based on the values of </returns>
        public async Task<RestUser> GetOneAsync(ulong userId)
        {
            var entities = _users.Where(x => x.User.Id == userId);

            CachedUser entity;
            if (entities.Any())
            {
                entity = entities.First();
                if (DateTime.UtcNow.AddMinutes(15) <= entities.First().LastChecked)
                {
                    _users.Remove(entity);
                    entity = new(await _client.GetUserAsync(userId));
                    _users.Add(entity);
                }

                return entity.User;
            }

            entity = new(await _client.GetUserAsync(userId));
            _users.Add(entity);

            return entity.User;
        }
    }
}
