namespace HRM.Services.Implement.Request;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Enums;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Request.KeepInView;
using HRM.Models.Request;
using HRM.Repositories.Interfaces;
using HRM.Services.Interfaces.Request;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Resources;

public class KeepInViewRequestService(
    IWebHostEnvironment env,
    IKeepInViewRequestRepository keepInViewRequestRepository,
    IBaseService baseService,
    IRequestMailService requestMailService,
    IEmailService emailService,
    ILetstalkService letstalkService,
    IUserControlSession userControlSession) : IKeepInViewRequestService
{
    private const string EmailTemplatePathForRequest = "EmailTemplates/KeepInView.Request.txt";
    private const string EmailTemplatePathForCancel = "EmailTemplates/KeepInView.Cancel.txt";
    private const string EmailTemplatePathForApproval = "EmailTemplates/KeepInView.Approval.txt";

    public async Task<IList<KeepInViewRequest>> GetByDateAsync(GetKeepInViewRequestByDateCriteria criteria)
    {
        IList<KeepInViewRequest> requests = null;
        if (criteria != null && criteria.IsValid())
        {
            requests = await keepInViewRequestRepository.GetByDateAsync(criteria);
        }

        return requests ?? new List<KeepInViewRequest>();
    }

    public async Task<KeepInViewRequest> GetByIdAsync(long requestId)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetKeepInViewRequestByIdCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        return await keepInViewRequestRepository.GetByIdAsync(criteria);
    }

    public async Task<IList<KeepInViewRequest>> GetHistoryByIdAsync(long requestId)
    {
        var companyId = userControlSession.Get<int>(SessionKey.CurrentCompanyId);

        var criteria = new GetKeepInViewRequestHistoryByIdCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        return await keepInViewRequestRepository.GetHistoryByIdAsync(criteria);
    }

    public async Task<IEnumerable<DateTime>> GetDisabledDateAsync(GetDisabledDateParam param)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetDisabledDateCriteria
        {
            EmployeeId = param.EmployeeId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            RequestId = param.RequestId,
            TypeId = param.RequestTypeId,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<DateTime>(criteria);
    }

    public async Task<CommandResult> DeleteAsync(long requestId)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new DeleteKeepInViewRequestCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task SendMailAndSendLetstalkCreateRequestAsync(long requestId)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.New);

            email.Subject =
                $"{Messages.InsertedCompensationLeaveEmailSubject} {request.Requester.Nickname} on {request.OnDate.ToDateMonthYearFormatedDate()}";

            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.Nickname,
                request.OnDate.ToDateMonthYearFormatedDate(),
                request.Remark,
                userControlSession.Get<string>(SessionKey.CurrentDomain));

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
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.Edit);
            email.Subject =
                $"{Messages.UpdatedCompensationLeaveEmailSubject} {request.Requester.Nickname} on {request.OnDate.ToDateMonthYearFormatedDate()}";

            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.Nickname,
                request.OnDate.ToDateMonthYearFormatedDate(),
                request.Remark,
                userControlSession.Get<string>(SessionKey.CurrentDomain));

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
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForCancel);
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.Cancel);
            email.Subject = string.Format(Messages.MessageCompensationLeaveRequest, request.Requester.Nickname);
            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.DisplayName,
                request.OnDate.ToDateMonthYearFormatedDate(),
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
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, action);
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

            await emailService.SendEmailAsync(
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);

            await letstalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body);
        }
    }
}