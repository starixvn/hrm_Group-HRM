namespace HRM.AutoJobs.Workers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using Helpers;
using Models;
using Services;
using Microsoft.Extensions.Logging;

public class InsertWeeklyDefaultShiftWorker : BaseWorker
{
    private readonly IScheduleArrangeService scheduleArrangeService;

    public InsertWeeklyDefaultShiftWorker(
        IScheduleArrangeService scheduleArrangeService,
        List<JobSetting> jobSettings,
        ILogger<InsertWeeklyDefaultShiftWorker> logfile,
        Fanex.Logging.ILogger logger) : base(logfile, logger)
    {
        this.scheduleArrangeService = scheduleArrangeService;
        jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.InsertWeeklyDefaultShift);
    }

    public override async Task DoWorkAsync()
    {
        if (DateTimeOffset.UtcNow.DayOfWeek == DayOfWeek.Friday)
        {
            await scheduleArrangeService.InsertWeeklyDefaultShiftAsync();
        }
    }
}