namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Models.API.Todolist;

    public interface IAPITodolistRequestService
    {
        IList<TodolistItem> GetPendingRequestByUserId(int userId);
    }
}