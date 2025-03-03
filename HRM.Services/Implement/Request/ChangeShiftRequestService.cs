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
using HRM.Core.XMLExtensions;
using HRM.Criteria.Request.ChangeShift;
using HRM.Models.Request.ChangeShift;
using HRM.Models.ScheduleArrange;
using HRM.Repositories;
using HRM.Resources;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Request;
using Microsoft.AspNetCore.Hosting;

namespace HRM.Services.Implement.Request;

public class ChangeShiftRequestService(
    AppSettings appSettings,
    IWebHostEnvironment env,
    IChangeShiftRequestRepository changeShiftRepository,
    IBaseService baseService,
    IEmailService emailService,
    IRequestMailService requestMailService,
    ILetstalkService letStalkService)
    : IChangeShiftRequestService
{
    private const string EmailTemplatePathForRequest = "EmailTemplates/ChangeShift.Request.txt";
    private const string EmailTemplatePathForCancel = "EmailTemplates/ChangeShift.Cancel.txt";
    private const string EmailTemplatePathForApproval = "EmailTemplates/ChangeShift.Approval.txt";

    public async Task<int> GetNumberOfRequestAsync(int employeeId, DateTime fromDate, DateTime toDate)
    {
        var criteria = new GetNumberOfRequestCriteria
        {
            EmployeeId = employeeId,
            FromDate = fromDate,
            ToDate = toDate
        };
        return await baseService.GetDataEntityAsync<int>(criteria);
    }

    public async Task<IEnumerable<DailySchedule>> GetWorkingCalendarForRequestAsync(GetWorkingCalendarForRequestParam param)
    {
        var criteria = new GetWorkingCalendarForRequestCriteria
        {
            EmployeeId = param.EmployeeId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            TypeId = param.TypeId,
            RequestId = param.RequestId
        };
        return await changeShiftRepository.GetWorkingCalendarForRequestAsync(criteria);
    }

    public async Task<IEnumerable<DateTime>> GetDisableDateForMoveOffDayRequestAsync(int employeeId, DateTime requestDate, long? requestId = null)
    {
        var fromDate = new DateTimeOffset(requestDate.Year, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var criteria = new GetDisableDateForMoveOffDayRequestCriteria
        {
            EmployeeId = employeeId,
            FromDate = fromDate,
            ToDate = fromDate.AddYears(1).AddDays(-1),
            RequestId = requestId
        };
        var dates = (await baseService.GetDataAsync<DateTime>(criteria)).ToList();
        for (var day = requestDate.FirstDayOfWeek().Date; day.Date <= requestDate.LastDayOfWeek().Date; day = day.AddDays(1))
        {
            if (dates.All(x => x.Date != day.Date))
            {
                dates.Add(day);
            }
        }

        return dates;
    }

    public async Task<IEnumerable<ChangeShiftRequest>> GetByEmployeeAsync(int employeeId, DateTime fromDate, DateTime toDate)
    {
        var criteria = new GetChangeShiftRequestByEmployeeCriteria
        {
            EmployeeId = employeeId,
            FromDate = fromDate,
            ToDate = toDate
        };
        return await changeShiftRepository.GetByEmployeeAsync(criteria);
    }

    public async Task<IEnumerable<ChangeShiftRequest>> GetHistoryByIdAsync(long requestId)
    {
        var criteria = new GetChangeShiftRequestHistoryByIdCriteria
        {
            RequestId = requestId
        };
        return await changeShiftRepository.GetHistoryByIdAsync(criteria);
    }

    public async Task<ChangeShiftRequest> GetByIdAsync(long requestId)
    {
        var criteria = new GetChangeShiftRequestByIdCriteria
        {
            RequestId = requestId
        };
        return await changeShiftRepository.GetByIdAsync(criteria);
    }

    public async Task<IEnumerable<ChangeShiftDetail>> GetDetailByIdAsync(long requestId)
    {
        var criteria = new GetChangeShiftRequestDetailByIdCriteria
        {
            RequestId = requestId
        };
        var details = await baseService.GetDataAsync<ChangeShiftDetail>(criteria);
        foreach (var detail in details)
        {
            if (detail.FromShiftId == Constants.OffDayShiftId)
            {
                detail.FromShift = Labels.Off;
            }

            if (detail.ToShiftId == Constants.OffDayShiftId)
            {
                detail.ToShift = Labels.Off;
            }
        }

        return details;
    }

    public async Task<CommandResult> CreateAsync(int actorId, CreateChangeShift model)
    {
        var criteria = new CreateChangeShiftRequestCriteria
        {
            EmployeeId = model.EmployeeId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TypeId = model.TypeId,
            VerifierId = model.VerifierId,
            ApproverId = model.ApproverId,
            Remark = model.Remark,
            DetailXml = model.Details.ToXML().ToString(),
            ActorId = actorId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        commandResult.OutputId = criteria.RequestId;

        return commandResult;
    }

    public async Task<CommandResult> EditAsync(int actorId, EditChangeShift model)
    {
        var criteria = new EditChangeShiftRequestCriteria
        {
            RequestId = model.RequestId,
            EmployeeId = model.EmployeeId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TypeId = model.TypeId,
            VerifierId = model.VerifierId,
            ApproverId = model.ApproverId,
            Remark = model.Remark,
            DetailXml = model.Details.ToXML().ToString(),
            ActorId = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteAsync(int actorId, long requestId)
    {
        var criteria = new DeleteChangeShiftRequestCriteria
        {
            RequestId = requestId,
            ActorId = actorId
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
            var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
            email.Subject = $"{Messages.ChangeShiftEmailSubject} {request.Requester.DisplayName} {forDate}";

            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.Nickname,
                forDate,
                request.Remark,
                appSettings.Domain);

            await Task.WhenAll(
                emailService.SendEmailAsync(email.SendTo, email.SendCC, email.Subject, email.Body),
                letStalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body)
            );
        }
    }

    public async Task SendMailEditRequestAsync(long requestId)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForRequest);
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.Edit);
            var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
            email.Subject = $"{Messages.UpdatedChangeShiftEmailSubject} {request.Requester.DisplayName} {forDate}";

            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.Nickname,
                forDate,
                request.Remark,
                appSettings.Domain);

            await Task.WhenAll(
                emailService.SendEmailAsync(email.SendTo, email.SendCC, email.Subject, email.Body),
                letStalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body)
            );
        }
    }

    public async Task SendMailDeleteRequestAsync(long requestId)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            var emailTemplatePath = Path.Combine(env.ContentRootPath, EmailTemplatePathForCancel);
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.Cancel);
            var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);

            email.Subject = string.Format(Messages.MessageChangeShiftRequest, request.Requester.Nickname);
            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.DisplayName,
                forDate,
                request.Remark);

            await Task.WhenAll(
                emailService.SendEmailAsync(email.SendTo, email.SendCC, email.Subject, email.Body),
                letStalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body)
            );
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
            var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);

            email.Subject = string.Format(Messages.ApprovalNotificationChangeShiftSubject, request.Requester.Nickname, commandMessage, actor.Nickname);
            email.Body = Utilities.ReadTextFile(emailTemplatePath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.DisplayName,
                commandMessage,
                actor.DisplayName,
                Labels.ChangeShift,
                forDate,
                request.Remark);

            await Task.WhenAll(
                emailService.SendEmailAsync(email.SendTo, email.SendCC, email.Subject, email.Body),
                letStalkService.SendNotificationAsync(email.LetsTalk, email.Subject, email.Body)
            );
        }
    }
}