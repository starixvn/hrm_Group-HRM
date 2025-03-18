
namespace HRM.Controllers;

using HRM.Core.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Filters;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Implement;
using Services.Interfaces;
using ViewModels.Memo;

public class MemoController(
    AppSettings appSettings,
    IWebHostEnvironment env,
    IUserControlSession userControlSession,
    IMemoService memoService,
    ICommonDataService commonDataService,
    IEmailService emailService,
    IWebHostEnvironment hostingEnvironment,
    IMinIOService minIOService,
    IConfiguration configuration) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;
    private const int UploadMemoType = 1;

    [HrmPermission(Policy.Read, FunctionKey.Memo)]
    public async Task<IActionResult> Index(int? year, int category = -1)
    {
        var model = new MemoViewModel
        {
            MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory, CurrentCompanyId),
            Category = category
        };

        model.Year = year ?? model.Year;

        model.Memos = await memoService.GetAllAsync(model.Year, model.Category, CurrentCompanyId);
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Memo, Policy.Host);
        
        return View(model);
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.Memo)]
    public async Task<IActionResult> Edit(int id)
    {
        var memo = await memoService.GetByIdAsync(id, CurrentCompanyId);
        ViewBag.MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory, CurrentCompanyId);
        ViewBag.Updated = false;
        ViewBag.UploadBasePath = $"{configuration["AWS:ExternalUrl"]}/{Bucket.File}";

        return View(memo);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Memo)]
    public async Task<IActionResult> Edit(Memo model)
    {
        ViewBag.MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory, CurrentCompanyId);
        if (model.MemoType == UploadMemoType && string.IsNullOrEmpty(model.AttachFile))
        {
            ViewBag.Updated = false;
            return View(model);
        }

        model.Contents = model.MemoType == UploadMemoType ? model.AttachFile : model.Contents;

        if (ModelState.IsValid)
        {
            model.CompanyId = CurrentCompanyId;
            var commandResult = await memoService.EditAsync(model);
            ViewBag.Updated = commandResult.IsSuccessful;

            switch (commandResult.IsSuccessful)
            {
	            case true when model.SendMail:
		            await SendMailAsync(model.Subject, commandResult);
		            break;
	            case false:
		            ModelState.AddModelError(string.Empty, commandResult.Message);
		            break;
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
        }

        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.Memo)]
    public async Task<IActionResult> Create()
    {
        var memo = new Memo
        {
            SentTo = "All Staff",
            SentFrom = "Management",
            TotalPages = 1,
            SentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            Contents = "[Contents]",
            MemoType = UploadMemoType,
            CompanyId = CurrentCompanyId
        };

        ViewBag.MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory, CurrentCompanyId);
        ViewBag.Updated = false;

        return View(memo);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Memo)]
    public async Task<IActionResult> Create(Memo model)
    {
        ViewBag.MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory, CurrentCompanyId);
        model.Contents = model.MemoType == UploadMemoType ? model.AttachFile : model.Contents;

        if (ModelState.IsValid)
        {
            model.CompanyId = CurrentCompanyId;
            var commandResult = await memoService.CreateAsync(model);
            ViewBag.Updated = commandResult.IsSuccessful;

            switch (commandResult.IsSuccessful)
            {
            case true when model.SendMail:
                await SendMailAsync(model.Subject, commandResult);
		            break;
	            case false:
		            ModelState.AddModelError(string.Empty, commandResult.Message);
		            break;
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
        }

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Memo)]
    public async Task<IActionResult> Delete(int id)
    {
        var memo = await memoService.GetByIdAsync(id, CurrentCompanyId);
        var commandResult = await memoService.DeleteAsync(id, CurrentCompanyId);

        if (commandResult.IsSuccessful && memo.MemoType == UploadMemoType)
        {
            var allFiles = memo.AttachFile.Split(';');

            foreach (var fileName in allFiles)
            {
                await minIOService.DeleteFileAsync(Bucket.File, fileName);
            }
        }

        return Json(commandResult);
    }

    [HrmPermission(Policy.Read, FunctionKey.Memo)]
    public async Task<IActionResult> Detail(int id)
    {
        var memo = await memoService.GetByIdAsync(id, CurrentCompanyId);
        ViewBag.UploadBasePath = $"{configuration["AWS:ExternalUrl"]}/{Bucket.File}";

        return View(memo);
    }

    private async Task SendMailAsync(string subject, CommandResult result)
    {
        var bodyTemplate = Utilities.ReadTextFile(Path.Combine(hostingEnvironment.ContentRootPath, "EmailTemplates/MemoNotification.txt"));
        var body = string.Format(bodyTemplate, subject, appSettings.Domain + "memo/detail/" + result);

        await emailService.SendEmailAsync(
            appSettings.EmailGo,
            string.Empty,
            Resources.Messages.MemoSubject + " " + subject,
            body);
        await emailService.SendEmailAsync(
            appSettings.EmailSD,
            string.Empty,
            Resources.Messages.MemoSubject + " " + subject,
            body);
    }

    private static string RemoveFirstSlash(string path)
    {
        if (path.StartsWith('/'))
        {
            path = path[1..];
        }
        return path;
    }
}