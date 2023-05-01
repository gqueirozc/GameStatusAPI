using MongoDB.Bson;

namespace GameStatusAPI.Interfaces
{
    public interface IBaseRepository
    {
        /// <summary>
        /// get data from database collection
        /// </summary>
        /// <param name="collection">name collection</param>
        /// <returns>collection result list</returns>
        List<BsonDocument> Get(string collection);

        /// <summary>
        /// delete data from database collection
        /// </summary>
        /// <param name="collection">name collection</param>
        void Delete(string collection);

        /// <summary>
        /// Deletes a single document from the specified collection in the MongoDB database that matches the given filter fields.
        /// </summary>
        /// <param name="collectionName">The name of the collection in the database to delete the document from.</param>
        /// <param name="filterFields">A dictionary of filter fields and their corresponding values to use when searching for the document to delete.</param>
        /// <remarks>The filter fields are combined using the "AND" operator to form the filter used to search for the document to delete.</remarks>
        /// <exception cref="ArgumentNullException">Thrown if the collectionName or filterFields parameter is null.</exception>
        public void DeleteEntry(string collectionName, Dictionary<string, object> filterFields);

        /// <summary>
        /// Inserts a specified BsonDocument object into the specified MongoDB collection specified by the collectionName parameter.
        /// </summary>
        /// <param name="collectionName">The name of the collection where the data will be stored.</param>
        /// <param name="pipelineRunId">The data to be inserted represented as a BsonDocument object.</param>
        public void Insert(string collectionName, BsonDocument pipelineRunId);
    }
}
