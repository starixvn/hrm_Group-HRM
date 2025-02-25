namespace HRM.AutoJobs.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class HomeControler : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("I'm fine!");
        }
    }
}