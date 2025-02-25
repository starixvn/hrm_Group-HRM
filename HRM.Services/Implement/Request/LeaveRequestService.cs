namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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

    public class LeaveRequestService(
        AppSettings appSettings,
        ILeaveRequestRepository leaveRequestRepository,
        IRequestMailService requestMailService,
        IBaseService baseService,
        IEmailService emailService,
        ILetstalkService letstalkService) : ILeaveRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/Leave.Request.txt";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/Leave.Cancel.txt";
        private const string EmailTemplatePathForApproval = "~/app_data/emails/Leave.Approval.txt";

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
            Task.Run(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailEditRequest(UpdateLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
            Task.Run(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria, string templateEmailPath)
        {
            var email = requestMailService.GetMailToMailCCAndLetstalk(criteria.RequestId, (int)SettingsSendMail.New);
            email.Subject = Messages.LeaveEmailSubject + " " + criteria.EmployeeName;
            SendMailForSuccessfully(criteria, templateEmailPath, email);
        }

        public void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria, string templateEmailPath)
        {
            var email = requestMailService.GetMailToMailCCAndLetstalk(criteria.RequestId, (int)SettingsSendMail.Edit);
            email.Subject = Messages.UpdatedLeaveEmailSubject + " " + criteria.EmployeeName;

            SendMailForSuccessfully(criteria, templateEmailPath, email);
        }

        public void SendMailDeleteRequest(long requestId)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Cancel);
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

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForApproval);
                var action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                var commandMessage = requestMailService.GetCommandMessage(command);
                var fromDate = request.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(request.LeaveDetail) : request.FromDate;
                var toDate = request.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(request.LeaveDetail) : request.ToDate;
                var forDate = requestMailService.GetStringDateForLeaveRequest(request.FromDate, fromDate, toDate, request.TotalDays);

                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, action);
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

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
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
                appSettings.Domain);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body);
            }

            Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
        }
    }
}