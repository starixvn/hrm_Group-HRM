namespace Fanex.NexHR.Models.API.Todolist
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TodolistItem
    {
        public short TypeId { get; set; }
        public string Name { get; set; }
        public IList<TodolistAction> Actions { get; set; } = new List<TodolistAction>();
    }
}