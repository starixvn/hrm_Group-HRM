namespace Fanex.NexHR.Models.API.Request
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.API.Todolist;

    public class ProcessRequestParam
    {
        public long RequestId { get; set; }
        public int UserId { get; set; }
        public byte RequestTypeId { get; set; }
        public byte StatusId { get; set; }
        public int EmpId { get; set; }
        public string Comment { get; set; }
        public IList<TodolistDiscipline> Disciplines { get; set; } = new List<TodolistDiscipline>();
    }
}