namespace HRM.AutoJobs.Workers;

using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using Helpers;
using Models;
using Services;
using Microsoft.Extensions.Logging;

public class SyncEmployeeInfoToGroupReportWorker : BaseWorker
{
    private readonly IGroupReportService groupReportService;

    public SyncEmployeeInfoToGroupReportWorker(
        IGroupReportService groupReportService,
        List<JobSetting> jobSettings,
        ILogger<SyncWorkingShiftWorker> logfile,
        Fanex.Logging.ILogger logger) : base(logfile, logger)
    {
        this.groupReportService = groupReportService;
        jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncEmployeeInfoToGroupReport);
    }

    public override async Task DoWorkAsync()
    {
        await groupReportService.SyncEmployeeInfoAsync();
    }
}