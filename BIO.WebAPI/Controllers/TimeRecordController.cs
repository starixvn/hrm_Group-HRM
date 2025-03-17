using System;
using System.Threading.Tasks;
using BIO.WebAPI.Criteria;
using BIO.WebAPI.Models;
using BIO.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BIO.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeRecordController(
    IConfiguration configuration,
    IBaseService baseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(DateTime fromDate, DateTime toDate)
    {
        var company = configuration.GetValue<string>("Company");
        var criteria = new GetTimeRecordCriteria
        {
            FromDate = fromDate,
            ToDate = toDate
        };
        var records = await baseService.GetDataAsync<TimeRecord>(criteria);
        foreach (var record in records)
        {
            record.Company = company;
        }

        return Ok(records);
    }
}