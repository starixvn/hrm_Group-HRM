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

    public class CheckInOutRequestService : ICheckInOutRequestService
    {
        private const string EmailTemplatePathForRequest = "EmailTemplates/CheckInOut.Request.txt";
        private const string EmailTemplatePathForCancel = "EmailTemplates/CheckInOut.Cancel.txt";
        private const string EmailTemplatePathForApproval = "EmailTemplates/CheckInOut.Approval.txt";

        private readonly AppSettings appSettings;
        private readonly IWebHostEnvironment env;
        private readonly ICheckInOutRequestRepository checkInOutRequestRepository;
        private readonly IBaseService baseService;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;
        private readonly ILetstalkService letstalkService;

        public CheckInOutRequestService(
            AppSettings appSettings,
            IWebHostEnvironment env,
            ICheckInOutRequestRepository checkInOutRequestRepository,
            IBaseService baseService,
            IRequestMailService requestMailService,
            IEmailService emailService,
            ILetstalkService letstalkService)
        {
            this.appSettings = appSettings;
            this.env = env;
            this.checkInOutRequestRepository = checkInOutRequestRepository;
            this.baseService = baseService;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
            this.letstalkService = letstalkService;
        }

        public async Task<IList<CheckInOutRequest>> GetByDateAsync(GetCheckInOutRequestByDateCriteria criteria)
        {
            IList<CheckInOutRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = await checkInOutRequestRepository.GetByDateAsync(criteria);
            }

            return requests ?? new List<CheckInOutRequest>();
        }

        public async Task<CheckInOutRequest> GetByIdAsync(long requestId)
        {
            var criteria = new GetCheckInOutRequestByIdCriteria
            {
                RequestId = requestId
            };
            return await checkInOutRequestRepository.GetByIdAsync(criteria);
        }

        public async Task<IList<CheckInOutRequest>> GetHistoryByIdAsync(long requestId)
        {
            var criteria = new GetCheckInOutRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return await checkInOutRequestRepository.GetHistoryByIdAsync(criteria);
        }

        public async Task<IEnumerable<DateTime>> GetDisabledDateAsync(GetDisabledDateParam param)
        {
            var criteria = new GetDisabledDateCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                RequestId = param.RequestId,
                RequestTypeId = param.RequestTypeId
            };
            return await baseService.GetDataAsync<DateTime>(criteria);
        }

        public async Task<CommandResult> DeleteAsync(long requestId)
        {
            var criteria = new DeleteCheckInOutRequestCriteria
            {
                RequestId = requestId
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task SendMailAndSendLetstalkCreateRequestAsync(long requestId)
        {
            var request = await GetByIdAsync(requestId);
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

                await emailService.SendEmailAsync(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body);

                await letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body);
            }
        }

        public async Task SendMailEditRequestAsync(long requestId)
        {
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
            var request = await GetByIdAsync(requestId);
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

                await emailService.SendEmailAsync(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body);

                await letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body);
            }
        }

        public async Task SendMailDeleteRequestAsync(long requestId)
        {
            var request = await GetByIdAsync(requestId);
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

                await emailService.SendEmailAsync(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body);

                await letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body);
            }
        }

        public async Task SendMailProcessRequestAsync(long requestId, EmployeeSession actor, RequestCommand command)
        {
            var request = await GetByIdAsync(requestId);
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

                await emailService.SendEmailAsync(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body);

                await letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body);
            }
        }
    }
}