using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Services;
using HRM.Criteria.AccessCounter;

namespace HRM.Services.Implement;

public interface IAccessCounterService
{
    Task UpdateCounterAsync(string functionName);
}

public class AccessCounterService(
    IUserControlSession session,
    IBaseService baseService) : IAccessCounterService
{
    public async Task UpdateCounterAsync(string functionName)
    {
        var command = new UpdateAccessCounterCommand
        {
            EmpId = session.UserId,
            Date = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            FunctionName = functionName
        };
        await baseService.ExecuteNonQueryAsync(command);
    }
}