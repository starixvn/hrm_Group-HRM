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
    using HRM.Criteria.Request.LateInEarlyOut;
    using HRM.Models.Request;
    using HRM.Repositories;
    using HRM.Resources;
    using Microsoft.AspNetCore.Hosting;

    public class LateInEarlyOutRequestService(
        AppSettings appSettings,
        IWebHostEnvironment env,
        ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository,
        IRequestMailService requestMailService,
        IEmailService emailService,
        IBaseService baseService,
        ILetstalkService letstalkService) : ILateInEarlyOutRequestService
    {
        private const string EmailTemplatePathForRequest = "EmailTemplates/LateInEarlyOut.Request.txt";
        private const string EmailTemplatePathForCancel = "EmailTemplates/LateInEarlyOut.Cancel.txt";
        private const string EmailTemplatePathForApproval = "EmailTemplates/LateInEarlyOut.Approval.txt";

        public IList<LateInEarlyOutRequest> GetByDate(GetLateInEarlyOutRequestByDateCriteria criteria)
        {
            IList<LateInEarlyOutRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = lateInEarlyOutRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<LateInEarlyOutRequest>();
        }

        public LateInEarlyOutRequest GetById(long requestId)
        {
            var criteria = new GetLateInEarlyOutRequestByIdCriteria
            {
                RequestId = requestId
            };
            return lateInEarlyOutRequestRepository.GetById(criteria);
        }

        public IList<LateInEarlyOutRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetLateInEarlyOutRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return lateInEarlyOutRequestRepository.GetHistoryById(criteria);
        }

        public EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria)
        {
            return baseService.GetDataEntity<EmployeeLateInEarlyOutModel>(criteria);
        }

        public IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param)
        {
            var criteria = new GetDisabledDateCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                RequestId = param.RequestId,
                RequestTypeId = param.RequestTypeId,
            };
            return baseService.GetData<DateTime>(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteLateInEarlyOutRequestCriteria
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
                    Messages.LateInEarlyOutEmailSubject,
                    request.Requester.Nickname,
                    request.OnDate.ToDateMonthYearFormatedDate());

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.Nickname,
                    request.RequestTypeId == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (short)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins,
                    request.Remark,
                    appSettings.Domain);

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
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
                    Messages.UpdatedLateInEarlyOutEmailSubject,
                    request.Requester.Nickname,
                    request.OnDate.ToDateMonthYearFormatedDate());

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.Nickname,
                    request.RequestTypeId == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (short)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins,
                    request.Remark,
                    appSettings.Domain);

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailDeleteRequest(long requestId)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForCancel);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Cancel);
                email.Subject = string.Format(Messages.MessageLateInEarlyOutRequest, request.RequestTypeName, request.Requester.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    request.RequestTypeName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (short)RequestType.LateIn ? ("+" + request.LateInMins) : ("-" + request.EarlyOutMins),
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
                var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForApproval);
                var action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                var commandMessage = requestMailService.GetCommandMessage(command);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, action);

                email.Subject = request.RequestTypeId == (byte)RequestType.LateIn
                    ? string.Format(Messages.ApprovalNotificationLateInSubject, request.Requester.Nickname, commandMessage, actor.Nickname)
                    : string.Format(Messages.ApprovalNotificationEarlyOutSubject, request.Requester.Nickname, commandMessage, actor.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    commandMessage,
                    actor.DisplayName,
                    request.RequestTypeName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (byte)RequestType.LateIn ? ("+" + request.LateInMins) : ("-" + request.EarlyOutMins),
                    request.Remark);

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }
    }
}