using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Criteria.Employee;
    using HRM.Models.Message;
    using HRM.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class MessageController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IMessageService messageService;
        private readonly ICommonDataService commonDataService;
        private readonly IEmployeeService employeeService;

        public MessageController(
            IUserControlSession userControlSession,
            IMessageService messageService,
            ICommonDataService commonDataService,
            IEmployeeService employeeService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.messageService = messageService;
            this.commonDataService = commonDataService;
            this.employeeService = employeeService;
        }

        [HrmPermission(Policy.Read, FunctionKey.Message)]
        public ActionResult Index()
        {
            var currentDateTime = DateTimeOffset.Now.DateTime;
            var param = new GetMessageByCreatorParam
            {
                ToDate = currentDateTime,
                FromDate = currentDateTime.AddMonths(-3),
                IsDisabled = 0,
                CreatedBy = EmployeeSession.EmpId
            };

            ViewBag.Messages = messageService.GetMessageByCreator(param);
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Message, Policy.Host);

            return View(param);
        }

        [HttpPost]
        [HrmPermission(Policy.Read, FunctionKey.Message)]
        public ActionResult Index(GetMessageByCreatorParam param)
        {
            if (param != null)
            {
                param.CreatedBy = EmployeeSession.EmpId;
            }

            ViewBag.Messages = messageService.GetMessageByCreator(param);
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Message, Policy.Host);

            return View(param);
        }

        [HrmPermission(Policy.Host, FunctionKey.Message)]
        public ActionResult Create()
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = null,
                TeamIds = null,
                EnableUser = true,
                IsDeleteUser = null,
                TitleId = null,
                IsLocalUser = null
            };
            ViewBag.Employees = employeeService.GetEmployees(criteria);
            ViewBag.Groups = commonDataService.GetAllTeam();
            var currentDateTime = DateTimeOffset.Now.DateTime;
            var model = new Message { FromDate = currentDateTime, ToDate = currentDateTime };

            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Create(Message model)
        {
            var commandResult = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                commandResult = messageService.Create(EmployeeSession.EmpId, model);
                if (model.HasSendEmail && commandResult.IsSuccessful)
                {
                    messageService.SendMailToStaff(model);
                }
            }

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        [HrmPermission(Policy.Host, FunctionKey.Message)]
        public ActionResult Edit(int id)
        {
            ViewBag.Message = messageService.GetMessageById(id);
            var criteria = new GetAllEmployeeCriteria
            {
                Query = null,
                TeamIds = null,
                EnableUser = true,
                IsDeleteUser = false,
                TitleId = null,
                IsLocalUser = null
            };

            ViewBag.Employees = employeeService.GetEmployees(criteria);
            ViewBag.Groups = commonDataService.GetAllTeam();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [HrmPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Edit(Message model)
        {
            var commandResult = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                commandResult = messageService.Edit(EmployeeSession.EmpId, model);
                if (model.HasSendEmail && commandResult.IsSuccessful)
                {
                    messageService.SendMailToStaff(model);
                }
            }

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Delete(int id)
        {
            CommandResult commandResult = messageService.Delete(EmployeeSession.EmpId, id);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewMessageOfEmployee()
        {
            var currentDateTime = DateTimeOffset.Now.DateTime;
            var param = new GetMessageByEmployeeParam
            {
                FromDate = currentDateTime,
                ToDate = currentDateTime,
                IsDisabled = 0,
                EmployeeId = EmployeeSession.EmpId
            };
            ViewBag.Messages = messageService.GetMessageByEmployee(param);

            return View(param);
        }

        [HttpPost]
        public ActionResult ViewMessageOfEmployee(GetMessageByEmployeeParam param)
        {
            if (param != null)
            {
                param.EmployeeId = EmployeeSession.EmpId;
            }

            ViewBag.Messages = messageService.GetMessageByEmployee(param);

            return View(param);
        }

        [HttpPost]
        public JsonResult DeleteMessageByEmployee(int id)
        {
            CommandResult commandResult = messageService.DeleteMessageByEmployee(EmployeeSession.EmpId, id);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        [HrmPermission(Policy.Read, FunctionKey.Message)]
        public ActionResult Detail(int id)
        {
            Message message = messageService.GetMessageById(id);
            return View(message);
        }

        public ActionResult DetailByEmployee(int id)
        {
            Message message = messageService.GetDetailMessageByEmployee(id);
            return View(message);
        }

        public JsonResult EditMessageStatus(int id)
        {
            CommandResult commandResult = messageService.EditStatus(EmployeeSession.EmpId, id);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }
    }
}