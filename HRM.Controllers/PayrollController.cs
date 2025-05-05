namespace HRM.Controllers;

using System;
using System.IO;
using System.Threading.Tasks;
using Core.Constants;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Models.Payroll;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class PayrollController(
    IUserControlSession session,
    IMinIOService minIOService,
    IUploadPayrollService uploadService) : BaseController(session)
{

    [HttpPost("UploadExcel")]
    [HrmPermission(Policy.Host, FunctionKey.SalaryManagement)]
    public async Task<IActionResult> UploadExcel(IFormFile file, short periodType, DateTime month)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
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
            PeriodType = periodType,
            CompanyId = CurrentCompanyId,
            ActorId = session.UserId,
            Month = month
        };
        var res = await uploadService.ImportEmpPayrollAsync(model);
        if (!res.IsSuccessful)
        {
            return BadRequest(res.Message);
        }
        await minIOService.UploadFileAsync(file, Bucket.File, path);
        return new JsonResult(res);
    }

}