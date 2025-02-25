namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Hangfire;

    public class CheckInOutRequestService : ICheckInOutRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/CheckInOut.Request.cshtml";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/CheckInOut.Cancel.cshtml";
        private const string EmailTemplatePathForApproval = "~/app_data/emails/CheckInOut.Approval.cshtml";
        private readonly ICheckInOutRequestRepository checkInOutRequestRepository;
        private readonly IBaseService baseService;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;
        private readonly ILetstalkService letstalkService;

        public CheckInOutRequestService(
            ICheckInOutRequestRepository checkInOutRequestRepository,
            IBaseService baseService,
            IRequestMailService requestMailService,
            IEmailService emailService,
            ILetstalkService letstalkService)
        {
            this.checkInOutRequestRepository = checkInOutRequestRepository;
            this.baseService = baseService;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
            this.letstalkService = letstalkService;
        }

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
            CheckInOutRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.New);
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
            var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
            CheckInOutRequest request = GetById(requestId);
            if (request != null)
            {
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Edit);
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
            CheckInOutRequest request = GetById(requestId);
            if (request != null)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Cancel);
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
            CheckInOutRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForApproval);
                byte action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                string commandMessage = requestMailService.GetCommandMessage(command);
                EmailModel email = requestMailService.GetMailToMailCCAndLetstalk(requestId, action);

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