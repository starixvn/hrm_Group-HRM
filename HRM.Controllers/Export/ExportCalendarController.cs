namespace HRM.Controllers;

using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using Core.Constants;
using Criteria.Calendar;
using Services;
using ViewModels;
using Microsoft.AspNetCore.Mvc;

public class ExportCalendarController(
    IUserControlSession userControlSession,
    IWorkingCalendarService workingCalendarService,
    IWorkingCalendarReportService reportService) : BaseExportController(userControlSession)
{
    public async Task<IActionResult> EmployeeWorkingCalendar(WorkingCalendarFilterViewModel model)
    {
        var fileName = $"EmployeeWorkingCalendar_{DateTimeOffset.Now:yyyyMMdd}.xlsx";
        var criteria = new GetWorkingCalendarWeeklyCriteria
        {
            EmpId = model.EmployeeId,
            Fromtime = model.FromDate,
            Totime = model.ToDate,
            TeamIds = model.TeamIds,
            JobTitleId = model.JobTitleIds,
            IsLocal = model.IsLocal,
            CompanyId = CurrentCompanyId
        };

        var workingCalendars = await workingCalendarService.GetWorkingCalendarsAsync(criteria);

        var bytes = reportService.Export(criteria, workingCalendars);
        return File(bytes, MimeType.XLSX, fileName);
    }
}