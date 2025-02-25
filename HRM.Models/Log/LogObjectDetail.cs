namespace HRM.Models.Log
{
    using System;
    using System.Collections.Generic;

    public class LogObjectDetail
    {
        public DateTime LogDate { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string OldData { get; set; }
        public string Data { get; set; }

        public IList<FieldValue> Fields { get; set; } = new List<FieldValue>();
    }
}