namespace HRM.Models.Request
{
    using System.Collections.Generic;
    using Core.Enums;

    public class ApproveRequestsParam
    {
        public IEnumerable<RequestsXML> Requests { get; set; }

        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }
        public RequestCommand Command { get; set; }
        public bool IsIgnoredViolate { get; set; }
        public bool IsConfirmed => !IsIgnoredViolate;
    }
}