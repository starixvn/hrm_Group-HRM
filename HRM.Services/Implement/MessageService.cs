namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using HRM.Core;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Message;
    using HRM.Models.Message;
    using Hangfire;

    public class MessageService : IMessageService
    {
        private readonly string messageEmailTemplate = "~/app_data/emails/MessageNotification.cshtml";

        private readonly IBaseService baseService;
        private readonly IEmailService emailService;

        public MessageService(
            IBaseService baseService,
            IEmailService emailService)
        {
            this.baseService = baseService;
            this.emailService = emailService;
        }

        public IList<Message> GetMessageByCreator(GetMessageByCreatorParam param)
        {
            var criteria = new GetMessageByCreatorCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsDisabled = param.IsDisabled,
                CreatedBy = param.CreatedBy
            };
            return baseService.GetData<Message>(criteria).ToList();
        }

        public Message GetMessageById(int id)
        {
            var criteria = new GetMessageByIdCriteria { MessageId = id };
            return baseService.GetDataEntity<Message>(criteria);
        }

        public CommandResult Create(int actorId, Message model)
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
                CreatedBy = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Edit(int actorId, Message model)
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
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int actorId, int id)
        {
            var criteria = new DeleteMessageCriteria
            {
                MessageId = id,
                ModifiedBy = actorId,
                ModifiedDate = DateTimeOffset.Now.DateTime
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult EditStatus(int employeeId, int messageId)
        {
            var criteria = new EditMessageStatusCriteria
            {
                MessageId = messageId,
                EmployeeId = employeeId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult DeleteMessageByEmployee(int employeeId, int messageId)
        {
            var criteria = new DeleteMessageByEmployeeCriteria
            {
                MessageId = messageId,
                EmployeeId = employeeId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public IList<Message> GetMessageByEmployee(GetMessageByEmployeeParam param)
        {
            var criteria = new GetMessageByEmployeeCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsDisabled = param.IsDisabled,
                EmployeeId = param.EmployeeId
            };
            return baseService.GetData<Message>(criteria).ToList();
        }

        public Message GetDetailMessageByEmployee(int messageId)
        {
            var criteria = new GetMessageOfEmployeeByIdCriteria
            {
                MessageId = messageId
            };
            return baseService.GetDataEntity<Message>(criteria);
        }

        public IList<Message> GetUnreadMessageOfEmployee(int employeeId)
        {
            var criteria = new GetUnreadMessageOfEmployeeCriteria
            {
                EmployeeId = employeeId
            };
            return baseService.GetData<Message>(criteria).ToList();
        }

        public void SendMailToStaff(Message model)
        {
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(messageEmailTemplate));
            BackgroundJob.Enqueue(() => SendEmail(model.Subject, model.Contents, model.SendToEmail, bodyTemplate));
        }

        public void SendEmail(string subject, string content, string sendToEmail, string emailTemplatePath)
        {
            var body = string.Format(emailTemplatePath, subject, content);

            emailService.SendEmail(
                ConfigSettings.EmailSetting,
                sendToEmail,
                string.Empty,
                Resources.Messages.MessageSubject + " " + subject,
                body);
        }
    }
}