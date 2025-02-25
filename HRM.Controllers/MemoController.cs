using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Configuration;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Models;
    using HRM.Services;
    using HRM.ViewModels.Memo;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class MemoController(
        IUserControlSession userControlSession,
        IMemoService memoService,
        ICommonDataService commonDataService,
        IEmailService emailService) : BaseController(userControlSession)
    {
        private const int UploadMemoType = 1;
        private readonly IUserControlSession userControlSession = userControlSession;

        [HrmPermission(Policy.Read, FunctionKey.Memo)]
        public ActionResult Index(int? year, int category = -1)
        {
            var model = new MemoViewModel
            {
                MemoCategories = commonDataService.GetLookupDataByCode(Constants.MemoCategory),
                Category = category
            };

            model.Year = year ?? model.Year;

            model.Memos = memoService.GetAll(model.Year, model.Category);
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Memo, Policy.Host);

            return View(model);
        }

        [HttpGet]
        [HrmPermission(Policy.Host, FunctionKey.Memo)]
        public ActionResult Edit(int id)
        {
            Memo memo = memoService.GetById(id);
            ViewBag.MemoCategories = commonDataService.GetLookupDataByCode(Constants.MemoCategory);
            ViewBag.Updated = false;

            return View(memo);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HrmPermission(Policy.Host, FunctionKey.Memo)]
        public ActionResult Edit(Memo model)
        {
            ViewBag.MemoCategories = commonDataService.GetLookupDataByCode(Constants.MemoCategory);
            if (model.MemoType == UploadMemoType && string.IsNullOrEmpty(model.AttachFile))
            {
                ViewBag.Updated = false;
                return View(model);
            }

            model.Contents = model.MemoType == UploadMemoType ? model.AttachFile : model.Contents;

            if (ModelState.IsValid)
            {
                CommandResult commandResult = memoService.Edit(model);
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
        public ActionResult Create()
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

            ViewBag.MemoCategories = commonDataService.GetLookupDataByCode(Constants.MemoCategory);
            ViewBag.Updated = false;

            return View(memo);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HrmPermission(Policy.Host, FunctionKey.Memo)]
        public ActionResult Create(Memo model)
        {
            ViewBag.MemoCategories = commonDataService.GetLookupDataByCode(Constants.MemoCategory);
            model.Contents = model.MemoType == UploadMemoType ? model.AttachFile : model.Contents;

            if (ModelState.IsValid)
            {
                CommandResult commandResult = memoService.Create(model);
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
        public JsonResult Delete(int id)
        {
            Memo memo = memoService.GetById(id);
            CommandResult commandResult = memoService.Delete(id);

            if (commandResult.IsSuccessful && memo.MemoType == UploadMemoType)
            {
                var allFiles = memo.AttachFile.Split(';');

                foreach (var fileName in allFiles)
                {
                    System.IO.File.Delete(Server.MapPath("~/" + fileName));
                }
            }

            return Json(commandResult);
        }

        [HrmPermission(Policy.Read, FunctionKey.Memo)]
        public ActionResult Detail(int id)
        {
            Memo memo = memoService.GetById(id);

            return View(memo);
        }

        private void SendMail(string subject, CommandResult result)
        {
            var link = Request.Url.Scheme + "://" + Request.Host.Value + Request.ApplicationPath;
            var groupSd = ConfigurationManager.AppSettings["EmailSD"];
            var groupGo = ConfigurationManager.AppSettings["EmailGo"];

            var bodyTemplate = Utilities.ReadTextFile(Server.MapPath("~/app_data/emails/MemoNotification.txt"));
            var body = string.Format(bodyTemplate, subject, link + "memo/detail/" + result);

            emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                groupGo,
                string.Empty,
                Resources.Messages.MemoSubject + " " + subject,
                body);
            emailService.SendEmail(
               ConfigurationManager.AppSettings["Email"],
               groupSd,
               string.Empty,
               Resources.Messages.MemoSubject + " " + subject,
               body);
        }
    }
}