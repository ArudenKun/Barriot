namespace Barriot
{
    /// <summary>
    ///     Represents the state of an entity.
    /// </summary>
    public enum EntityState : int
    {
        /// <summary>
        ///     Base state, when the entity has not yet fully deserialized.
        /// </summary>
        Deserializing = 0,

        /// <summary>
        ///     Default state, when the entity is ready to modify.
        /// </summary>
        Initialized = 1,

        /// <summary>
        ///     Deleted state, when the entity cannot be modified or called anymore.
        /// </summary>
        Deleted = 2,
    }
}
