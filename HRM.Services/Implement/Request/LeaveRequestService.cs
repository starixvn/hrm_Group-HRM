namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Request;
    using HRM.Criteria.Request.Leave;
    using HRM.Models;
    using HRM.Models.Request;
    using HRM.Repositories;
    using HRM.Resources;
    using HRM.Services.Interfaces;
    using Hangfire;

    public class LeaveRequestService : ILeaveRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/Leave.Request.cshtml";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/Leave.Cancel.cshtml";
        private const string EmailTemplatePathForApproval = "~/app_data/emails/Leave.Approval.cshtml";
        private readonly ILeaveRequestRepository leaveRequestRepository;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;
        private readonly IBaseService baseService;
        private readonly ILetstalkService letstalkService;

        public LeaveRequestService(
            ILeaveRequestRepository leaveRequestRepository,
            IRequestMailService requestMailService,
            IBaseService baseService,
            IEmailService emailService,
            ILetstalkService letstalkService)
        {
            this.leaveRequestRepository = leaveRequestRepository;
            this.requestMailService = requestMailService;
            this.baseService = baseService;
            this.emailService = emailService;
            this.letstalkService = letstalkService;
        }

        public IList<LeaveRequest> GetByDate(GetLeaveRequestByDateCriteria criteria)
        {
            IList<LeaveRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = leaveRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<LeaveRequest>();
        }

        public LeaveRequest GetById(long requestId)
        {
            var criteria = new GetLeaveRequestByIdCriteria
            {
                RequestId = requestId
            };
            return leaveRequestRepository.GetById(criteria);
        }

        public IList<LeaveRequestDetail> GetLeaveRequestDetailById(long requestId)
        {
            var criteria = new GetLeaveRequestDetailByIdCriteria
            {
                RequestId = requestId
            };
            return baseService.GetData<LeaveRequestDetail>(criteria).ToList();
        }

        public IEnumerable<LeaveRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetLeaveRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return leaveRequestRepository.GetHistoryById(criteria);
        }

        public IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param)
        {
            var criteria = new GetDisabledDateCriteria
            {
                LeaveTypeId = param.LeaveTypeId,
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                RequestId = param.RequestId
            };
            return baseService.GetData<DateTime>(criteria);
        }

        public IEnumerable<Models.ScheduleArrange.DailySchedule> GetEmployeeCalendar(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetEmployeeCalendarCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return leaveRequestRepository.GetEmployeeCalendar(criteria);
        }

        public EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria)
        {
            var leaveBalance = baseService.GetDataEntity<EmployeeLeaveBalance>(criteria);
            leaveBalance.KeepInViewByHourBalance = Math.Round(leaveBalance.KeepInViewByHourBalance / Constants.TotalMinuteInHour, 1);
            return leaveBalance;
        }
        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteLeaveRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailAndSendLetstalkCreateRequest(InsertLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailEditRequest(UpdateLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(criteria.RequestId, (int)SettingsSendMail.New);
            email.Subject = Messages.LeaveEmailSubject + " " + criteria.EmployeeName;
            SendMailForSuccessfully(criteria, templateEmailPath, email);
        }

        public void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(criteria.RequestId, (int)SettingsSendMail.Edit);
            email.Subject = Messages.UpdatedLeaveEmailSubject + " " + criteria.EmployeeName;

            SendMailForSuccessfully(criteria, templateEmailPath, email);
        }

        public void SendMailDeleteRequest(long requestId)
        {
            LeaveRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Cancel);
                var fromDate = request.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(request.LeaveDetail) : request.FromDate;
                var toDate = request.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(request.LeaveDetail) : request.ToDate;
                var forDate = requestMailService.GetStringDateForLeaveRequest(request.FromDate, fromDate, toDate, request.TotalDays);

                email.Subject = string.Format(Messages.MessageLeaveRequestCanceled, request.Requester.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    forDate,
                    request.LeaveType.Name,
                    request.Remark);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                BackgroundJob.Enqueue(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command)
        {
            LeaveRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForApproval);
                byte action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                string commandMessage = requestMailService.GetCommandMessage(command);
                var fromDate = request.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(request.LeaveDetail) : request.FromDate;
                var toDate = request.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(request.LeaveDetail) : request.ToDate;
                var forDate = requestMailService.GetStringDateForLeaveRequest(request.FromDate, fromDate, toDate, request.TotalDays);

                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, action);
                email.Subject = string.Format(Messages.ApprovalNotificationLeaveSubject,
                    request.Requester.Nickname,
                    commandMessage,
                    actor.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    commandMessage,
                    actor.DisplayName,
                    request.RequestTypeName,
                    forDate,
                    request.LeaveType.Name,
                    request.Remark);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                BackgroundJob.Enqueue(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public DateTime GetFromTimeHalfDay(IList<LeaveRequestDetail> leaveDetails)
        {
            var fromTime = DateTimeOffset.Now;
            var leaveHalfDay = leaveDetails.FirstOrDefault(t => t.WorkType == (byte)WorkType.Leave && t.IsHalfDay);
            if (leaveHalfDay != null)
            {
                fromTime = leaveHalfDay.FromTime;
            }

            return fromTime.DateTime;
        }

        public DateTime GetToTimeHalfDay(IList<LeaveRequestDetail> leaveDetails)
        {
            var toTime = DateTimeOffset.Now;
            var leaveHalfDay = leaveDetails.FirstOrDefault(t => t.WorkType == (byte)WorkType.Leave && t.IsHalfDay);
            if (leaveHalfDay != null)
            {
                toTime = leaveHalfDay.ToTime;
            }

            return toTime.DateTime;
        }

        private void SendMailForSuccessfully(BaseLeaveRequestCriteria criteria, string templateEmailPath, EmailModel email)
        {
            email.Subject += criteria.TotalDays > 1
                   ? " from " + criteria.FromDate.ToDateMonthYearFormatedDate() + " to " + criteria.ToDate.ToDateMonthYearFormatedDate()
                   : " on " + criteria.FromDate.ToDateMonthYearFormatedDate();
            var fromDate = criteria.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(criteria.RequestDetail) : criteria.FromDate;
            var toDate = criteria.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(criteria.RequestDetail) : criteria.ToDate;
            var forDate = requestMailService.GetStringDateForLeaveRequest(criteria.FromDate, fromDate, toDate, criteria.TotalDays);

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                criteria.EmployeeName,
                forDate,
                criteria.LeaveTypeName,
                criteria.Remark,
                ConfigSettings.ApplicationDomain);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body);
            }

            BackgroundJob.Enqueue(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
        }
    }
}