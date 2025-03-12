namespace HRM.Controllers;

using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.Services;
using Fanex.Logging;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.Auth;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Core.Constants;
using Core.Filters;
using Criteria.Time;
using Models;
using Services;
using ViewModels.Time;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetTools;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

public class TimeController(
    AppSettings appSettings,
    IUserControlSession userControlSession,
    IBaseService baseService,
    IAuthService authService,
    ICryptoService cryptoService,
    IHRMAuthService hrmAuthService) : BaseController(userControlSession)
{
    [AllowAnonymous]
    public ActionResult CheckInOut()
    {
        if (appSettings.WhitelistIPs.Equals("*"))
        {
            return View();
        }

        var clientIpAddress = GetClientIp(HttpContext);
        var clientIp = IPAddress.Parse(clientIpAddress);
        if (appSettings.WhitelistIPs.Split(';').ToList().Exists(ip => IPAddressRange.Parse(ip).Contains(clientIp)))
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
    public async Task<IActionResult> GetCheckInOutOnlineRecord()
    {
        var criteria = new GetCheckInOutOnlineRecordCriteria
        {
            Date = DateTimeOffset.Now.Date
        };
        var records = await baseService.GetDataAsync<TimeRecordModel>(criteria);
        return Json(records);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CheckInOut(CheckInOutViewModel model)
    {
        model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyiv = token.Split(':');
        var rijndaelKey = keyiv[0];
        var rijndaelIV = keyiv[1];
        var loginParam = new LoginParam
        {
            Username = cryptoService.Encrypt(model.Username, rijndaelKey, rijndaelIV),
            Password = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIV),
            Token = token,
            IP = GetClientIp(HttpContext),
            Platform = HttpContext.Request?.Headers["User-Agent"].ToString(),
            BrowserName = HttpContext.Request?.Headers["User-Agent"].ToString(),
            NetSessionId = HttpContext.Session.Id
        };
        var loginResult = await authService.LoginAsync(loginParam);
        if (loginResult.IsSuccessfull)
        {
            var employee = await hrmAuthService.GetByUserIdAsync(loginResult.UserId);
            if (employee == null)
            {
                loginResult = new LoginResult { IsSuccessfull = false, ErrorMessage = "User has not been mapped." };
                return Json(loginResult);
            }

            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = employee.EmpId,
                RecordTime = DateTimeOffset.Now.DateTime,
                InOutType = model.InOutType,
                IPAddress = GetClientIp(HttpContext),
                Platform = HttpContext.Request?.Headers["User-Agent"].ToString()
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);
            loginResult = new LoginResult { IsSuccessfull = result.IsSuccessful, ErrorMessage = result.Message };
        }

        return Json(loginResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TimeRecord)]
    public async Task<IActionResult> ProcessInOut(UpdateTypeTimeRecordCriteria criteria)
    {
        if (criteria != null)
        {
            criteria.ModifiedBy = EmployeeSession.EmpId;
            criteria.ModifiedDate = DateTimeOffset.Now.DateTime;
        }

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
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
    public async Task<IActionResult> AddTimeRecord(TimeRecordViewModel model)
    {
        if (model != null)
        {
            var criteria = new InsertTimeRecordCriteria
            {
                EmpId = model.EmpId,
                RecordTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                InOutType = model.InOutType != 1
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            ViewBag.Requested = result.IsSuccessful;
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddCheckInTime()
    {
        var criteria = new InsertCheckInOutRecordCriteria
        {
            EmpId = EmployeeSession.EmpId,
            RecordTime = DateTimeOffset.Now.DateTime,
            InOutType = 0,
            IPAddress = GetClientIp(HttpContext),
            Platform = HttpContext.Request?.Headers["User-Agent"].ToString()
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddCheckOutTime()
    {
        var criteria = new InsertCheckInOutRecordCriteria
        {
            EmpId = EmployeeSession.EmpId,
            RecordTime = DateTimeOffset.Now.DateTime,
            InOutType = 1,
            IPAddress = GetClientIp(HttpContext),
            Platform = HttpContext.Request?.Headers["User-Agent"].ToString()
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);
        return Json(result);
    }

    private static string GetClientIp(HttpContext httpContext)
    {
        var clientIp = httpContext.Request.Headers["x-forwarded-for"].ToString();
        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        else
        {
            var forwardedIps = clientIp.Split([','], StringSplitOptions.RemoveEmptyEntries);

            clientIp = forwardedIps[0];
        }

        return clientIp;
    }
}