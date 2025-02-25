namespace HRM.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.AutoJobs.Constants;
    using HRM.AutoJobs.Helpers;
    using HRM.AutoJobs.Models;
    using HRM.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class PublicScheduleWorker : BaseWorker
    {
        private readonly IScheduleArrangeService scheduleArrangeService;

        public PublicScheduleWorker(
            IScheduleArrangeService scheduleArrangeService,
            List<JobSetting> jobSettings,
            ILogger<PublicScheduleWorker> logfile,
            Fanex.Logging.ILogger logger) : base(logfile, logger)
        {
            this.scheduleArrangeService = scheduleArrangeService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.PublicSchedule);
        }

        public override async Task DoWorkAsync()
        {
            await scheduleArrangeService.PublicScheduleAsync();
        }
    }
}