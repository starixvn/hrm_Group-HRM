namespace HRM.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Constants;
    using Helpers;
    using Models;
    using Services;
    using Microsoft.Extensions.Logging;

    public class InsertALBalanceForNewEmployeeWorker : BaseWorker
    {
        private readonly IAnnualLeaveBalanceService annualLeaveBalanceService;

        public InsertALBalanceForNewEmployeeWorker(
            IAnnualLeaveBalanceService annualLeaveBalanceService,
            List<JobSetting> jobSettings,
            ILogger<InsertALBalanceForNewEmployeeWorker> logfile,
            Fanex.Logging.ILogger logger) : base(logfile, logger)
        {
            this.annualLeaveBalanceService = annualLeaveBalanceService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.InsertALBalanceForNewEmployee);
        }

        public override async Task DoWorkAsync()
        {
            await annualLeaveBalanceService.InsertForNewEmployeeAsync();
        }
    }
}