using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IShiftService = HRM.Services.Interfaces.FnB.IShiftService;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class ShiftController(IShiftService shiftService,
    ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetShifts([FromQuery] SyncFnBRequestModel request)
    {
        var company = await companyService.GetCompanySiteByDomainAsync();
        var criteria = new GetAllShiftCriteria
        {
            CompanyId = company.CompanyId
        };

        var result = await shiftService.GetAllShift(criteria);

        return new JsonResult(result);
    }
}