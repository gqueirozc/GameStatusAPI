using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStatusAPI.Helpers
{
    public interface IMongoFinder
    {
        List<BsonDocument> Find(IMongoCollection<BsonDocument> inputData,
            FilterDefinition<BsonDocument> filter);
    }
}
