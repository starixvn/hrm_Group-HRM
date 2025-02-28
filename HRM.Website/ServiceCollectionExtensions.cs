using HRM.Services.Interfaces.Request;

namespace HRM.Website;

using System;
using System.Threading.Tasks;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.NexHR.Repositories;
using GO.UserControl.Client.AspNetCore;
using GO.UserControl.Client.AspNetCore.Session;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Repositories;
using HRM.Core.Services;
using HRM.Letstalk.Core.Services;
using HRM.Repositories.Interfaces;
using HRM.Services.ExportExcel;
using HRM.Services.Implement.Request;
using MailDeliver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Services;
using Services.Implement;
using Services.Interfaces;
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

        services.AddDataProtection().PersistKeysToStackExchangeRedis(redisMultiplexer, "HRM.Website-ProtectionKeys");

        services.AddSingleton(redisMultiplexer);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig;
            options.InstanceName = "HRM.Website.Session-";
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

        var syncWorkingShiftSettings = configuration.GetSection("SyncWorkingShiftSettings").Get<SyncWorkingShiftSettings>();
        services.AddSingleton(syncWorkingShiftSettings);

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IUserControlSettings>(new UserControlSettings(configuration));
        services.AddSingleton<ILogger, Logger>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();

        services.AddSingleton<IBaseRepository, BaseRepository>();
        services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
        services.AddSingleton<IReportRepository, ReportRepository>();
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

        services.AddDIExcelServices();

        services.AddSingleton<IHRMAuthService, HRMAuthService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IDashboardService, DashboardService>();
        services.AddSingleton<IReportService, ReportService>();
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
        services.AddSingleton<IReportService, ReportService>();
        services.AddSingleton<IReportRepository, ReportRepository>();
        services.AddSingleton<IWorkingCalendarService, WorkingCalendarService>();
        services.AddSingleton<IWorkingCalendarRepository, WorkingCalendarRepository>();

        return services;
    }

    public static IServiceCollection AddDIExcelServices(this IServiceCollection services)
    {
        services.AddSingleton<IExportExcelService, ExportExcelService>();
        services.AddSingleton<IExportExcelUnpaidLeaveSummaryReportService, ExportExcelUnpaidLeaveSummaryReportService>();
        services.AddSingleton<IExportExcelAttendanceReportService, ExportExcelAttendanceReportService>();
        services.AddSingleton<IExportExcelTimeRecordService, ExportExcelTimeRecordService>();
        services.AddSingleton<IExportOvertimeService, ExportOvertimeService>();

        return services;
    }
}