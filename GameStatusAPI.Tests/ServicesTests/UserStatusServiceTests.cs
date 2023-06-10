using FluentAssertions;
using GameStatusAPI.Interfaces;
using GameStatusAPI.Services;
using MongoDB.Bson;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace GameStatusAPI.Tests.ServicesTests
{
    [TestFixture]
    public class UserStatusServiceTests
    {
        private Mock<IUserStatusRepository> _userStatusRepositoryMock;
        private Mock<IBaseRepository> _baseRepositoryMock;
        private Mock<IRegularHttpClient> _httpClientMock;
        private UserStatusService _userStatusService;

        [SetUp]
        public void Setup()
        {
            _userStatusRepositoryMock = new Mock<IUserStatusRepository>();
            _baseRepositoryMock = new Mock<IBaseRepository>();
            _httpClientMock = new Mock<IRegularHttpClient>();
            _userStatusService = new UserStatusService(
                _baseRepositoryMock.Object,
                _httpClientMock.Object,
                _userStatusRepositoryMock.Object
            );
        }

        [Test]
        public void AddUserData_WhenValidJsonResult_ShouldCallBaseRepositoryInsert()
        {
            // Arrange
            string jsonResult = "{ \"name\": \"Guilherme\", \"score\": 100 }";
            string collectionName = "UserStatus";

            _baseRepositoryMock.Setup(_ => _.Get(collectionName)).Returns(new List<BsonDocument>());
            _baseRepositoryMock.Setup(_ => _.Insert(collectionName, It.IsAny<BsonDocument>()));

            // Act
            _userStatusService.AddUserData(jsonResult, collectionName);

            // Assert
            _baseRepositoryMock.Verify(_ => _.Insert(collectionName, It.IsAny<BsonDocument>()), Times.Once);
        }

        [Test]
        public void DeleteUserData_WhenValidJsonResult_ShouldCallBaseRepositoryDeleteEntry()
        {
            // Arrange
            string jsonResult = "{ \"name\": \"Guilherme\", \"score\": 100 }";
            string collectionName = "UserStatus";

            _baseRepositoryMock.Setup(_ => _.Get(collectionName)).Returns(
                new List<BsonDocument> {
                      new BsonDocument { { "name", "Guilherme" }, { "score", 100 } },
                    }
                 );;

            _baseRepositoryMock.Setup(_ => _.DeleteEntry(collectionName, It.IsAny<Dictionary<string, object>>()));

            // Act
            _userStatusService.DeleteUserData(jsonResult, collectionName);

            // Assert
            _baseRepositoryMock.Verify(_ => _.DeleteEntry(collectionName, It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Test]
        public async Task GetUserInfo_WhenValidUserName_ShouldReturnFilteredUserData()
        {
            // Arrange
            var userName = "Guilherme";
            var expectedUserData = new
            {
                name = "Guilherme",
                rank = 1,
                loggedIn = true,
                combatlevel = 100,
                totalskill = 3000,
                questsstarted = 10,
                questscomplete = 5,
                questsnotstarted = 3,
                totalxp = 50000000,
                skillvalues = new[]
               {
                    new { id = 0, skillName = "Attack", level = 99, xp = 1303443, rank = 1 },
                    new { id = 1, skillName = "Defence", level = 99, xp = 1303443, rank = 1 }
                },
                activities = new[]
                {
                    new { date = "2023-06-09", details = "Activity 2", text = "Activity 2 completed." },
                    new { date = "2023-06-10", details = "Activity 1", text = "Activity 1 completed." }
                }
            };

            var expectedUserDataJson = JsonConvert.SerializeObject(expectedUserData);
            var expectedJson = JToken.Parse(expectedUserDataJson);

            _httpClientMock.Setup(_ => _.GetStringAsync(It.IsAny<string>())).ReturnsAsync(expectedUserDataJson);

            // Act
            var result = await _userStatusService.GetUserInfo(userName);
            var resultJson = JToken.Parse(result);

            // Assert
            resultJson.Should().BeEquivalentTo(expectedJson, options => options
             .ExcludingMissingMembers()
             .IncludingNestedObjects()
             .WithStrictOrdering());
        }

        [Test]
        public void GetPlayerDataByName_WhenValidNameAndCollection_ShouldReturnDataList()
        {
            // Arrange
            string playerName = "Guilherme";
            string collectionName = "UserStatus";
            var playerDataList = new List<BsonDocument>()
            {
                new BsonDocument { { "name", "Guilherme" }, { "score", 100 } },
            };

            _userStatusRepositoryMock.Setup(mock => mock.GetPlayerDataByName(playerName, collectionName)).Returns(playerDataList);

            // Act
            var result = _userStatusService.GetPlayerDataByName(playerName, collectionName);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);

            var expectedObject = new JObject(
                new JProperty("name", "Guilherme"),
                new JProperty("score", new JObject(new JProperty("$numberInt", "100")))
            );

            result[0].Should().BeEquivalentTo(expectedObject);
        }

        [Test]
        public void GetAllPlayerData_WhenValidCollectionName_ShouldReturnsAllPlayerDataList()
        {
            // Arrange
            string collectionName = "UserStatus";
            var playerDataList = new List<BsonDocument>()
            {
                new BsonDocument { { "name", "Guilherme" }, { "score", 100 } },
                new BsonDocument { { "name", "Gustavo" }, { "score", 200 } }
            };

            _baseRepositoryMock.Setup(mock => mock.Get(collectionName)).Returns(playerDataList);

            // Act
            var result = _userStatusService.GetAllPlayerData(collectionName);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var expectedObject1 = new JObject(
                new JProperty("name", "Guilherme"),
                new JProperty("score", new JObject(new JProperty("$numberInt", "100")))
            );

            var expectedObject2 = new JObject(
                new JProperty("name", "Gustavo"),
                new JProperty("score", new JObject(new JProperty("$numberInt", "200")))
            );

            result[0].Should().BeEquivalentTo(expectedObject1);
            result[1].Should().BeEquivalentTo(expectedObject2);
        }
    }
}
