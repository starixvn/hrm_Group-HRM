namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Hangfire;

    public class KeepInViewRequestService : IKeepInViewRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/KeepInView.Request.cshtml";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/KeepInView.Cancel.cshtml";
        private const string EmailTemplatePathForApproval = "~/app_data/emails/KeepInView.Approval.cshtml";
        private readonly IKeepInViewRequestRepository keepInViewRequestRepository;
        private readonly IBaseService baseService;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;
        private readonly ILetstalkService letstalkService;

        public KeepInViewRequestService(
            IKeepInViewRequestRepository keepInViewRequestRepository,
            IBaseService baseService,
            IRequestMailService requestMailService,
            IEmailService emailService,
            ILetstalkService letstalkService)
        {
            this.keepInViewRequestRepository = keepInViewRequestRepository;
            this.baseService = baseService;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
            this.letstalkService = letstalkService;
        }

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
            KeepInViewRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.New);

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
                    ConfigSettings.ApplicationDomain);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                BackgroundJob.Enqueue(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailEditRequest(long requestId)
        {
            KeepInViewRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Edit);
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
                    ConfigSettings.ApplicationDomain);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                BackgroundJob.Enqueue(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailDeleteRequest(long requestId)
        {
            KeepInViewRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Cancel);
                email.Subject = string.Format(Messages.MessageCompensationLeaveRequest, request.Requester.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
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
            KeepInViewRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForApproval);
                byte action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                string commandMessage = requestMailService.GetCommandMessage(command);
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, action);
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

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                BackgroundJob.Enqueue(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }
    }
}