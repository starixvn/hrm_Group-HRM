namespace HRM.Models.API.Todolist
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TodolistMetadata
    {
        public long RequestId { get; set; }
        public byte RequestTypeId { get; set; }
        public byte StatusId { get; set; }
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Comment { get; set; }

        public IList<TodolistDiscipline> Disciplines { get; set; } = new List<TodolistDiscipline>();
    }
}