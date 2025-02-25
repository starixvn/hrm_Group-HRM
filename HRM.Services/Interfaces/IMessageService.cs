namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Models.Message;

    public interface IMessageService
    {
        IList<Message> GetMessageByCreator(GetMessageByCreatorParam param);

        Message GetMessageById(int id);

        CommandResult Create(int actorId, Message model);

        CommandResult Edit(int actorId, Message model);

        CommandResult Delete(int actorId, int id);

        CommandResult EditStatus(int employeeId, int messageId);

        CommandResult DeleteMessageByEmployee(int employeeId, int messageId);

        IList<Message> GetMessageByEmployee(GetMessageByEmployeeParam param);

        Message GetDetailMessageByEmployee(int messageId);

        IList<Message> GetUnreadMessageOfEmployee(int employeeId);

        void SendMailToStaff(Message model);
    }
}