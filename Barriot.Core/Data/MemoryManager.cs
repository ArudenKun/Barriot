using Barriot.Data.Memory;

namespace Barriot
{
    public class MemoryManager
    {
        private readonly Dictionary<Guid, IMemoryEntity> _data;

        public MemoryManager()
        {
            _data = new();
        }

        /// <summary>
        ///     Adds an instance of <see cref="IMemoryEntity"/> to the memory cache.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The <see cref="Guid"/> with which this entity was added.</returns>
        public Guid AddData(IMemoryEntity entity)
        {
            Guid id;
            while (true)
            {
                id = new Guid();
                if (_data.TryAdd(id, entity))
                    return id;
            }
        }

        /// <summary>
        ///     Attempts to get an instance of <see cref="IMemoryEntity"/> from the memory cache.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> to get a value for.</param>
        /// <param name="data">An instance of <see cref="IMemoryEntity"/>.</param>
        /// <param name="prune">Wether or not to prune the entity from memory once retrieved. Can be safely used to finalize the entity.</param>
        /// <returns><see langword="true"/> if found. <see langword="false"/> if not.</returns>
        public bool TryGetData(Guid id, out IMemoryEntity data, bool prune = false)
        {
            if (_data.TryGetValue(id, out data!))
            {
                if (prune)
                    _data.Remove(id);
                return true;
            }

            else return false;
        }

        /// <summary>
        ///     Gets an instance of <see cref="IMemoryEntity"/> from the memory cache.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> to get a value for.</param>
        /// <param name="prune">Wether or not to prune the entity from memory once retrieved. Can be safely used to finalize the entity.</param>
        /// <returns>An instance of <see cref="IMemoryEntity"/>.</returns>
        /// <exception cref="KeyNotFoundException">The provided <see cref="Guid"/> was not found in the memory cache.</exception>
        public IMemoryEntity GetData(Guid id, bool prune = false)
        {
            if (_data.TryGetValue(id, out var data))
            {
                if (prune)
                    _data.Remove(id);
                return data;
            }

            else
                throw new KeyNotFoundException("No key was found for provided Guid");
        }

        /// <summary>
        ///     Prunes the instance(s) linked to provided <see cref="Guid"/>. Clears the entire cache if no <see cref="Guid"/> has been provided.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> to prune the values of.</param>
        /// <returns><see langword="true"/> if instances were succesfully removed. <see langword="false"/> if not.</returns>
        public bool Prune(Guid? id = null)
        {
            if (id is not null)
            {
                if (_data.ContainsKey(id.Value))
                {
                    _data.Remove(id.Value);

                    return true;
                }
                return false;
            }
            _data.Clear();
            return true;
        }
    }
}
