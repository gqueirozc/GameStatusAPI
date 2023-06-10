using GameStatusAPI.Helpers;
using GameStatusAPI.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStatusAPI.Database
{
    public class UserStatusRepository : IUserStatusRepository
    {
        private readonly IMongoClient _client;
        private readonly IMongoFinder _mongoFinder;
        private const string DatabaseName = "gamestatusapi";

        public UserStatusRepository(IMongoClient client, IMongoFinder mongoFinder)
        {
            _mongoFinder = mongoFinder;
            _client = client;
        }

        public List<BsonDocument> GetPlayerDataByName(string playerName, string collectionName)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("name", playerName);
            var collection = _client.GetDatabase(DatabaseName).GetCollection<BsonDocument>(collectionName);
            var list = _mongoFinder.Find(collection, filter);
            return list.ToList();
        }
    }
}

