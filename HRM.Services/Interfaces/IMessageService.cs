using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models.Message;

namespace HRM.Services.Interfaces;

public interface IMessageService
{
    Task<IEnumerable<Message>> GetMessageByCreator(GetMessageByCreatorParam param);

    Task<Message> GetMessageById(int id);

    Task<CommandResult> Create(int actorId, Message model);

    Task<CommandResult> Edit(int actorId, Message model);

    Task<CommandResult> Delete(int actorId, int id);

    Task<CommandResult> EditStatus(int employeeId, int messageId);

    Task<CommandResult> DeleteMessageByEmployee(int employeeId, int messageId);

    Task<IEnumerable<Message>> GetMessageByEmployee(GetMessageByEmployeeParam param);

    Task<Message> GetDetailMessageByEmployee(int messageId);

    Task<IEnumerable<Message>> GetUnreadMessageOfEmployeeAsync(int employeeId);

    Task SendMailToStaff(Message model);
}