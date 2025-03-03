namespace HRM.Controllers;

using System;
using System.IO;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using Core;
using Core.Constants;
using Core.Filters;
using HRM.Core.Models;
using Models;
using Services;
using ViewModels.Memo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.Implement;

public class MemoController(
    AppSettings appSettings,
    IWebHostEnvironment env,
    IUserControlSession userControlSession,
    IMemoService memoService,
    ICommonDataService commonDataService,
    IEmailService emailService) : BaseController(userControlSession)
{
    private const int UploadMemoType = 1;

    [HrmPermission(Policy.Read, FunctionKey.Memo)]
    public async Task<IActionResult> Index(int? year, int category = -1)
    {
        var model = new MemoViewModel
        {
            MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory),
            Category = category
        };

        model.Year = year ?? model.Year;

        model.Memos = await memoService.GetAllAsync(model.Year, model.Category);
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Memo, Policy.Host);

        return View(model);
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.Memo)]
    public async Task<IActionResult> Edit(int id)
    {
        var memo = await memoService.GetByIdAsync(id);
        ViewBag.MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory);
        ViewBag.Updated = false;

        return View(memo);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Memo)]
    public async Task<IActionResult> Edit(Memo model)
    {
        ViewBag.MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory);
        if (model.MemoType == UploadMemoType && string.IsNullOrEmpty(model.AttachFile))
        {
            ViewBag.Updated = false;
            return View(model);
        }

        model.Contents = model.MemoType == UploadMemoType ? model.AttachFile : model.Contents;

        if (ModelState.IsValid)
        {
            var commandResult = await memoService.EditAsync(model);
            ViewBag.Updated = commandResult.IsSuccessful;

            if (commandResult.IsSuccessful && model.SendMail)
            {
                SendMail(model.Subject, commandResult);
            }

            if (!commandResult.IsSuccessful)
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
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
            SentDate = DateTimeOffset.Now.DateTime,
            Contents = "[Contents]",
            MemoType = UploadMemoType
        };

        ViewBag.MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory);
        ViewBag.Updated = false;

        return View(memo);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Memo)]
    public async Task<IActionResult> Create(Memo model)
    {
        ViewBag.MemoCategories = await commonDataService.GetLookupDataByCodeAsync(Constants.MemoCategory);
        model.Contents = model.MemoType == UploadMemoType ? model.AttachFile : model.Contents;

        if (ModelState.IsValid)
        {
            var commandResult = await memoService.CreateAsync(model);
            ViewBag.Updated = commandResult.IsSuccessful;

            if (commandResult.IsSuccessful && model.SendMail)
            {
                SendMail(model.Subject, commandResult);
            }

            if (!commandResult.IsSuccessful)
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
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
        var memo = await memoService.GetByIdAsync(id);
        var commandResult = await memoService.DeleteAsync(id);

        if (commandResult.IsSuccessful && memo.MemoType == UploadMemoType)
        {
            var allFiles = memo.AttachFile.Split(';');

            foreach (var fileName in allFiles)
            {
                var filePath = Path.Combine(env.ContentRootPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        return Json(commandResult);
    }

    [HrmPermission(Policy.Read, FunctionKey.Memo)]
    public async Task<IActionResult> Detail(int id)
    {
        var memo = await memoService.GetByIdAsync(id);

        return View(memo);
    }

    private void SendMail(string subject, CommandResult result)
    {
        var bodyTemplate = Utilities.ReadTextFile(Path.Combine(env.ContentRootPath, "EmailTemplates/MemoNotification.txt"));
        var body = string.Format(bodyTemplate, subject, appSettings.Domain + "memo/detail/" + result);

        emailService.SendEmail(
            appSettings.EmailGo,
            string.Empty,
            Resources.Messages.MemoSubject + " " + subject,
            body);
        emailService.SendEmail(
            appSettings.EmailSD,
            string.Empty,
            Resources.Messages.MemoSubject + " " + subject,
            body);
    }
}