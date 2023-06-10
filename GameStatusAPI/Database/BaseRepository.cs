using GameStatusAPI.Helpers;
using GameStatusAPI.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStatusAPI.Database
{
    public class BaseRepository : IBaseRepository
    {
        private readonly IMongoClient _client;
        private readonly IMongoFinder _mongoFinder;
        private const string DatabaseName = "gamestatusapi";

        public BaseRepository(IMongoClient client, IMongoFinder mongoFinder)
        {
            _client = client;
            _mongoFinder = mongoFinder;
        }

        public List<BsonDocument> Get(string collection)
        {
            var list = _client.GetDatabase(DatabaseName).GetCollection<BsonDocument>(collection);
            return _mongoFinder.Find(list, new BsonDocument());
        }

        public void DeleteEntry(string collectionName, Dictionary<string, object> filterFields)
        {
            var collection = _client.GetDatabase(DatabaseName).GetCollection<BsonDocument>(collectionName);
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.And(filterFields.Select(f => filterBuilder.Eq(f.Key, BsonValue.Create(f.Value))));
            collection.DeleteOne(filter);
        }

        public void Insert(string collectionName, BsonDocument document)
        {
            var collection = _client.GetDatabase(DatabaseName).GetCollection<BsonDocument>(collectionName);
            collection.InsertOne(document);
        }
    }
}
