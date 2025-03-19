namespace HRM.Website;

using System;
using System.Threading.Tasks;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using GO.UserControl.Client.AspNetCore;
using GO.UserControl.Client.AspNetCore.Session;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Repositories;
using HRM.Core.Services;
using HRM.Letstalk.Core.Services;
using HRM.Letstalk.Core.Services.Interfaces;
using MailDeliver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Repositories.Interfaces;
using Services.ExportExcel;
using Services.ExportExcel.Discipline;
using Services.ExportExcel.Employee;
using Services.ExportExcel.WeekendLeave;
using Services.Implement;
using Services.Implement.ActionLog;
using Services.Implement.Report;
using Services.Implement.Request;
using Services.Interfaces;
using Services.Interfaces.Report;
using Services.Interfaces.Request;
using StackExchange.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = configuration.GetSection("AuthSettings:LoginPath").Value;
                options.LogoutPath = configuration.GetSection("AuthSettings:LogoutPath").Value;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:CookieTimeOut").Value));
                options.Cookie = new CookieBuilder
                {
                    Name = "HRM.Website.Cookies",
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
                options.Events.OnSigningOut = x =>
                {
                    x.Response.Redirect(options.LoginPath);
                    return Task.CompletedTask;
                };
            });

        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedisCache(configuration.GetSection("fanex.caching.redis"));

        var redisConfig = configuration.GetSection("AuthSettings:RedisConfig").Value;
        var redisMultiplexer = ConnectionMultiplexer.Connect(redisConfig);

        services.AddDataProtection().PersistKeysToStackExchangeRedis(redisMultiplexer, "HRM.Website:ProtectionKeys");

        services.AddSingleton(redisMultiplexer);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig;
            options.InstanceName = "HRM.Website:Session:";
        });

        return services;
    }

    public static IServiceCollection AddRedisSession(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSession(options =>
        {
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:SessionTimeOut").Value));
            options.Cookie.Name = "HRM.Website.Session";
        });

        return services;
    }

    public static IServiceCollection AddDIServices(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
        services.AddSingleton(appSettings);

        var letstalkSettings = configuration.GetSection("LetstalkSettings").Get<LetstalkSettings>();
        services.AddSingleton(letstalkSettings);

        var s3Config = configuration.GetSection("AmazonStorageConfig").Get<AmazonStorageConfig>();
        services.AddSingleton(s3Config);

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IUserControlSettings>(new UserControlSettings(configuration));
        services.AddSingleton<ILogger, Logger>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();

        services.AddSingleton<IBaseRepository, BaseRepository>();
        services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
        services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
        services.AddSingleton<ITeamRepository, TeamRepository>();
        services.AddSingleton<ISectionRepository, SectionRepository>();
        services.AddSingleton<IRequestRepository, RequestRepository>();

        services.AddSingleton<IUserControlSession, UserControlSession>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IRoleService, RoleService>();
        services.AddSingleton<IFunctionService, FunctionService>();

        services.AddSingleton<IBaseService, BaseService>();
        services.AddSingleton<IBaseMailService, BaseMailService>();
        services.AddSingleton<IHttpClientService, HttpClientService>();

        services.AddSingleton<IHRMAuthService, HRMAuthService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IDashboardService, DashboardService>();
        services.AddSingleton<IAnnualLeaveBalanceService, AnnualLeaveBalanceService>();
        services.AddSingleton<IEmployeeService, EmployeeService>();
        services.AddSingleton<IUnitService, UnitService>();
        services.AddSingleton<IDepartmentService, DepartmentService>();
        services.AddSingleton<ISectionService, SectionService>();
        services.AddSingleton<IJobLevelService, JobLevelService>();
        services.AddSingleton<IMemoService, MemoService>();

        services.AddSingleton<IRequestService, RequestService>();
        services.AddSingleton<IAttachFileService, AttachFileService>();
        services.AddSingleton<ICommonDataService, CommonDataService>();
        services.AddSingleton<IRequestMailService, RequestMailService>();
        services.AddSingleton<ILetstalkService, LetstalkService>();
        services.AddSingleton<ISendNotificationService, SendNotificationService>();
        services.AddSingleton<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddSingleton<ILeaveRequestService, LeaveRequestService>();
        services.AddSingleton<ICheckInOutRequestService, CheckInOutRequestService>();
        services.AddSingleton<ICheckInOutRequestRepository, CheckInOutRequestRepository>();
        services.AddSingleton<ITeamService, TeamService>();
        services.AddSingleton<ILateInEarlyOutRequestService, LateInEarlyOutRequestService>();
        services.AddSingleton<ILateInEarlyOutRequestRepository, LateInEarlyOutRequestRepository>();
        services.AddSingleton<IChangeShiftRequestService, ChangeShiftRequestService>();
        services.AddSingleton<IChangeShiftRequestRepository, ChangeShiftRequestRepository>();
        services.AddSingleton<IKeepInViewRequestService, KeepInViewRequestService>();
        services.AddSingleton<IKeepInViewRequestRepository, KeepInViewRequestRepository>();
        services.AddSingleton<IShiftService, ShiftService>();
        services.AddSingleton<IExtensionNumberService, ExtensionNumberService>();

        services.AddSingleton<IOvertimeRequestService, OvertimeRequestService>();
        services.AddSingleton<IOvertimeRequestRepository, OvertimeRequestRepository>();
        services.AddSingleton<IWorkingCalendarService, WorkingCalendarService>();
        services.AddSingleton<IWorkingCalendarRepository, WorkingCalendarRepository>();
        services.AddSingleton<IDisciplineService, DisciplineService>();
        services.AddSingleton<IDisciplineRepository, DisciplineRepository>();
        services.AddSingleton<IScheduleArrangeService, ScheduleArrangeService>();
        services.AddSingleton<IScheduleArrangeRepository, ScheduleArrangeRepository>();
        services.AddSingleton<ISyncWorkingShiftService, SyncWorkingShiftService>();
        services.AddSingleton<IEmployeeLogService, EmployeeLogService>();
        services.AddSingleton<IActionLogService, ActionLogService>();
        services.AddSingleton<IEmployeeWorkingHoursService, EmployeeWorkingHoursService>();
        services.AddSingleton<IEmployeeWorkingHoursRepository, EmployeeWorkingHoursRepository>();
        services.AddSingleton<IManagementLevelService, ManagementLevelService>();
        services.AddSingleton<IPaternityService, PaternityService>();
        services.AddSingleton<IPostnatalService, PostnatalService>();
        services.AddSingleton<IHolidayService, HolidayService>();
        services.AddSingleton<IMinIOService, MinIOService>();
        services.AddDIReportServices();
        services.AddDIExcelServices();

        return services;
    }

    public static IServiceCollection AddDIReportServices(this IServiceCollection services)
    {
        services.AddSingleton<IReportRepository, ReportRepository>();
        services.AddSingleton<IOvertimeReportRepository, OvertimeReportRepository>();
        services.AddSingleton<IWorkingCalendarSummaryReportRepository, WorkingCalendarSummaryReportRepository>();

        services.AddSingleton<IReportService, ReportService>();
        services.AddSingleton<IShiftStatisticReportService, ShiftStatisticReportService>();
        services.AddSingleton<IWorkingCalendarReportService, WorkingCalendarReportService>();
        services.AddSingleton<IOvertimeReportService, OvertimeReportService>();
        services.AddSingleton<IWorkingHoursSummaryReportService, WorkingHoursSummaryReportService>();
        services.AddSingleton<IManpowerReportService, ManpowerReportService>();
        services.AddSingleton<IWorkingCalendarSummaryReportService, WorkingCalendarSummaryReportService>();
        services.AddSingleton<IMealAllowanceSummaryService, MealAllowanceSummaryService>();

        return services;
    }

    public static IServiceCollection AddDIExcelServices(this IServiceCollection services)
    {
        services.AddSingleton<IExportExcelService, ExportExcelService>();
        services.AddSingleton<IExportExcelUnpaidLeaveSummaryReportService, ExportExcelUnpaidLeaveSummaryReportService>();
        services.AddSingleton<IExportExcelAttendanceReportService, ExportExcelAttendanceReportService>();
        services.AddSingleton<IExportExcelTimeRecordService, ExportExcelTimeRecordService>();
        services.AddSingleton<IExportOvertimeService, ExportOvertimeService>();
        services.AddSingleton<IExportExcelShiftStatisticService, ExportExcelShiftStatisticService>();
        services.AddSingleton<IJobTitleExportService, JobTitleExportService>();
        services.AddSingleton<IExportDisciplineSummaryService, ExportDisciplineSummaryService>();
        services.AddSingleton<IExportDisciplineDetailService, ExportDisciplineDetailService>();
        services.AddSingleton<IExportWeekendLeaveSummaryService, ExportWeekendLeaveSummaryService>();
        services.AddSingleton<IExportSupervisorService, ExportSupervisorService>();
        services.AddSingleton<IExportWeekendLeaveDetailService, ExportWeekendLeaveDetailService>();
        services.AddSingleton<IExportDisciplineSummaryPerYearService, ExportDisciplineSummaryPerYearService>();
        services.AddSingleton<IExportExcelScheduleService, ExportExcelScheduleService>();
        services.AddSingleton<IExportExcelTimeRecordService, ExportExcelTimeRecordService>();
        services.AddSingleton<IExportExcelEmployeeInfoService, ExportExcelEmployeeInfoService>();

        return services;
    }
}