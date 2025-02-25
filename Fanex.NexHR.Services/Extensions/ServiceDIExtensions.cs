namespace Fanex.NexHR.Services.Extensions
{
    using Fanex.NexHR.MailDeliver;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Implement;
    using Fanex.NexHR.Services.Implement.Report;
    using Fanex.NexHR.Services.Interfaces;
    using GO.UserControl.Client.AspNetMvc;
    using GO.UserControl.Client.AspNetMvc.Session;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using HRM.Letstalk.Core.Services;
    using Unity;

    public static class ServiceDIExtensions
    {
        public static void ResolveServiceCoreDI(this IUnityContainer container)
        {
            ResolveRepositoryDI(container);

            // user control
            container.RegisterType<IUserControlSettings, UserControlSettings>(TypeLifetime.Singleton);
            container.RegisterType<IUserControlSession, UserControlSession>(TypeLifetime.Singleton);
            container.RegisterType<IAuthService, AuthService>(TypeLifetime.Singleton);
            container.RegisterType<ICryptoService, CryptoService>(TypeLifetime.Singleton);
            container.RegisterType<IUserService, UserService>(TypeLifetime.Singleton);
            container.RegisterType<IRoleService, RoleService>(TypeLifetime.Singleton);
            container.RegisterType<IFunctionService, FunctionService>(TypeLifetime.Singleton);

            container.RegisterType<IBaseMailService, BaseMailService>(TypeLifetime.Singleton);
            container.RegisterType<IHRMAuthService, HRMAuthService>(TypeLifetime.Singleton);
            container.RegisterType<IEmailService, EmailService>(TypeLifetime.Singleton);
            container.RegisterType<ISectionService, SectionService>(TypeLifetime.Singleton);
            container.RegisterType<IUnitService, UnitService>(TypeLifetime.Singleton);
            container.RegisterType<IEmployeeService, EmployeeService>(TypeLifetime.Singleton);
            container.RegisterType<IExtensionNumberService, ExtensionNumberService>(TypeLifetime.Singleton);
            container.RegisterType<IHolidayService, HolidayService>(TypeLifetime.Singleton);
            container.RegisterType<ICommonDataService, CommonDataService>(TypeLifetime.Singleton);
            container.RegisterType<IMemoService, MemoService>(TypeLifetime.Singleton);
            container.RegisterType<IMessageService, MessageService>(TypeLifetime.Singleton);
            container.RegisterType<IOvertimeRequestService, OvertimeRequestService>(TypeLifetime.Singleton);
            container.RegisterType<IReportService, ReportService>(TypeLifetime.Singleton);
            container.RegisterType<IRequestMailService, RequestMailService>(TypeLifetime.Singleton);
            container.RegisterType<IRequestService, RequestService>(TypeLifetime.Singleton);
            container.RegisterType<IAttachFileService, AttachFileService>(TypeLifetime.Singleton);
            container.RegisterType<IShiftService, ShiftService>(TypeLifetime.Singleton);
            container.RegisterType<ITeamService, TeamService>(TypeLifetime.Singleton);
            container.RegisterType<IWorkingCalendarService, WorkingCalendarService>(TypeLifetime.Singleton);
            container.RegisterType<IChangeShiftRequestService, ChangeShiftRequestService>(TypeLifetime.Singleton);
            container.RegisterType<ICheckInOutRequestService, CheckInOutRequestService>(TypeLifetime.Singleton);
            container.RegisterType<IKeepInViewRequestService, KeepInViewRequestService>(TypeLifetime.Singleton);
            container.RegisterType<ILateInEarlyOutRequestService, LateInEarlyOutRequestService>(TypeLifetime.Singleton);
            container.RegisterType<ILeaveRequestService, LeaveRequestService>(TypeLifetime.Singleton);
            container.RegisterType<IDisciplineService, DisciplineService>(TypeLifetime.Singleton);
            container.RegisterType<IScheduleArrangeService, ScheduleArrangeService>(TypeLifetime.Singleton);
            container.RegisterType<IPostnatalService, PostnatalService>(TypeLifetime.Singleton);
            container.RegisterType<IPaternityService, PaternityService>(TypeLifetime.Singleton);
            container.RegisterType<IDashboardService, DashboardService>(TypeLifetime.Singleton);
            container.RegisterType<IAnnualLeaveBalanceService, AnnualLeaveBalanceService>(TypeLifetime.Singleton);
            container.RegisterType<IEmployeeWorkingHoursService, EmployeeWorkingHoursService>(TypeLifetime.Singleton);
            container.RegisterType<IOvertimeReportService, OvertimeReportService>(TypeLifetime.Singleton);
            container.RegisterType<IWorkingHoursSummaryReportService, WorkingHoursSummaryReportService>(TypeLifetime.Singleton);
            container.RegisterType<IWorkingCalendarSummaryReportService, WorkingCalendarSummaryReportService>(TypeLifetime.Singleton);
            container.RegisterType<IMealAllowanceSummaryService, MealAllowanceSummaryService>(TypeLifetime.Singleton);
            container.RegisterType<IWorkingCalendarReportService, WorkingCalendarReportService>(TypeLifetime.Singleton);
            container.RegisterType<IManpowerReportService, ManpowerReportService>(TypeLifetime.Singleton);
            container.RegisterType<IActionLogService, ActionLogService>(TypeLifetime.Singleton);
            container.RegisterType<IEmployeeLogService, EmployeeLogService>(TypeLifetime.Singleton);
            container.RegisterType<IShiftStatisticReportService, ShiftStatisticReportService>(TypeLifetime.Singleton);
            container.RegisterType<IDepartmentService, DepartmentService>(TypeLifetime.Singleton);
            container.RegisterType<IJobLevelService, JobLevelService>(TypeLifetime.Singleton);
            container.RegisterType<IEmployeeWorkFromHomeService, EmployeeWorkFromHomeService>(TypeLifetime.Singleton);
            container.RegisterType<ISyncWorkingShiftService, SyncWorkingShiftService>(TypeLifetime.Singleton);

            container.RegisterType<IHttpClientService, HttpClientService>(TypeLifetime.Singleton);
            container.RegisterType<ISendNotificationService, SendNotificationService>(TypeLifetime.Singleton);
            container.RegisterType<ILetstalkService, LetstalkService>(TypeLifetime.Singleton);

            RegisterAPIDI(container);

            // excel
            RegisterExportExcelDI(container);
        }

        private static void RegisterExportExcelDI(IUnityContainer container)
        {
            container.RegisterType<IExportExcelService, ExportExcelService>(TypeLifetime.Singleton);
            container.RegisterType<IExportExcelAttendanceReportService, ExportExcelAttendanceReportService>(TypeLifetime.Singleton);
            container.RegisterType<IExportDisciplineDetailService, ExportDisciplineDetailService>(TypeLifetime.Singleton);
            container.RegisterType<IExportDisciplineSummaryService, ExportDisciplineSummaryService>(TypeLifetime.Singleton);
            container.RegisterType<IExportExcelEmployeeInfoService, ExportExcelEmployeeInfoService>(TypeLifetime.Singleton);
            container.RegisterType<IExportSupervisorService, ExportSupervisorService>(TypeLifetime.Singleton);
            container.RegisterType<IExportExcelUnpaidLeaveSummaryReportService, ExportExcelUnpaidLeaveSummaryReportService>(TypeLifetime.Singleton);
            container.RegisterType<IExportWeekendLeaveDetailService, ExportWeekendLeaveDetailService>(TypeLifetime.Singleton);
            container.RegisterType<IExportWeekendLeaveSummaryService, ExportWeekendLeaveSummaryService>(TypeLifetime.Singleton);
            container.RegisterType<IExportExcelScheduleService, ExportExcelScheduleService>(TypeLifetime.Singleton);
            container.RegisterType<IJobTitleExportService, JobTitleExportService>(TypeLifetime.Singleton);
            container.RegisterType<IExportExcelShiftStatisticService, ExportExcelShiftStatisticService>(TypeLifetime.Singleton);
            container.RegisterType<IExportExcelTimeRecordService, ExportExcelTimeRecordService>(TypeLifetime.Singleton);
            container.RegisterType<IExportOvertimeService, ExportOvertimeService>(TypeLifetime.Singleton);
            container.RegisterType<IExportDisciplineSummaryPerYearService, ExportDisciplineSummaryPerYearService>(TypeLifetime.Singleton);
        }

        private static void ResolveRepositoryDI(IUnityContainer container)
        {
            container.RegisterType<ISectionRepository, SectionRepository>();
            container.RegisterType<IEmployeeRepository, EmployeeRepository>();
            container.RegisterType<IOvertimeRequestRepository, OvertimeRequestRepository>();
            container.RegisterType<IReportRepository, ReportRepository>();
            container.RegisterType<IRequestRepository, RequestRepository>();
            container.RegisterType<ITeamRepository, TeamRepository>();
            container.RegisterType<IWorkingCalendarRepository, WorkingCalendarRepository>();
            container.RegisterType<IDisciplineRepository, DisciplineRepository>();
            container.RegisterType<IScheduleArrangeRepository, ScheduleArrangeRepository>();
            container.RegisterType<IKeepInViewRequestRepository, KeepInViewRequestRepository>();
            container.RegisterType<ILeaveRequestRepository, LeaveRequestRepository>();
            container.RegisterType<IEmployeeWorkingHoursRepository, EmployeeWorkingHoursRepository>();
            container.RegisterType<ICheckInOutRequestRepository, CheckInOutRequestRepository>();
            container.RegisterType<ILateInEarlyOutRequestRepository, LateInEarlyOutRequestRepository>();
            container.RegisterType<IOvertimeReportRepository, OvertimeReportRepository>();
            container.RegisterType<IChangeShiftRequestRepository, ChangeShiftRequestRepository>();
            container.RegisterType<IDepartmentRepository, DepartmentRepository>();
        }

        private static void RegisterAPIDI(IUnityContainer container)
        {
            container.RegisterType<IAPITodolistRequestRepository, APITodolistRequestRepository>();

            container.RegisterType<IAPITodolistRequestService, APITodolistRequestService>();
        }
    }
}