namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.API.Todolist;

    public interface IAPITodolistRequestService
    {
        IList<TodolistItem> GetPendingRequestByUserId(int userId);
    }
}