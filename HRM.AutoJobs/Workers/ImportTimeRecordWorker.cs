namespace HRM.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.AutoJobs.Constants;
    using HRM.AutoJobs.Helpers;
    using HRM.AutoJobs.Models;
    using HRM.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class ImportTimeRecordWorker : BaseWorker
    {
        private readonly ITimeRecordService timeRecordService;

        public ImportTimeRecordWorker(
            ITimeRecordService timeRecordService,
            List<JobSetting> jobSettings,
            ILogger<ImportTimeRecordWorker> logfile,
            Fanex.Logging.ILogger logger) : base(logfile, logger)
        {
            this.timeRecordService = timeRecordService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.ImportTimeRecord);
        }

        public override async Task DoWorkAsync()
        {
            await timeRecordService.ImportTimeRecordAsync();
        }
    }
}