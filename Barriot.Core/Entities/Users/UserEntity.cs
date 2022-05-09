using Barriot.Entities.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Barriot
{
    [BsonIgnoreExtraElements]
    public class UserEntity : IMutableEntity, IConcurrentlyAccessible<UserEntity>
    {
        /// <inheritdoc/>
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <inheritdoc/>
        [BsonIgnore]
        public EntityState State { get; set; } = EntityState.Deserializing;

        #region UserEntity

        /// <summary>
        ///     The Discord ID of this user.
        /// </summary>
        public ulong UserId { get; set; }

        private string _userName = "Unknown#0000";
        /// <summary>
        ///     Gets the username of this user.
        /// </summary>
        public string UserName
        {
            get
                => _userName;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.UserName, value));
                _userName = value;
            }
        }

        private bool _doEphemeral = true;
        /// <summary>
        ///     If messages are supposed to be sent ephemerally.
        /// </summary>
        public bool DoEphemeral
        {
            get
                => _doEphemeral;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.DoEphemeral, value));
                _doEphemeral = value;
            }
        }

        private bool _isBlackListed = false;
        /// <summary>
        ///     Wether or not this user is blacklisted. If so, user is unable to execute any functions, including buttons & dropdowns.
        /// </summary>
        public bool IsBlacklisted
        {
            get
                => _isBlackListed;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.IsBlacklisted, value));
                _isBlackListed = value;
            }
        }

        private string _preferredLang = "en|English";
        /// <summary>
        ///     The preferred language of this user.
        /// </summary>
        public string PreferredLang
        {
            get
                => _preferredLang;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.PreferredLang, value));
                _preferredLang = value;
            }
        }

        private long _commandsExecuted = 0;
        /// <summary>
        ///     How many commands this user has executed.
        /// </summary>
        public long CommandsExecuted
        {
            get
                => _commandsExecuted;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.CommandsExecuted, value));
                _commandsExecuted = value;
            }
        }

        private long _buttonsPressed = 0;
        /// <summary>
        ///     How many buttons have been pressed by this user.
        /// </summary>
        public long ButtonsPressed
        {
            get
                => _buttonsPressed;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.ButtonsPressed, value));
                _buttonsPressed = value;
            }
        }

        private long _gamesWon = 0;
        /// <summary>
        ///     How many minigames this user has won.
        /// </summary>
        public long GamesWon
        {
            get
                => _gamesWon;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.GamesWon, value));
                _gamesWon = value;
            }
        }

        private string _lastCommand = "None";
        /// <summary>
        ///     The last command this user has executed.
        /// </summary>
        public string LastCommand
        {
            get
                => _lastCommand;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.LastCommand, value));
                _lastCommand = value;
            }
        }

        private List<string> _inbox = new();
        /// <summary>
        ///     A list of messages targetting this user.
        /// </summary>
        public List<string> Inbox
        {
            get
                => _inbox;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.Inbox, value));
                _inbox = value;
            }
        }

        private List<UserFlag> _flags = new();
        /// <summary>
        ///     A list of all user flags.
        /// </summary>
        public List<UserFlag> Flags
        {
            get
                => _flags;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.Flags, value));
                _flags = value;
            }
        }

        private DateTime _lastVotedAt;
        /// <summary>
        ///     When the user last voted.
        /// </summary>
        public DateTime LastVotedAt
        {
            get
                => _lastVotedAt;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.LastVotedAt, value));
                _lastVotedAt = value;
            }
        }

        private long _votes = 0;
        /// <summary>
        ///     How many times this user has voted in total.
        /// </summary>
        public long Votes
        {
            get
                => _votes;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.Votes, value));
                _votes = value;
            }
        }

        private long _monthlyVotes = 0;
        /// <summary>
        ///     How many times this user has voted this month.
        /// </summary>
        public long MonthlyVotes
        {
            get
                => _monthlyVotes;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.MonthlyVotes, value));
                _monthlyVotes = value;
            }
        }

        private uint _color = Discord.Color.Blue.RawValue;
        /// <summary>
        ///     The user's embed color.
        /// </summary>
        public uint Color
        {
            get
                => _color;
            set
            {
                _ = ModifyAsync(Builders<UserEntity>.Update.Set(x => x.Color, value));
                _color = value;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync()
            => await UserHelper.DeleteAsync(this);

        /// <inheritdoc/>
        public async Task<bool> ModifyAsync(UpdateDefinition<UserEntity> update)
            => await UserHelper.ModifyAsync(this, update);

        /// <summary>
        ///     Check if the user has a certain acknowledgement.
        /// </summary>
        /// <param name="type"></param>
        /// <returns><see langword="true"/> if the user has provided flag. <see langword="false"/> if not.</returns>
        public bool HasFlag(FlagType type)
            => Flags.Any(x => x.Type == type);

        /// <summary>
        ///     Checks if the user has voted within the last 24 hours.
        /// </summary>
        /// <returns><see langword="true"/> if the user has voted within the last 24 hours. <see langword="false"/> if not.</returns>
        public bool HasVoted()
            => (DateTime.UtcNow - LastVotedAt).TotalDays < 1d;

        /// <summary>
        ///     Get the user from the DB.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The <see cref="UserEntity"/> belonging to provided Discord ID.</returns>
        public static async Task<UserEntity> GetAsync(ulong id)
            => await UserHelper.GetAsync(id);

        /// <summary>
        ///     Get the user from the DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The <see cref="UserEntity"/> belonging to provided Discord user.</returns>
        public static async Task<UserEntity> GetAsync(IUser user)
            => await UserHelper.GetAsync(user.Id);

        /// <summary>
        ///     Gets all users at once.
        /// </summary>
        /// <returns>The list of all <see cref="UserEntity"/> instances currently in the collection.</returns>
        public static async Task<List<UserEntity>> GetAllAsync()
            => await UserHelper.GetAllAsync().ToListAsync();

        #endregion

        #region IDisposable
        void IDisposable.Dispose() { }
        #endregion

        #region IMutableEntity
        Task<bool> IMutableEntity.UpdateAsync()
            => throw new NotSupportedException();
        #endregion

        /// <summary>
        ///     Returns the mention of this user.
        /// </summary>
        /// <returns>A Discord user mention.</returns>
        public override string ToString()
            => $"<@{UserId}>";
    }
}
