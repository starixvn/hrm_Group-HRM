using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Request.LateInEarlyOut;
using HRM.Models.Request;
using HRM.Repositories.Interfaces;
using HRM.Resources;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Request;
using Microsoft.AspNetCore.Hosting;

namespace HRM.Services.Implement.Request;

public class LateInEarlyOutRequestService(
    IWebHostEnvironment env,
    ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository,
    IRequestMailService requestMailService,
    IEmailService emailService,
    IBaseService baseService,
    IUserControlSession userControlSession,
    ICompanyService companyService) : ILateInEarlyOutRequestService
{
    private const string EmailTemplatePathForRequest = "EmailTemplates/LateInEarlyOut.Request.txt";
    private const string EmailTemplatePathForCancel = "EmailTemplates/LateInEarlyOut.Cancel.txt";
    private const string EmailTemplatePathForApproval = "EmailTemplates/LateInEarlyOut.Approval.txt";

    public async Task<IList<LateInEarlyOutRequest>> GetByDateAsync(GetLateInEarlyOutRequestByDateCriteria criteria)
    {
        IList<LateInEarlyOutRequest> requests = null;
        if (criteria != null && criteria.IsValid())
        {
            requests = await lateInEarlyOutRequestRepository.GetByDateAsync(criteria);
        }

        return requests ?? new List<LateInEarlyOutRequest>();
    }

    public async Task<LateInEarlyOutRequest> GetByIdAsync(long requestId)
    {
        var criteria = new GetLateInEarlyOutRequestByIdCriteria
        {
            RequestId = requestId
        };
        return await lateInEarlyOutRequestRepository.GetByIdAsync(criteria);
    }

    public async Task<IList<LateInEarlyOutRequest>> GetHistoryByIdAsync(long requestId)
    {
        var companyId = userControlSession.Get<int>(SessionKey.CurrentCompanyId);

        var criteria = new GetLateInEarlyOutRequestHistoryByIdCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        return await lateInEarlyOutRequestRepository.GetHistoryByIdAsync(criteria);
    }

    public async Task<EmployeeLateInEarlyOutModel> GetActualLateInEarlyOutAsync(GetActualLateInEarlyOutCriteria criteria)
    {
        return await baseService.GetDataEntityAsync<EmployeeLateInEarlyOutModel>(criteria);
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
            RequestTypeId = param.RequestTypeId,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<DateTime>(criteria);
    }

    public async Task<CommandResult> DeleteAsync(long requestId)
    {
        var criteria = new DeleteLateInEarlyOutRequestCriteria
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
            if (request.Approver == null)
            {
                return;
            }

            var domain = await companyService.GetDomain(request.Approver.CompanyId);
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.New);
            email.Subject =
                $"{Messages.LateInEarlyOutEmailSubject} {request.Requester.Nickname} on {request.OnDate.ToDateMonthYearFormatedDate()}";

            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.Nickname,
                request.RequestTypeId == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                request.OnDate.ToDateMonthYearFormatedDate(),
                request.RequestTypeId == (short)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins,
                request.Remark,
                domain);

            await emailService.SendEmailAsync(
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body,
                request.Approver.CompanyId);
        }
    }

    public async Task SendMailEditRequestAsync(long requestId)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            if (request.Approver == null)
            {
                return;
            }

            var domain = await companyService.GetDomain(request.Approver.CompanyId);
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.Edit);
            email.Subject =
                $"{Messages.UpdatedLateInEarlyOutEmailSubject} {request.Requester.Nickname} on {request.OnDate.ToDateMonthYearFormatedDate()}";

            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.Nickname,
                request.RequestTypeId == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                request.OnDate.ToDateMonthYearFormatedDate(),
                request.RequestTypeId == (short)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins,
                request.Remark,
                domain);

            await emailService.SendEmailAsync(
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body,
                request.Approver.CompanyId);
        }
    }

    public async Task SendMailDeleteRequestAsync(long requestId)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForCancel);
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.Cancel);
            email.Subject = string.Format(Messages.MessageLateInEarlyOutRequest, request.RequestTypeName, request.Requester.Nickname);
            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.DisplayName,
                request.RequestTypeName,
                request.OnDate.ToDateMonthYearFormatedDate(),
                request.RequestTypeId == (short)RequestType.LateIn ? "+" + request.LateInMins : "-" + request.EarlyOutMins,
                request.Remark);

            await emailService.SendEmailAsync(
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
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
                request.RequestTypeId == (byte)RequestType.LateIn ? "+" + request.LateInMins : "-" + request.EarlyOutMins,
                request.Remark);

            await emailService.SendEmailAsync(
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body,
                companyId: request.Requester.CompanyId);
        }
    }
}