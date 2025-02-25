using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Core.Models;
    using Core.Services;
    using Fanex.Logging;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Criteria.Time;
    using HRM.Models;
    using HRM.Services;
    using HRM.ViewModels.Time;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Models.Auth;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using NetTools;

    public class TimeController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IAuthService authService;
        private readonly ICryptoService cryptoService;
        private readonly IHRMAuthService hrmAuthService;

        public TimeController(
            IUserControlSession userControlSession,
            IBaseService baseService,
            IAuthService authService,
            ICryptoService cryptoService,
            IHRMAuthService hrmAuthService) : base(userControlSession)
        {
            this.baseService = baseService;
            this.authService = authService;
            this.cryptoService = cryptoService;
            this.hrmAuthService = hrmAuthService;
        }

        [AllowAnonymous]
        public ActionResult CheckInOut()
        {
            if(ConfigSettings.WhitelistIPs.Equals("*"))
            {
                return View();
            }

            var clientIpAddress = GetClientIpAddress();
            var clientIP = IPAddress.Parse(clientIpAddress);
            if (ConfigSettings.WhitelistIPs.Split(';').ToList().Exists(ip => IPAddressRange.Parse(ip).Contains(clientIP)))
            {
                return View();
            }
            else
            {
                LogService.Info(clientIpAddress);
                return RedirectToAction("AccessDenied", "Error");
            }
        }

        [AllowAnonymous]
        public ActionResult GetCheckInOutOnlineRecord()
        {
            var criteria = new GetCheckInOutOnlineRecordCriteria
            {
                Date = DateTimeOffset.Now.Date
            };
            var records = baseService.GetData<TimeRecordModel>(criteria);
            return new CustomJsonResult { Data = records, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckInOut(CheckInOutViewModel model)
        {
            model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
            var token = cryptoService.GenerateRijndaelKeyIV();
            string[] keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            var loginParam = new LoginParam
            {
                Username = cryptoService.Encrypt(model.Username, rijndaelKey, rijndaelIV),
                Password = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIV),
                Token = token,
                IP = GetClientIpAddress(),
                Platform = Request.Browser.Platform,
                BrowserName = Request.Browser.Browser,
                NetSessionId = Session.SessionID
            };
            var loginResult = authService.Login(loginParam);
            if (loginResult.IsSuccessfull)
            {
                EmployeeSession employee = hrmAuthService.GetByUserId(loginResult.UserId);
                if (employee == null)
                {
                    loginResult = new LoginResult { IsSuccessfull = false, ErrorMessage = "User has not been mapped." };
                    return new CustomJsonResult { Data = loginResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                var criteria = new InsertCheckInOutRecordCriteria
                {
                    EmpId = employee.EmpId,
                    RecordTime = DateTimeOffset.Now.DateTime,
                    InOutType = model.InOutType,
                    IPAddress = GetClientIpAddress(),
                    Platform = Request.Headers["User-Agent"]
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);
                loginResult = new LoginResult { IsSuccessfull = result.IsSuccessful, ErrorMessage = result.Message };

                return new CustomJsonResult { Data = loginResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new CustomJsonResult { Data = loginResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.TimeRecord)]
        public JsonResult ProcessInOut(UpdateTypeTimeRecordCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = EmployeeSession.EmpId;
                criteria.ModifiedDate = DateTimeOffset.Now.DateTime;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddTimeRecord()
        {
            var model = new TimeRecordViewModel
            {
                ForDate = DateTimeOffset.Now.Date,
                Hour = DateTimeOffset.Now.Hour,
                Minute = DateTimeOffset.Now.Minute
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddTimeRecord(TimeRecordViewModel model)
        {
            if (model != null)
            {
                var criteria = new InsertTimeRecordCriteria
                {
                    EmpId = model.EmpId,
                    RecordTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                    InOutType = model.InOutType != 1
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

                ViewBag.Requested = result.IsSuccessful;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult AddChekInTime()
        {
            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = EmployeeSession.EmpId,
                RecordTime = DateTimeOffset.Now.DateTime,
                InOutType = 0,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"]
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddChekOutTime()
        {
            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = EmployeeSession.EmpId,
                RecordTime = DateTimeOffset.Now.DateTime,
                InOutType = 1,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"]
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);
            return Json(result);
        }

        public string GetClientIpAddress()
        {
            var clientIp = Request.ServerVariables["X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = Request.UserHostAddress;
            }
            else
            {
                var forwardedIps = clientIp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                clientIp = forwardedIps[forwardedIps.Length - 1];
            }

            return clientIp;
        }
    }
}