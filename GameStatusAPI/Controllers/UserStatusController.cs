using System.Net;
using GameStatusAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStatusAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserStatusController
    {
        private readonly IUserStatusService _userStatusService;

        public UserStatusController(IUserStatusService gitApiService)
        {
            _userStatusService = gitApiService;
        }

        [Route("InsertUser/{playerName}")]
        [HttpPost]
        public async Task<HttpStatusCode> InsertUser(string playerName)
        { 
            try
            {
                const string collectionName = "UserStatus";
                var jsonResult = await _userStatusService.GetUserInfo(playerName);
                _userStatusService.AddUserData(jsonResult, collectionName);
                return HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                throw new Exception(HttpStatusCode.BadRequest + e.Message);
            }
        }

        [Route("GetPlayerData/{playerName}")]
        [HttpGet]
        public JsonResult GetPlayerData(string playerName)
        {
            try
            {
                const string collectionName = "UserStatus";
                var result = _userStatusService.GetPlayerDataByName(playerName, collectionName);
                return new JsonResult(result);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        [Route("GetAllPlayerData")]
        [HttpGet]
        public JsonResult GetAllPlayerData()
        {
            try
            {
                const string collectionName = "UserStatus";
                var result = _userStatusService.GetAllPlayerData(collectionName);

                return new JsonResult(result);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
