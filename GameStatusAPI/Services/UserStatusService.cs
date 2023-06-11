using GameStatusAPI.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;

namespace GameStatusAPI.Services
{
    public class UserStatusService : IUserStatusService
    {
        private readonly IUserStatusRepository _userStatusRepository;
        private readonly IBaseRepository _baseRepository;
        private readonly IRegularHttpClient _httpClient;

        public UserStatusService(IBaseRepository baseRepository, IRegularHttpClient httpClient,
            IUserStatusRepository userStatusRepository)
        {
            _baseRepository = baseRepository;
            _httpClient = httpClient;
            _userStatusRepository = userStatusRepository;
        }

        public void AddUserData(string jsonResult, string collectionName)
        {
            var document = BsonDocument.Parse(jsonResult);
            var collection = _baseRepository.Get(collectionName);

            if (collection.Any(item => item["name"] == document["name"]))
            {
                var filterFields = new Dictionary<string, object>
                {
                    {"name", document["name"].AsString},
                };
                _baseRepository.DeleteEntry(collectionName, filterFields);
            }
            _baseRepository.Insert(collectionName, document);
        }

        public void DeleteUserData(string jsonResult, string collectionName)
        {
            var document = BsonDocument.Parse(jsonResult);
            var collection = _baseRepository.Get(collectionName);

            if (collection.Any(item => item["name"] == document["name"]))
            {
                var filterFields = new Dictionary<string, object>
                {
                    {"name", document["name"].AsString},
                };
                _baseRepository.DeleteEntry(collectionName, filterFields);
            }
        }

        public async Task<string> GetUserInfo(string userName)
        {
            var playerInfoUrl = $"https://apps.runescape.com/runemetrics/profile/profile?user={userName}&activities=20";
            var response = await _httpClient.GetStringAsync(playerInfoUrl);
            var playerInfo = JObject.Parse(response);

            if (playerInfo["error"]?.ToString() == "PROFILE_PRIVATE")
            {
                throw new Exception("The profile is private.");
            }
            else if (playerInfo["error"]?.ToString() == "NO_PROFILE")
            {
                throw new Exception("The profile does not exist.");
            }

            return GetFilteredUserData(playerInfo).ToString();
        }

        private static JArray GetUserSkills(JToken userInfo)
        {
            var skillList = new Dictionary<int, string>()
            {
                {0, "Attack"},
                {1, "Defence"},
                {2, "Strength"},
                {3, "Constitution"},
                {4, "Ranged"},
                {5, "Prayer"},
                {6, "Magic"},
                {7, "Cooking"},
                {8, "Woodcutting"},
                {9, "Fletching"},
                {10, "Fishing"},
                {11, "Firemaking"},
                {12, "Crafting"},
                {13, "Smithing"},
                {14, "Mining"},
                {15, "Herblore"},
                {16, "Agility"},
                {17, "Thieving"},
                {18, "Slayer"},
                {19, "Farming"},
                {20, "Runecrafting"},
                {21, "Hunter"},
                {22, "Construction"},
                {23, "Summoning"},
                {24, "Dungeoneering"},
                {25, "Divination"},
                {26, "Invention"},
                {27, "Archaeology"}
            };

            var levelsArray = new JArray();

            try
            {
                foreach (var skillValue in userInfo.SelectToken("skillvalues"))
                {
                    var id = skillValue.SelectToken("id")!.Value<int>();
                    skillList.TryGetValue(id, out var value);
                    var levelObject = new JObject
                    {
                        ["id"] = skillValue.SelectToken("id"),
                        ["skillName"] = value,
                        ["level"] = skillValue.SelectToken("level"),
                        ["xp"] = skillValue.SelectToken("xp")!.Value<int>() / 10,
                        ["rank"] = skillValue.SelectToken("rank")
                    };
                    levelsArray.Add(levelObject);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error adding skill values: " + e.Message);
            }
       
            return levelsArray;
        }

        private static JArray GetUserActivities(JToken userInfo)
        {
            var activitiesArray = new JArray();
            foreach (var activity in userInfo.SelectToken("activities"))
            {
                var activitiesObject = new JObject
                {
                    ["date"] = activity.SelectToken("date"),
                    ["details"] = activity.SelectToken("details"),
                    ["text"] = activity.SelectToken("text"),
                };
                activitiesArray.Add(activitiesObject);
            }
            return activitiesArray;
        }

        private static JObject GetFilteredUserData(JToken userInfo)
        {
            var levelsArray = GetUserSkills(userInfo);
            var activitiesArray = GetUserActivities(userInfo);
            var filteredJObject = new JObject
            {
                ["lowercaseName"] = userInfo.SelectToken("name")?.ToString().ToLower(),
                ["name"] = userInfo.SelectToken("name"),
                ["rank"] = userInfo.SelectToken("rank"),
                ["loggedIn"] = userInfo.SelectToken("loggedIn"),
                ["combatlevel"] = userInfo.SelectToken("combatlevel"),
                ["totalskill"] = userInfo.SelectToken("totalskill"),
                ["questsstarted"] = userInfo.SelectToken("questsstarted"),
                ["questscomplete"] = userInfo.SelectToken("questscomplete"),
                ["questsnotstarted"] = userInfo.SelectToken("questsnotstarted"),
                ["totalxp"] = userInfo.SelectToken("totalxp"),
                ["skill_values"] = new JArray(levelsArray.OrderBy(j => j["id"])),
                ["activities"] = new JArray(activitiesArray.OrderBy(j => j["date"])),
            };
            return filteredJObject;
        }
        
        public List<JObject> GetPlayerDataByName(string playerName, string collectionName)
        {
            var list = _userStatusRepository.GetPlayerDataByName(playerName, collectionName);
            var objectList = new List<JObject>();
            foreach (var doc in list)
            {
                var jsonWriterSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
                objectList.Add(JObject.Parse(doc.ToJson(jsonWriterSetting)));
            }
            return objectList;
        }

        public List<JObject> GetAllPlayerData(string collectionName)
        {
            var list = _baseRepository.Get(collectionName);
            var objectList = new List<JObject>();
            foreach (var doc in list)
            {
                var jsonWriterSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
                objectList.Add(JObject.Parse(doc.ToJson(jsonWriterSetting)));
            }
            return objectList;
        }
    }
}
