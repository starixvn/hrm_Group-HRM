using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Security;
using HRM.Core.Services;
using HRM.Criteria.Auth;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HRM.Services.Implement;

public class HRMAuthService(
    IUserControlSession session,
    IUserService userService,
    IBaseService baseService) : IHRMAuthService
{
    public async Task<EmployeeSession> GetByUserIdAsync(int userId)
    {
        var companyId = session.Get<short?>(SessionKey.CurrentCompanyId);
        var criteria = new GetEmployeeLoginByUserIdCriteria
        {
            UserId = userId,
            CompanyId = companyId
        };
        var employee = await baseService.GetDataEntityAsync<EmployeeSession>(criteria);

        if (employee == null)
        {
            var user = await userService.GetByIdAsync(userId);
            employee = new EmployeeSession
            {
                EmpId = user.UserId,
                Nickname = user.Nickname,
                FullName = user.FullName
            };
        }

        return employee;
    }

    public int Validate(EmployeeSession employee, string password)
    {
        if (employee == null)
        {
            return (short)ValidateUserStatus.WrongUsername;
        }

        if (!employee.Enabled)
        {
            return (short)ValidateUserStatus.Disabled;
        }

        if (Encryptor.CompareStringToHash(password, employee.Password))
        {
            return (short)ValidateUserStatus.Success;
        }

        if (string.IsNullOrWhiteSpace(employee.Password))
        {
            return (short)ValidateUserStatus.WrongPassword;
        }

        return (short)ValidateUserStatus.Deleted;
    }

    public string GetClientIp(HttpContext httpContext)
    {
        var clientIp = httpContext.Request.Headers["x-forwarded-for"].ToString();
        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
        else
        {
            var forwardedIps = clientIp.Split([','], StringSplitOptions.RemoveEmptyEntries);

            clientIp = forwardedIps[0];
        }

        return clientIp;
    }
}