using HRM.AutoJobs.Services.Interfaces;

namespace HRM.AutoJobs.Services;

using System.Threading.Tasks;
using Criteria;

public class TimeRecordService(IBaseService baseService) : ITimeRecordService
{
    public async Task ImportTimeRecordAsync()
    {
        var command = new ImportTimeRecordCommand();
        await baseService.ExecuteNonQueryAsync(command);
    }
}