using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class ShiftController(IShiftService shiftService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetShifts([FromQuery] SyncFnBRequestModel request)
    {
        var criteria = new GetAllShiftCriteria
        {
            CompanyId = request.CompanyId
        };

        var result = await shiftService.GetAllShift(criteria);

        return new JsonResult(result);
    }
}