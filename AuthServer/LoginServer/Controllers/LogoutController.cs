using LoginServer.DB;
using LoginServer.ErrorCodeEnum;
using LoginServer.Packet;
using Microsoft.AspNetCore.Mvc;
using LoginServer.Redis;

namespace LoginServer.Controllers
{
    [Route("[controller]")]
    public class LogoutController : ControllerBase
    {
        IDBRepository _mySqlRepository;
        private readonly Redis.Redis _redis;

        public LogoutController(IDBRepository mySqlRepository, Redis.Redis redis)
        {
            _mySqlRepository = mySqlRepository;
            _redis = redis;
        }

        [HttpPost]
        public async Task<ErrorCode> PostAsync([FromBody] LogoutRequest logoutRequest)
        {
            LogoutResponse logoutResponse = new();

            // var result = await _mySqlRepository.CheckLoging(logoutRequest.UserID!);
            var result = _redis.GetValue(logoutRequest.UserID!);

            if (result == null)
            {
                logoutResponse.ErrorCode = ErrorCode.Loging;
                return logoutResponse.ErrorCode;
            }

            var removeResult = await _redis.RemoveAccount(logoutRequest.UserID!);

            if(removeResult == true)
            {
                logoutResponse.ErrorCode = ErrorCode.LogoutSuccess;
                await _mySqlRepository.DeleteToken(logoutRequest.UserID!);
            }
            else
            {
                logoutResponse.ErrorCode = ErrorCode.Fail;
            }            
            
            return logoutResponse.ErrorCode;           
        }
    }
}