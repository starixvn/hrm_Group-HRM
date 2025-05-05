using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayroll;
using HRM.Models.Payroll;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class SalaryManagementController(
    IUserControlSession session,
    IMinIOService minIOService,
    IUploadPayrollService uploadService,
    IPayrollService payrollService) : BaseController(session)
{

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetSalaryData()
    {
        var criteria = new GetEmployeePayrollCriteria
        {
            CompanyId = CurrentCompanyId,
            Month = new DateTime(DateTimeOffset.Now.DateTime.ToClientTimeZone().Year, DateTimeOffset.Now.DateTime.ToClientTimeZone().Month, 1)
        };
        var data = await payrollService.GetEmployeePayrollAsync(criteria);
        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetUploadHistory()
    {
        var history = await payrollService.GetUploadHistoryAsync(CurrentCompanyId);
        return Json(history);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SalaryManagement)]
    public async Task<IActionResult> UploadSalary()
    {
        try
        {
            var file = Request.Form.Files[0];
            var month = Request.Form["month"];
            var periodType = Request.Form["periodType"];
            
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var timestamp = DateTimeOffset.Now.ToString("yyyyMMddHHmmss");
            
            var savedFileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";
            var path = $"Payroll/{savedFileName}";
            var model = new UploadPayrollModel
            {
                File = file,
                SavedFileName = savedFileName,
                PeriodType = short.Parse(periodType),
                CompanyId = CurrentCompanyId,
                ActorId = session.UserId,
                Month = new DateTime(DateTime.Parse(month).Year, DateTime.Parse(month).Month, 1)
            };

            var res = await uploadService.ImportEmpPayrollAsync(model);
            if (!res.IsSuccessful)
            {
                return BadRequest(res.Message);
            }
            await minIOService.UploadFileAsync(file, Bucket.File, path);

            return new JsonResult(res);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}