using FluentAssertions;
using GameStatusAPI.Controllers;
using GameStatusAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Net;

namespace GameStatusAPI.Tests.ControllersTests
{
    [TestFixture]
    public class UserStatusControllerTests
    {
        private UserStatusController _userController;
        private Mock<IUserStatusService> _userStatusServiceMock;

        [SetUp]
        public void Setup()
        {
            _userStatusServiceMock = new Mock<IUserStatusService>();
            _userController = new UserStatusController(_userStatusServiceMock.Object);
        }

        [Test]
        public async Task InsertUser_WhenValidPlayerName_ShouldAddAndReturnsStatusCodeOk()
        {
            // Arrange
            const string playerName = "Guilherme";
            var expectedStatusCode = HttpStatusCode.OK;

            _userStatusServiceMock
                .Setup(_ => _.GetUserInfo(playerName))
                .ReturnsAsync("AnyJson");

            // Act
            var result = await _userController.InsertUser(playerName);

            // Assert
            result.Should().Be(expectedStatusCode);
            _userStatusServiceMock.Verify(_ => _.GetUserInfo(playerName), Times.Once);
            _userStatusServiceMock.Verify(_ => _.AddUserData(It.IsAny<string>(), "UserStatus"), Times.Once);
        }

        [Test]
        public void InsertUser_WhenServiceThrowsException_ShouldReturnsBadRequest()
        {
            // Arrange
            const string playerName = "Zzzz";
            var expectedExceptionMessage = "Private Profile";
            var expectedStatusCode = HttpStatusCode.BadRequest;

            _userStatusServiceMock.Setup(_ => _.GetUserInfo(playerName)).Throws(new Exception(expectedExceptionMessage));

            // Act
            Func<Task<HttpStatusCode>> action = () => _userController.InsertUser(playerName);

            // Assert
            action.Should().ThrowAsync<Exception>().WithMessage($"{expectedStatusCode} - {expectedExceptionMessage}");
            _userStatusServiceMock.Verify(_ => _.GetUserInfo(playerName), Times.Once);
            _userStatusServiceMock.Verify(_ => _.AddUserData(It.IsAny<string>(), "UserStatus"), Times.Never);
        }


        [Test]
        public async Task DeleteUser_WhenValidPlayerName_ShouldReturnStatusCodeOk()
        {
            // Arrange
            const string playerName = "Guilherme";
            var expectedStatusCode = HttpStatusCode.OK;

            _userStatusServiceMock
                .Setup(_ => _.GetUserInfo(playerName))
                .ReturnsAsync("AnyJson");

            // Act
            var result = await _userController.DeleteUser(playerName);

            // Assert
            result.Should().Be(expectedStatusCode);
            _userStatusServiceMock.Verify(_ => _.GetUserInfo(playerName), Times.Once);
            _userStatusServiceMock.Verify(_ => _.DeleteUserData(It.IsAny<string>(), "UserStatus"), Times.Once);
        }

        [Test]
        public void DeleteUser_WhenServiceThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            const string playerName = "ZZZz";
            var expectedExceptionMessage = "Entry does not exist";
            var expectedStatusCode = HttpStatusCode.BadRequest;

            _userStatusServiceMock.Setup(_ => _.GetUserInfo(playerName)).Throws(new Exception(expectedExceptionMessage));

            // Act
            Func<Task<HttpStatusCode>> action = () => _userController.DeleteUser(playerName);

            // Assert
            action.Should().ThrowAsync<Exception>().WithMessage($"{expectedStatusCode} - {expectedExceptionMessage}");
            _userStatusServiceMock.Verify(_ => _.GetUserInfo(playerName), Times.Once);
            _userStatusServiceMock.Verify(_ => _.DeleteUserData(It.IsAny<string>(), "UserStatus"), Times.Never);
        }

        [Test]
        public async Task UpdateUserInfo_WhenValidPlayerName_ShouldReturnHttpStatusCodeOK()
        {
            // Arrange
            string playerName = "Guilherme";
            string collectionName = "UserStatus";
            var jsonResult = "{ \"name\": \"Guilherme\", \"score\": 100 }";
            var expectedStatusCode = HttpStatusCode.OK;

            _userStatusServiceMock.Setup(_ => _.GetUserInfo(playerName)).ReturnsAsync(jsonResult);
            _userStatusServiceMock.Setup(_ => _.DeleteUserData(jsonResult, collectionName));
            _userStatusServiceMock.Setup(_ => _.AddUserData(jsonResult, collectionName));

            // Act
            var result = await _userController.UpdateUserInfo(playerName);

            // Assert
            result.Should().Be(expectedStatusCode);
            _userStatusServiceMock.Verify(_ => _.GetUserInfo(playerName), Times.Once);
            _userStatusServiceMock.Verify(_ => _.DeleteUserData(jsonResult, collectionName), Times.Once);
            _userStatusServiceMock.Verify(_ => _.AddUserData(jsonResult, collectionName), Times.Once);
        }

        [Test]
        public void GetPlayerData_WhenValidPlayerName_ShouldReturnsJsonResult()
        {
            // Arrange
            const string playerName = "guilherme";
            var expectedResult = new List<JObject>
            {
                new JObject { { "Name", "Guilherme" }, { "Level", 10 } },
            };

            _userStatusServiceMock.Setup(_ => _.GetPlayerDataByName(playerName, "UserStatus")).Returns(expectedResult);

            // Act
            var result = _userController.GetPlayerData(playerName);

            // Assert
            result.Should().BeOfType<JsonResult>();
            result.Value.Should().BeEquivalentTo(expectedResult);
            _userStatusServiceMock.Verify(_ => _.GetPlayerDataByName(playerName, "UserStatus"), Times.Once);
        }

        [Test]
        public void GetPlayerData_WhenServiceThrowsException_ShouldThrowsExceptionWithMessage()
        {
            // Arrange
            const string playerName = "guilherme";
            var expectedExceptionMessage = "Some error message";

            _userStatusServiceMock
                .Setup(_ => _.GetPlayerDataByName(playerName, "UserStatus"))
                .Throws(new Exception(expectedExceptionMessage));

            // Act
            Action action = () => _userController.GetPlayerData(playerName);

            // Assert
            action.Should().Throw<Exception>().WithMessage(expectedExceptionMessage);
            _userStatusServiceMock.Verify(_ => _.GetPlayerDataByName(playerName, "UserStatus"), Times.Once);
        }

        [Test]
        public void GetAllPlayerData_WhenThereIsCollection_ShouldReturnJsonResult()
        {
            // Arrange
            const string collectionName = "UserStatus";
            var expectedResult = new List<JObject>
            {
                new JObject { { "Name", "Guilherme" }, { "Level", 10 } },
                new JObject { { "Name", "Gustavo" }, { "Level", 5 } }
            };

            _userStatusServiceMock.Setup(_ => _.GetAllPlayerData(collectionName)).Returns(expectedResult);

            // Act
            var result = _userController.GetAllPlayerData();

            // Assert
            result.Should().BeOfType<JsonResult>();
            result.Value.Should().BeEquivalentTo(expectedResult);
            _userStatusServiceMock.Verify(_ => _.GetAllPlayerData(collectionName), Times.Once);
        }

        [Test]
        public void GetAllPlayerData_WhenServiceThrowsException_ShouldThrowExceptionWithMessage()
        {
            // Arrange
            const string collectionName = "UserStatus";
            var expectedExceptionMessage = "Failed to get player data, collection must be empty";

            _userStatusServiceMock.Setup(_ => _.GetAllPlayerData(collectionName)).Throws(new Exception(expectedExceptionMessage));

            // Act
            Action action = () => _userController.GetAllPlayerData();

            // Assert
            action.Should().Throw<Exception>().WithMessage(expectedExceptionMessage);
            _userStatusServiceMock.Verify(_ => _.GetAllPlayerData(collectionName), Times.Once);
        }
    }
}
