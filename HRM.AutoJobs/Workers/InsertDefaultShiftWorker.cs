using HRM.AutoJobs.Services.Interfaces;

namespace HRM.AutoJobs.Workers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using Helpers;
using Models;
using Microsoft.Extensions.Logging;

public class InsertDefaultShiftWorker : BaseWorker
{
    private readonly IScheduleArrangeService scheduleArrangeService;

    public InsertDefaultShiftWorker(
        IScheduleArrangeService scheduleArrangeService,
        List<JobSetting> jobSettings,
        ILogger<InsertDefaultShiftWorker> logfile,
        Fanex.Logging.ILogger logger) : base(logfile, logger)
    {
        this.scheduleArrangeService = scheduleArrangeService;
        jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.InsertDefaultShift);
    }

    public override async Task DoWorkAsync()
    {
        if (DateTimeOffset.UtcNow.DayOfWeek == DayOfWeek.Friday)
        {
            await scheduleArrangeService.InsertDefaultShiftAsync();
        }
    }
}