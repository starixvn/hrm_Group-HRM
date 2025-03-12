namespace HRM.AutoJobs.Workers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using Helpers;
using Models;
using Services;
using Microsoft.Extensions.Logging;

public class RemindUsedALWorker : BaseWorker
{
    private readonly IAnnualLeaveBalanceService annualLeaveBalanceService;

    public RemindUsedALWorker(
        IAnnualLeaveBalanceService annualLeaveBalanceService,
        List<JobSetting> jobSettings,
        ILogger<RemindUsedALWorker> logfile,
        Fanex.Logging.ILogger logger) : base(logfile, logger)
    {
        this.annualLeaveBalanceService = annualLeaveBalanceService;
        jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.RemindUsedAL);
    }

    public override async Task DoWorkAsync()
    {
        var months = new[] { 3, 6, 9, 12 };
        var today = DateTimeOffset.UtcNow;
        if (today.Day == 1 && Array.Exists(months, x => x == today.Month))
        {
            await annualLeaveBalanceService.RemindUsedALAsync();
        }
    }
}