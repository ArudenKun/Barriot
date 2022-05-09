namespace Barriot.API.Voting
{
    public interface IVoteClient
    {
        /// <summary>
        ///     Gets if a user has voted.
        /// </summary>
        /// <param name="user">The user that a vote should be checked for.</param>
        /// <returns><c>true</c> if succesful. <c>false</c> if not.</returns>
        Task<bool> GetVoteAsync(RestUser user);
    }
}
