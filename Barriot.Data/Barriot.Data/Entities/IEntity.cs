using MongoDB.Bson;

namespace Barriot
{
    /// <summary>
    ///     A database entity.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        ///     The original ID of this entity.
        /// </summary>
        ObjectId ObjectId { get; set; }
    }
}
