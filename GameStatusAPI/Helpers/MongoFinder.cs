using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStatusAPI.Helpers
{
    [ExcludeFromCodeCoverage]
    public class MongoFinder : IMongoFinder
    {
        public List<BsonDocument> Find(IMongoCollection<BsonDocument> inputData, 
            FilterDefinition<BsonDocument> filter)
        {
            return inputData.Find(filter).ToList();
        }
    }
}
