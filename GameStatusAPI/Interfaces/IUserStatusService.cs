using Newtonsoft.Json.Linq;

namespace GameStatusAPI.Interfaces
{
    public interface IUserStatusService
    {
        List<JObject> GetPlayerDataByName(string playerName, string collectionName);

        Task<string> GetUserInfo(string userName);

        void AddUserData(string jsonResult, string collectionName);

    }
}
