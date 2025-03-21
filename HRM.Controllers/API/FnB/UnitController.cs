using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class UnitController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUnitByLastModifiedDate([FromQuery] DateTime LastModifiedDate)
    {
        return null;
    }
}