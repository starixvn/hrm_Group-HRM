using HRM.Criteria.Employee;

namespace HRM.Controllers;

using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Filters;
using Core.Models;
using Core.Services;
using Criteria.CompanySite;
using Criteria.SystemSetting;
using Criteria.Time;
using Fanex.Logging;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.Auth;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using NetTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Services.Interfaces;
using ViewModels.Time;

public class TimeController(
    IUserControlSession session,
    IBaseService baseService,
    IAuthService authService,
    ICryptoService cryptoService,
    IHRMAuthService hrmAuthService,
    ILogger logger,
    IEmployeeService employeeService) : BaseController(session)
{
    [AllowAnonymous]
    public async Task<IActionResult> CheckInOut()
    {
        var companySite = await GetCompanySiteByDomainAsync();
        if (companySite == null)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        session.SiteId = companySite.SiteId;
        session.AccessToken = companySite.AccessToken;
        session.Set(SessionKey.CurrentCompanyId, companySite.CompanyId);
        session.Set(SessionKey.CurrentTimeZone, await GetClientTimeZoneAsync());

        var clientIpAddress = hrmAuthService.GetClientIp(HttpContext);

        var companyBioInfo = await employeeService.GetEmployeeBioInfoAsync(companySite.CompanyId, clientIpAddress);
        if (!companyBioInfo.IsWhitelisted)
        {
            await logger.InfoAsync(clientIpAddress);
            return RedirectToAction("AccessDenied", "Error");
        }

        return View(DateTimeOffset.Now.DateTime.ToClientTimeZone());
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CheckInOut([FromBody] CheckInOutViewModel model)
    {
        var companySite = await GetCompanySiteByDomainAsync();

        session.SiteId = companySite.SiteId;
        session.AccessToken = companySite.AccessToken;
        session.Set(SessionKey.CurrentCompanyId, companySite.CompanyId);
        session.Set(SessionKey.CurrentTimeZone, await GetClientTimeZoneAsync());

        model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyIv = token.Split(':');
        var rijndaelKey = keyIv[0];
        var rijndaelIv = keyIv[1];
        var loginParam = new LoginParam
        {
            Username = cryptoService.Encrypt(model.Username, rijndaelKey, rijndaelIv),
            Password = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIv),
            Token = token,
            IP = hrmAuthService.GetClientIp(HttpContext),
            Platform = HttpContext.Request.Headers["User-Agent"].ToString(),
            BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
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

            var employeeBioInfo = await employeeService.GetEmployeeBioInfoAsync(employee.EmpId, loginParam.IP);

            var isEmployeeSecondmentOrWorkFromHome = employeeBioInfo.IsWorkFromHome || employeeBioInfo.IsSecondment;
            if (employeeBioInfo.HasBio && !isEmployeeSecondmentOrWorkFromHome)
            {
                loginResult = new LoginResult { IsSuccessfull = false, ErrorMessage = "Your submission is unsuccessful. Please contact your supervisor!" };
                return Json(loginResult);
            }

            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = employee.EmpId,
                RecordTime = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                ClientRecordTime = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                InOutType = model.InOutType,
                IPAddress = hrmAuthService.GetClientIp(HttpContext),
                Platform = HttpContext.Request.Headers["User-Agent"].ToString()
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);
            if (employeeBioInfo.IsWFHNearlyExpired)
            {
                result.Message = "Your WFH period is ending soon. Please contact your supervisor!";
            }
            loginResult = new LoginResult { IsSuccessfull = result.IsSuccessful, ErrorMessage = result.Message };
        }

        return Json(loginResult);
    }

    [AllowAnonymous]
    public async Task<IActionResult> GetCheckInOutOnlineRecord()
    {
        var criteria = new GetCheckInOutOnlineRecordCriteria
        {
            CompanyId = CurrentCompanyId,
            Date = DateTimeOffset.Now.Date
        };
        var records = await baseService.GetDataAsync<TimeRecordModel>(criteria);
        return Json(records);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TimeRecord)]
    public async Task<IActionResult> ProcessInOut(UpdateTypeTimeRecordCriteria criteria)
    {
        if (criteria != null)
        {
            criteria.ModifiedBy = session.UserId;
            criteria.ModifiedDate = DateTimeOffset.Now.DateTime;
        }

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    [HttpPost]
    public async Task<IActionResult> AddCheckInTime()
    {
        var criteria = new InsertCheckInOutRecordCriteria
        {
            EmpId = session.UserId,
            RecordTime = DateTimeOffset.Now.DateTime,
            ClientRecordTime = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            InOutType = 0,
            IPAddress = hrmAuthService.GetClientIp(HttpContext),
            Platform = HttpContext.Request.Headers["User-Agent"].ToString()
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddCheckOutTime()
    {
        var criteria = new InsertCheckInOutRecordCriteria
        {
            EmpId = session.UserId,
            RecordTime = DateTimeOffset.Now.DateTime,
            ClientRecordTime = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            InOutType = 1,
            IPAddress = hrmAuthService.GetClientIp(HttpContext),
            Platform = HttpContext.Request.Headers["User-Agent"].ToString()
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);
        return Json(result);
    }

    private async Task<float> GetClientTimeZoneAsync()
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = companyId,
            Key = SystemConfigKey.TimeZone
        };

        return await baseService.GetDataEntityAsync<float>(criteria);
    }

    private async Task<CompanySite> GetCompanySiteByDomainAsync()
    {
        var domainName = GetDomainName(HttpContext);
        var criteria = new GetCompanySiteByDomainCriteria
        {
            Domain = domainName
        };
        return await baseService.GetDataEntityAsync<CompanySite>(criteria);
    }
}