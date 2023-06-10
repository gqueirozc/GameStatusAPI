using MongoDB.Bson;

namespace GameStatusAPI.Interfaces
{
    public interface IUserStatusRepository
    {
        /// <summary>
        /// Gets data information from the repository on mongo db
        /// </summary>
        /// <param name="playerName">Player name</param>
        /// <param name="collectionName">Collection name</param>
        /// <returns>Specific player information</returns>
        List<BsonDocument> GetPlayerDataByName(string playerName, string collectionName);
    }
}
