using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class ShiftController(IShiftService shiftService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetShifts([FromQuery] DateTime lastModifiedDate)
    {
        var criteria = new GetShiftByLastModifiedDateCriteria()
        {
            LastModifiedDate = lastModifiedDate
        };

        var result = await shiftService.GetShiftByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}