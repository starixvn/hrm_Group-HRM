using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers.API
{
    using HRM.Core;
    using HRM.Core.JWT;
    using HRM.Models.API;

    public class TokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetToken(GetTokenParam param)
        {
            if (IsValid(param))
            {
                var token = JwtManager.GenerateToken(param.Username);

                return Ok(token);
            }

            return Unauthorized();
        }

        private static bool IsValid(GetTokenParam param)
        {
            return param.Username == ConfigSettings.API_Auth_Username && param.Password == ConfigSettings.API_Auth_Password;
        }
    }
}