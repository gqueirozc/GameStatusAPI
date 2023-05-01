using MongoDB.Bson;

namespace GameStatusAPI.Interfaces
{
    public interface IUserStatusRepository
    {
        List<BsonDocument> GetPlayerDataByName(string playerName, string collectionName);
    }
}
