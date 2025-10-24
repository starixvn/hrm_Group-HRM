using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Models;
using NSubstitute;

namespace HRM.Controllers.UnitTests;

public abstract class BaseControllerTest
{
    protected readonly IUserControlSession _session;

    protected BaseControllerTest()
    {
        _session = Substitute.For<IUserControlSession>();
        SetupCommonSession();
    }

    protected virtual void SetupCommonSession()
    {
        SetupCompanyId(3);
        SetupUserId(1);
        SetupEmployeeSession(1);
        SetupTimezone(7);
    }

    protected void SetupPermission(string functionKey, Policy policy, bool hasPermission)
    {
        _session.HasPermissionOnFunction(functionKey, policy).Returns(hasPermission);
    }

    protected void SetupCompanyId(int companyId)
    {
        _session.Get<short>(SessionKey.CurrentCompanyId).Returns((short)companyId);
    }

    protected void SetupUserId(int userId)
    {
        _session.UserId.Returns(userId);
    }

    protected void SetupEmployeeSession(int empId)
    {
        var employeeSession = new EmployeeSession { EmpId = empId };
        _session.Get<EmployeeSession>(Arg.Any<string>()).Returns(employeeSession);
    }

    protected void SetupTimezone(float timezoneOffset)
    {
        _session.Get<float>(Arg.Any<string>()).Returns(timezoneOffset);
        DateTimeExtensions.Configure(_session);
    }
}