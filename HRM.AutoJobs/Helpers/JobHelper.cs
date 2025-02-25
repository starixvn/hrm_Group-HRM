namespace HRM.AutoJobs.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.AutoJobs.Models;

    public static class JobHelper
    {
        public static JobSetting GetJobConfig(IEnumerable<JobSetting> jobSettings, string jobName)
        {
            return jobSettings.FirstOrDefault(job => job.JobName.Equals(jobName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsEnabled(IEnumerable<JobSetting> jobSettings, string jobName)
        {
            var settings = GetJobConfig(jobSettings, jobName);

            return settings != null && settings.Enabled;
        }
    }
}