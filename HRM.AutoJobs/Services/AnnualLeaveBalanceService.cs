using HRM.AutoJobs.Services.Interfaces;

namespace HRM.AutoJobs.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Criteria;
using Models;
using Models.MailDeliver;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

public class AnnualLeaveBalanceService(
    IWebHostEnvironment hostEnvironment,
    ILogger<AnnualLeaveBalanceService> logfile,
    IBaseService baseService,
    IBaseMailService mailService,
    ILetstalkService letstalkService)
    : IAnnualLeaveBalanceService
{
    private const string RemindUseALTemplatePath = "EmailTemplates/RemindUseAL.txt";
    private const string StatisticALBalanceTemplatePath = "EmailTemplates/StatisticALBalance.txt";

    private readonly ILogger logfile = logfile;

    public async Task InsertForNextYearAsync()
    {
        var command = new UpdateAnnualLeaveBalanceCommand
        {
            Year = DateTimeOffset.Now.Year + 1
        };
        await baseService.ExecuteNonQueryAsync(command);
    }

    public async Task InsertForNewEmployeeAsync()
    {
        var command = new InsertAnnualLeaveBalanceForNewEmployeeCommand
        {
            Year = DateTimeOffset.Now.Year
        };
        await baseService.ExecuteNonQueryAsync(command);
    }

    public async Task RemindUsedALAsync()
    {
        var year = DateTimeOffset.Now.Year;
        var criteria = new GetAnnualLeaveBalanceCriteria
        {
            Year = year
        };
        var (employees, supervisors, statistics) = await baseService.FetchMultipleAsync<AnnualLeaveBalance, EmployeeSupervisor, SectionStatisticAnnualLeave>(criteria);
        var bodyTemplate = ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, RemindUseALTemplatePath));
        var validEmployees = new List<AnnualLeaveBalance>();

        foreach (var employee in employees)
        {
            var percentAnnualLeaveRemaining = employee.LeaveBalance / employee.AnnualLeaveCurrentYear * 100;
            var isSentMail = false;
            var percentInMail = string.Empty;

            CheckSendMail(ref isSentMail, employee.IsLocal, ref percentInMail, percentAnnualLeaveRemaining);

            validEmployees.Add(employee);

            var body = string.Format(
                bodyTemplate,
                employee.Nickname,
                percentInMail,
                year + 1,
                $"{employee.LeaveBalance}/{employee.AnnualLeaveCurrentYear}");

            if (!string.IsNullOrWhiteSpace(employee.Email) && isSentMail)
            {
                var mailItem = new MailRecord
                {
                    MailTo = employee.Email,
                    MailCc = string.Empty,
                    Subject = "Annual Leave Balance Reminder",
                    Body = body
                };
                var thread = new Thread(async () => await SendEmailAsync(mailItem));
                thread.Start();
            }

            var letstalkThread = new Thread(async () => await SendLetstalkNotificationAsync(employee.Letstalk, "Annual Leave Balance Reminder", body));
            letstalkThread.Start();
        }

        SendMailStatisticAnnualLeave(validEmployees, supervisors, statistics);
    }

    private void SendMailStatisticAnnualLeave(
        List<AnnualLeaveBalance> validEmployees,
        IEnumerable<EmployeeSupervisor> supervisors,
        IEnumerable<SectionStatisticAnnualLeave> statistics)
    {
        foreach (var item in statistics)
        {
            var totalEmployees = validEmployees.Count(x => x.SectionId == item.SectionId && x.DepartmentId == item.DepartmentId);
            var percentage = Math.Round((double)totalEmployees / item.TotalEmployee * 100);

            item.AnnualLeaveBalance = totalEmployees;
            item.Percentage = percentage;
        }

        var percentByMonth = GetPercentByMonth();

        var groupSupervisors = supervisors
            .GroupBy(g => new { g.Nickname, g.OwnerId, g.SectionId, g.DepartmentId, g.Email })
            .GroupBy(x => x.Key.OwnerId).ToList();

        const string tableALTemplate = "<tr><td style=\"border: 1px solid;\">{0}</td>"
                                       + "<td style=\"border: 1px solid;\">{1}</td>"
                                       + "<td style=\"text-align: center; border: 1px solid;\">{2}</td>"
                                       + "<td style=\"text-align: center; border: 1px solid;\">{3}</td>"
                                       + "<td style=\"text-align: center; border: 1px solid;\">{4}%</td></tr>";

        foreach (var groupSupervisor in groupSupervisors)
        {
            var tableStatistic = new StringBuilder();
            var nickname = groupSupervisor.First().Key.Nickname;
            var email = groupSupervisor.First().Key.Email;

            var sections = groupSupervisor.Select(x => x.Key).ToList();
            foreach (var item in sections)
            {
                var statisticSection = statistics.FirstOrDefault(x => x.SectionId == item.SectionId && x.DepartmentId == item.DepartmentId);
                if (statisticSection != null)
                {
                    tableStatistic.AppendFormat(
                        tableALTemplate,
                        statisticSection.Department,
                        statisticSection.Section,
                        statisticSection.TotalEmployee,
                        statisticSection.AnnualLeaveBalance,
                        statisticSection.Percentage
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var bodyTemplate = ReadTextFile(Path.Combine(hostEnvironment.ContentRootPath, StatisticALBalanceTemplatePath));
                var body = string.Format(
                    bodyTemplate,
                    nickname,
                    percentByMonth,
                    tableStatistic,
                    DateTimeOffset.Now.Year + 1);

                var mailItem = new MailRecord
                {
                    MailTo = email,
                    MailCc = string.Empty,
                    Subject = "Department / Section Annual Leave Balance Reminder",
                    Body = body
                };
                var thread = new Thread(async () => await SendEmailAsync(mailItem));
                thread.Start();
            }
        }
    }

    private async Task SendEmailAsync(MailRecord mailRecord)
    {
        try
        {
            await mailService.SendEmailAsync(mailRecord);
        }
        catch (Exception ex)
        {
            logfile.LogError(ex, ex.Message);
        }
    }

    private async Task SendLetstalkNotificationAsync(string letstalkId, string subject, string message)
    {
        try
        {
            await letstalkService.SendNotificationAsync(letstalkId, subject, message);
        }
        catch (Exception ex)
        {
            logfile.LogError(ex, ex.Message);
        }
    }

    private static string ReadTextFile(string filePath)
    {
        using var sr = new StreamReader(filePath);
        return sr.ReadToEnd();
    }

    private static string GetPercentByMonth()
    {
        var percent = string.Empty;
        const byte March = 3;
        const byte June = 6;
        const byte September = 9;
        const byte December = 12;

        switch (DateTimeOffset.Now.Month)
        {
            case March:
                percent = "> 75%";
                break;

            case June:
                percent = "> 50%";
                break;

            case September:
                percent = "> 25%";
                break;

            case December:
                percent = "> 0%";
                break;
            default: 
                percent = percent;
                break;
        }
        return percent;
    }

#pragma warning disable S1541 // Methods and properties should not be too complex

    private static void CheckSendMail(ref bool isSentMail, bool isLocal, ref string percentInMail, float percentAnnualLeaveRemaining)
#pragma warning restore S1541 // Methods and properties should not be too complex
    {
        const byte March = 3;
        const byte June = 6;
        const byte September = 9;
        const byte December = 12;
        const byte TwentyFive = 25;
        const byte Fifty = 50;
        const byte SeventyFive = 75;
        switch (DateTimeOffset.Now.Month)
        {
            case March:
                if (!isLocal && percentAnnualLeaveRemaining > SeventyFive)
                {
                    isSentMail = true;
                    percentInMail = "over 75% ";
                }
                break;

            case June:
                if (percentAnnualLeaveRemaining > Fifty)
                {
                    isSentMail = true;
                    percentInMail = "over 50% ";
                }
                break;

            case September:
                if (!isLocal && percentAnnualLeaveRemaining > TwentyFive)
                {
                    isSentMail = true;
                    percentInMail = "over 25% ";
                }
                break;

            case December:
                if (percentAnnualLeaveRemaining > 0)
                {
                    isSentMail = true;
                }
                break;
            default:
                percentInMail = string.Empty;
                break;

        }
    }
}