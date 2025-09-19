namespace HRM.Models.Report.MonthlyWorkingHours;

using System.Collections.Generic;
using Core.Constants;

public class MonthlyWorking
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public int WorkingDays { get; set; }
    public bool Enabled { get; set; }
    public bool IsDeleted { get; set; }

    public string UnitCode { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }

    public decimal ActualWorkingDays
    {
        get
        {
            decimal workingDays = 0;

            foreach (var dailyWorking in DailyWorkings)
            {
                if (dailyWorking.IsLeaveDay)
                {
                    workingDays += dailyWorking.IsLeaveHalfDay && dailyWorking.TotalWorkingHours > 0 ? Constants.HalfDayOffValue : 0;
                }
                else
                {
                    workingDays += dailyWorking.TotalWorkingHours > 0 ? 1 : 0;
                }
            }

            return workingDays;
        }
    }

    public int TeamId { get; set; }
    public string TeamCode { get; set; }
    public string TeamName { get; set; }
    public IEnumerable<DailyWorking> DailyWorkings { get; set; }
}