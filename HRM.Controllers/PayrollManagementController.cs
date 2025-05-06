using System;
using System.IO;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayroll;
using HRM.Models.Payroll;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PayrollManagementController(
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
    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> GetPayrollData(string month, short? periodType)
    {
        var criteria = new GetEmployeePayrollCriteria
        {
            CompanyId = CurrentCompanyId,
            Month = !string.IsNullOrEmpty(month) ? DateTime.Parse(month) : null,
            PeriodType = periodType
        };
        var data = await payrollService.GetEmployeePayrollAsync(criteria);
        return Json(data);
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> GetUploadHistory()
    {
        var history = await payrollService.GetUploadHistoryAsync(CurrentCompanyId);
        return Json(history);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> UploadPayroll()
    {
        try
        {
            var file = Request.Form.Files[0];
            var month = Request.Form["month"];
            var periodType = Request.Form["periodType"];
            if (file == null)
            {
                return BadRequest("Please select a file to upload");
            }
            if (string.IsNullOrEmpty(month))
            {
                return BadRequest("Please select a month to upload");
            }
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
            await minIOService.UploadFileAsync(file, Bucket.Payroll, path);

            return new JsonResult(res);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> ConfirmUpload()
    {
        var month = Request.Form["month"];
        var periodType = Request.Form["periodType"];
        var criteria = new ConfirmUploadPayrollCriteria
        {
            CompanyId = CurrentCompanyId,
            Month = new DateTime(DateTime.Parse(month).Year, DateTime.Parse(month).Month, 1),
            PeriodType = short.Parse(periodType)
        };
        var res = await payrollService.ConfirmUploadPayrollAsync(criteria);
        return Json(res);
    }

    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> DownloadFile(string savedFileName, string fileName)
    {
        if (string.IsNullOrEmpty(savedFileName))
        {
            return BadRequest("File name is required.");
        }

        var path = $"Payroll/{savedFileName}";
        var stream = await minIOService.GetFileContentAsync(Bucket.Payroll, path);
        return File(stream, "application/octet-stream", fileName);
    }

}