using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class ShiftController : ControllerBase
{
    // GET
    public async Task<IActionResult> Index()
    {
        return null;
    }
}