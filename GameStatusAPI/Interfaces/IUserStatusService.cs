using Newtonsoft.Json.Linq;

namespace GameStatusAPI.Interfaces
{
    public interface IUserStatusService
    {
        /// <summary>
        /// Gets data information from the repository and filters the informataion
        /// </summary>
        /// <param name="playerName">Player name</param>
        /// <param name="collectionName">Collection name</param>
        /// <returns>Specific player information</returns>
        List<JObject> GetPlayerDataByName(string playerName, string collectionName);

        /// <summary>
        /// Gets data information from the user via official API and converts it into a string.
        /// </summary>
        /// <param name="userName">Player name to be used in the url</param>
        /// <returns>Specific player information</returns>
        Task<string> GetUserInfo(string userName);

        /// <summary>
        /// Adds user information in the mongo db collection.
        /// </summary>
        /// <param name="jsonResult">Information value as json that will be added to the collection</param>
        /// <param name="collectionName">Collection name from mongo db</param>
        void AddUserData(string jsonResult, string collectionName);

        /// <summary>
        /// Gets players information from mongo
        /// </summary>
        /// <param name="collectionName">Collection name from mongo db</param>
        /// <returns>All players information in a JObject list</returns>
        List<JObject> GetAllPlayerData(string collectionName);

        /// <summary>
        /// Deletes user information in the mongo db collection.
        /// </summary>
        /// <param name="jsonResult">Information value as json that will be added to the collection</param>
        /// <param name="collectionName">Collection name from mongo db</param>
        void DeleteUserData(string jsonResult, string collectionName);

    }
}
