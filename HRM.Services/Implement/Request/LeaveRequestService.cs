namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
    using Microsoft.AspNetCore.Hosting;

    public class LeaveRequestService(
        AppSettings appSettings,
        IWebHostEnvironment env,
        ILeaveRequestRepository leaveRequestRepository,
        IRequestMailService requestMailService,
        IBaseService baseService,
        IEmailService emailService,
        ILetstalkService letstalkService) : ILeaveRequestService
    {
        private const string EmailTemplatePathForRequest = "EmailTemplates/Leave.Request.txt";
        private const string EmailTemplatePathForCancel = "EmailTemplates/Leave.Cancel.txt";
        private const string EmailTemplatePathForApproval = "EmailTemplates/Leave.Approval.txt";

        public async Task<IList<LeaveRequest>> GetByDateAsync(GetLeaveRequestByDateCriteria criteria)
        {
            IList<LeaveRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = await leaveRequestRepository.GetByDateAsync(criteria);
            }

            return requests ?? [];
        }

        public async Task<LeaveRequest> GetByIdAsync(long requestId)
        {
            var criteria = new GetLeaveRequestByIdCriteria
            {
                RequestId = requestId
            };
            return await leaveRequestRepository.GetByIdAsync(criteria);
        }

        public async Task<IList<LeaveRequestDetail>> GetLeaveRequestDetailByIdAsync(long requestId)
        {
            var criteria = new GetLeaveRequestDetailByIdCriteria
            {
                RequestId = requestId
            };
            return (await baseService.GetDataAsync<LeaveRequestDetail>(criteria)).ToList();
        }

        public async Task<IEnumerable<LeaveRequest>> GetHistoryByIdAsync(long requestId)
        {
            var criteria = new GetLeaveRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return await leaveRequestRepository.GetHistoryByIdAsync(criteria);
        }

        public async Task<IEnumerable<DateTime>> GetDisabledDateAsync(GetDisabledDateParam param)
        {
            var criteria = new GetDisabledDateCriteria
            {
                LeaveTypeId = param.LeaveTypeId,
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                RequestId = param.RequestId
            };
            return await baseService.GetDataAsync<DateTime>(criteria);
        }

        public async Task<IEnumerable<Models.ScheduleArrange.DailySchedule>> GetEmployeeCalendarAsync(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetEmployeeCalendarCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return await leaveRequestRepository.GetEmployeeCalendarAsync(criteria);
        }

        public async Task<EmployeeLeaveBalance> GetLeaveBalanceAsync(GetLeaveBalanceCriteria criteria)
        {
            var leaveBalance = await baseService.GetDataEntityAsync<EmployeeLeaveBalance>(criteria);
            leaveBalance.KeepInViewByHourBalance = Math.Round(leaveBalance.KeepInViewByHourBalance / Constants.TotalMinuteInHour, 1);
            return leaveBalance;
        }

        public async Task<CommandResult> DeleteAsync(long requestId)
        {
            var criteria = new DeleteLeaveRequestCriteria
            {
                RequestId = requestId
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public void SendMailAndSendLetstalkCreateRequest(InsertLeaveRequestCriteria criteria)
        {
            var templateEmailPath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
            Task.Run(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailEditRequest(UpdateLeaveRequestCriteria criteria)
        {
            var templateEmailPath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
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

        public async Task SendMailDeleteRequest(long requestId)
        {
            var request = await GetByIdAsync(requestId);
            if (request != null)
            {
                var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForCancel);
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

        public async Task SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command)
        {
            var request = await GetByIdAsync(requestId);
            if (request != null)
            {
                var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForApproval);
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