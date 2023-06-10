using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStatusAPI.Helpers
{
    /// <summary>
    /// Given a Mongo Collection and a filter, returns a list of BsonDocuments
    /// </summary>
    /// Excluded from code coverage because it is working as a wrapper for the extension method .Find
    public interface IMongoFinder
    {
        List<BsonDocument> Find(IMongoCollection<BsonDocument> inputData,
            FilterDefinition<BsonDocument> filter);
    }
}
