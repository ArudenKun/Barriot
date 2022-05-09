namespace Barriot
{
    /// <summary>
    ///     An entity that can be edited and saved on disposal.
    /// </summary>
    public interface IMutableEntity : IDisposable, IEntity
    {
        /// <summary>
        ///     Updates this entity.
        /// </summary>
        /// <returns><see cref="true"/> if successful. <see cref="false"/> if failed.</returns>
        public Task<bool> UpdateAsync();

        /// <summary>
        ///     Deletes this entity.
        /// </summary>
        /// <returns><see cref="true"/> if successful. <see cref="false"/> if failed.</returns>
        public Task<bool> DeleteAsync();
    }
}