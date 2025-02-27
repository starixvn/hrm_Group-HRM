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
    using HRM.Criteria.Request.KeepInView;
    using HRM.Models.Request;
    using HRM.Repositories;
    using HRM.Resources;
    using HRM.Services.Interfaces;
    using Microsoft.AspNetCore.Hosting;

    public class KeepInViewRequestService(
        AppSettings appSettings,
        IWebHostEnvironment env,
        IKeepInViewRequestRepository keepInViewRequestRepository,
        IBaseService baseService,
        IRequestMailService requestMailService,
        IEmailService emailService,
        ILetstalkService letstalkService) : IKeepInViewRequestService
    {
        private const string EmailTemplatePathForRequest = "EmailTemplates/KeepInView.Request.txt";
        private const string EmailTemplatePathForCancel = "EmailTemplates/KeepInView.Cancel.txt";
        private const string EmailTemplatePathForApproval = "EmailTemplates/KeepInView.Approval.txt";

        public IList<KeepInViewRequest> GetByDate(GetKeepInViewRequestByDateCriteria criteria)
        {
            IList<KeepInViewRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = keepInViewRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<KeepInViewRequest>();
        }

        public KeepInViewRequest GetById(long requestId)
        {
            var criteria = new GetKeepInViewRequestByIdCriteria
            {
                RequestId = requestId
            };
            return keepInViewRequestRepository.GetById(criteria);
        }

        public IList<KeepInViewRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetKeepInViewRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return keepInViewRequestRepository.GetHistoryById(criteria);
        }

        public IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param)
        {
            var criteria = new GetDisabledDateCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                RequestId = param.RequestId,
                TypeId = param.RequestTypeId
            };
            return baseService.GetData<DateTime>(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteKeepInViewRequestCriteria
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
                    Messages.InsertedCompensationLeaveEmailSubject,
                    request.Requester.Nickname,
                    request.OnDate.ToDateMonthYearFormatedDate());

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.Nickname,
                    request.OnDate.ToDateMonthYearFormatedDate(),
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
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Edit);
                email.Subject = string.Format(
                    "{0} {1} on {2}",
                    Messages.UpdatedCompensationLeaveEmailSubject,
                    request.Requester.Nickname,
                    request.OnDate.ToDateMonthYearFormatedDate());

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.Nickname,
                    request.OnDate.ToDateMonthYearFormatedDate(),
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
                var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForCancel);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Cancel);
                email.Subject = string.Format(Messages.MessageCompensationLeaveRequest, request.Requester.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
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
                email.Subject = string.Format(Messages.ApprovalNotificationCompensationLeaveSubject, request.Requester.Nickname, commandMessage, actor.Nickname);
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