namespace HRM.AutoJobs.Workers;

using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using Helpers;
using Models;
using Services;
using Microsoft.Extensions.Logging;

public class UpdateWorkingHourPerDayWorker : BaseWorker
{
    private readonly IPostnatalService postnatalService;

    public UpdateWorkingHourPerDayWorker(
        IPostnatalService postnatalService,
        List<JobSetting> jobSettings,
        ILogger<UpdateWorkingHourPerDayWorker> logfile,
        Fanex.Logging.ILogger logger) : base(logfile, logger)
    {
        this.postnatalService = postnatalService;
        jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.UpdateWorkingHourPerDay);
    }

    public override async Task DoWorkAsync()
    {
        await postnatalService.UpdateWorkingHourPerDayAsync();
    }
}