namespace Fanex.NexHR.Controllers.API
{
    using System.Web.Http;
    using Fanex.NexHR.Core.JWT;
    using Fanex.NexHR.Services;

    [Authorize]
    [JwtAuthentication]
    public class TodolistController : ApiController
    {
        private readonly IAPITodolistRequestService requestService;

        public TodolistController(IAPITodolistRequestService requestService)
        {
            this.requestService = requestService;
        }

        [HttpGet]
        public IHttpActionResult GetTodolist(int userId)
        {
            var items = requestService.GetPendingRequestByUserId(userId);
            return Ok(items);
        }
    }
}