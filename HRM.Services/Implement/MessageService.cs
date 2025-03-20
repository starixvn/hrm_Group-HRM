using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HRM.Core;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Message;
using HRM.Models.Message;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace HRM.Services.Implement;

public class MessageService(
    IWebHostEnvironment env,
    IBaseService baseService,
    IEmailService emailService) : IMessageService
{
    private const string MessageEmailTemplate = "EmailTemplates/MessageNotification.txt";

    public async Task<IEnumerable<Message>> GetMessageByCreator(GetMessageByCreatorParam param)
    {
        var criteria = new GetMessageByCreatorCriteria
        {
            FromDate = param.FromDate?.ToServerTimeZone(),
            ToDate = param.ToDate?.ToEndOfDay().ToServerTimeZone(),
            IsDisabled = param.IsDisabled,
            CreatedBy = param.CreatedBy
        };
        return await baseService.GetDataAsync<Message>(criteria);
    }

    public async Task<Message> GetMessageById(int id)
    {
        var criteria = new GetMessageByIdCriteria { MessageId = id };
        return await baseService.GetDataEntityAsync<Message>(criteria);
    }

    public async Task<CommandResult> Create(int actorId, Message model, short companyId)
    {
        var criteria = new CreateMessageCriteria
        {
            Subject = model.Subject,
            Contents = model.Contents,
            Category = model.Category,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            EmployeeIds = model.EmployeeIds,
            TeamIds = model.TeamIds,
            CreatedBy = actorId     ,
            CompanyId = companyId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Edit(int actorId, Message model)
    {
        var criteria = new EditMessageCriteria
        {
            MessageId = model.MessageId,
            Subject = model.Subject,
            Contents = model.Contents,
            Category = model.Category,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            EmployeeIds = model.EmployeeIds,
            TeamIds = model.TeamIds,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Delete(int actorId, int id)
    {
        var criteria = new DeleteMessageCriteria
        {
            MessageId = id,
            ModifiedBy = actorId,
            ModifiedDate = DateTimeOffset.Now.DateTime
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditStatus(int employeeId, int messageId)
    {
        var criteria = new EditMessageStatusCriteria
        {
            MessageId = messageId,
            EmployeeId = employeeId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteMessageByEmployee(int employeeId, int messageId)
    {
        var criteria = new DeleteMessageByEmployeeCriteria
        {
            MessageId = messageId,
            EmployeeId = employeeId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IEnumerable<Message>> GetMessageByEmployee(GetMessageByEmployeeParam param)
    {
        var criteria = new GetMessageByEmployeeCriteria
        {
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            IsDisabled = param.IsDisabled,
            EmployeeId = param.EmployeeId
        };
        return await baseService.GetDataAsync<Message>(criteria);
    }

    public async Task<Message> GetDetailMessageByEmployee(int messageId)
    {
        var criteria = new GetMessageOfEmployeeByIdCriteria
        {
            MessageId = messageId
        };
        return await baseService.GetDataEntityAsync<Message>(criteria);
    }

    public async Task<IEnumerable<Message>> GetUnreadMessageOfEmployeeAsync(int employeeId)
    {
        var criteria = new GetUnreadMessageOfEmployeeCriteria
        {
            EmployeeId = employeeId
        };
        return await baseService.GetDataAsync<Message>(criteria);
    }

    public async Task SendMailToStaff(Message model)
    {
        var bodyTemplate = Utilities.ReadTextFile(Path.Combine(env.ContentRootPath, MessageEmailTemplate));
        await Task.Run(() => SendEmail(model.Subject, model.Contents, model.SendToEmail, bodyTemplate));
    }

    public async Task SendEmail(string subject, string content, string sendToEmail, string emailTemplatePath)
    {
        var body = string.Format(emailTemplatePath, subject, content);

        await emailService.SendEmailAsync(
            sendToEmail,
            string.Empty,
            Resources.Messages.MessageSubject + " " + subject,
            body);
    }
}