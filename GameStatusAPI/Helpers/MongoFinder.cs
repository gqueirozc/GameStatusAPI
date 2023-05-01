using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStatusAPI.Helpers
{
    /// <summary>
    /// Given a Mongo Collection and a filter, returns a list of BsonDocuments
    /// </summary>
    /// Excluded from code coverage because it is working as a wrapper for the extension method .Find
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
