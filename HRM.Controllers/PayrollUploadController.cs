using System;
using System.Globalization;
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

public class PayrollUploadController(
    IUserControlSession session,
    IMinIOService minIOService,
    IUploadPayrollService uploadService,
    IPayrollService payrollService) : BaseController(session)
{

    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public IActionResult Index()
    {
        ViewBag.HasPermissionToUpload = session.HasPermissionOnFunction(FunctionKey.PayrollManagement, Policy.Host);
        return View();
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> GetPayrollData(string month, short? periodType)
    {
        var criteria = new GetEmployeePayrollCriteria
        {
            CompanyId = CurrentCompanyId,
            Month = !string.IsNullOrEmpty(month) ? DateTimeOffset.Parse(month, CultureInfo.InvariantCulture).DateTime : null,
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
    public async Task<IActionResult> UploadPayroll([FromForm] UploadPayrollRequest request)
    {
        if (request.File == null)
        {
            return BadRequest("Please select a file to upload");
        }
        if (string.IsNullOrEmpty(request.Month))
        {
            return BadRequest("Please select a month to upload");
        }

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(request.File.FileName);
        var extension = Path.GetExtension(request.File.FileName);
        var timestamp = DateTimeOffset.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        var savedFileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";
        var path = $"{CurrentCompanyId}/{savedFileName}";
        var model = new UploadPayrollModel
        {
            File = request.File,
            SavedFileName = savedFileName,
            PeriodType = short.Parse(request.PeriodType, CultureInfo.InvariantCulture),
            CompanyId = CurrentCompanyId,
            ActorId = session.UserId,
            Month = DateTime.Parse(request.Month, CultureInfo.InvariantCulture)
        };

        var res = await uploadService.ImportEmpPayrollAsync(model);
        if (!res.IsSuccessful)
        {
            return BadRequest(res.Message);
        }
        await minIOService.UploadFileAsync(request.File, Bucket.Payroll, path);

        return new JsonResult(res);

    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> ConfirmUpload([FromForm] ConfirmUploadRequest request)
    {
        var criteria = new ConfirmUploadPayrollCriteria
        {
            CompanyId = CurrentCompanyId,
            Month = DateTime.Parse(request.Month, CultureInfo.InvariantCulture),
            PeriodType = short.Parse(request.PeriodType, CultureInfo.InvariantCulture)
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

        var path = $"{CurrentCompanyId}/{savedFileName}";
        var stream = await minIOService.GetFileContentAsync(Bucket.Payroll, path);
        return File(stream, "application/octet-stream", fileName);
    }
}