namespace HRM.AutoJobs.Models;

using System;

[Serializable]
public class JobSetting
{
    public string JobName { get; set; }
    public string CronExpression { get; set; }
    public bool Enabled { get; set; }
}