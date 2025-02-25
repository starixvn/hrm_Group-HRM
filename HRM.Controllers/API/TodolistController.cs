using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers.API
{
    using HRM.Core.JWT;
    using HRM.Services;

    [Authorize]
    [JwtAuthentication]
    public class TodolistController : ControllerBase
    {
        private readonly IAPITodolistRequestService requestService;

        public TodolistController(IAPITodolistRequestService requestService)
        {
            this.requestService = requestService;
        }

        [HttpGet]
        public IActionResult GetTodolist(int userId)
        {
            var items = requestService.GetPendingRequestByUserId(userId);
            return Ok(items);
        }
    }
}