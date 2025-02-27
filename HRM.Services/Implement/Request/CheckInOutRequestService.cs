namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using HRM.Core;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Request.CheckInOut;
    using HRM.Models.Request;
    using HRM.Repositories;
    using HRM.Resources;
    using Microsoft.AspNetCore.Hosting;

    public class CheckInOutRequestService(
        AppSettings appSettings,
        IWebHostEnvironment env,
        ICheckInOutRequestRepository checkInOutRequestRepository,
        IBaseService baseService,
        IRequestMailService requestMailService,
        IEmailService emailService,
        ILetstalkService letstalkService) : ICheckInOutRequestService
    {
        private const string EmailTemplatePathForRequest = "EmailTemplates/CheckInOut.Request.txt";
        private const string EmailTemplatePathForCancel = "EmailTemplates/CheckInOut.Cancel.txt";
        private const string EmailTemplatePathForApproval = "EmailTemplates/CheckInOut.Approval.txt";

        public IList<CheckInOutRequest> GetByDate(GetCheckInOutRequestByDateCriteria criteria)
        {
            IList<CheckInOutRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = checkInOutRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<CheckInOutRequest>();
        }

        public CheckInOutRequest GetById(long requestId)
        {
            var criteria = new GetCheckInOutRequestByIdCriteria
            {
                RequestId = requestId
            };
            return checkInOutRequestRepository.GetById(criteria);
        }

        public IList<CheckInOutRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetCheckInOutRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return checkInOutRequestRepository.GetHistoryById(criteria);
        }

        public IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param)
        {
            var criteria = new GetDisabledDateCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                RequestId = param.RequestId,
                RequestTypeId = param.RequestTypeId
            };
            return baseService.GetData<DateTime>(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteCheckInOutRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailAndSendLetstalkCreateRequest(long requestId)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.New);
                email.Subject = string.Format(
                    "{0} {1} on {2}",
                    Messages.CheckInOutEmailSubject,
                    request.Requester.Nickname,
                    request.OnDate.ToDateMonthYearFormatedDate());

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.Nickname,
                    request.RequestTypeId == (byte)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                        request.OnDate.ToDateMonthYearFormatedDate(),
                        request.ShiftName,
                        request.RequestTime.ToShortTime(),
                        request.Remark,
                        appSettings.Domain);

                Task.Run(() => emailService.SendEmailAsync(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailEditRequest(long requestId)
        {
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
            var request = GetById(requestId);
            if (request != null)
            {
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Edit);
                email.Subject = string.Format(
                    "{0} {1} on {2}",
                    Messages.UpdatedCheckInOutEmailSubject,
                    request.Requester.Nickname,
                    request.OnDate.ToDateMonthYearFormatedDate());

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.Nickname,
                    request.RequestTypeId == (byte)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                        request.OnDate.ToDateMonthYearFormatedDate(),
                        request.ShiftName,
                        request.RequestTime.ToShortTime(),
                        request.Remark,
                        appSettings.Domain);

                Task.Run(() => emailService.SendEmailAsync(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailDeleteRequest(long requestId)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var templateEmailPath = Path.Combine(env.ContentRootPath, EmailTemplatePathForCancel);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Cancel);
                email.Subject = string.Format(Messages.MessageCheckInOutRequest, request.RequestTypeName, request.Requester.Nickname);
                email.Body = Utilities.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    request.RequestTypeName.ToLowerInvariant(),
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.ShiftName,
                    request.RequestTime.ToShortTime(),
                    request.Remark);

                Task.Run(() => emailService.SendEmailAsync(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForApproval);
                var action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                var commandMessage = requestMailService.GetCommandMessage(command);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, action);

                email.Subject = request.RequestTypeId == (byte)RequestType.CheckIn
                    ? string.Format(Messages.ApprovalNotificationCheckInSubject, request.Requester.Nickname, commandMessage, actor.Nickname)
                    : string.Format(Messages.ApprovalNotificationCheckOutSubject, request.Requester.Nickname, commandMessage, actor.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    commandMessage,
                    actor.DisplayName,
                    request.RequestTypeName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.ShiftName,
                    request.RequestTime.ToShortTime(),
                    request.Remark);

                Task.Run(() => emailService.SendEmailAsync(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body));
            }
        }
    }
}