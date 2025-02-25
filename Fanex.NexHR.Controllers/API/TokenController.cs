namespace Fanex.NexHR.Controllers.API
{
    using System.Web.Http;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.JWT;
    using Fanex.NexHR.Models.API;

    public class TokenController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetToken(GetTokenParam param)
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