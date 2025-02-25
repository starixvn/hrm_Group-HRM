namespace Fanex.NexHR.Models.Request
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;

    public class ProcessRequestParam
    {
        public long RequestId { get; set; }
        public RequestType RequestTypeId { get; set; }
        public byte RoleId { get; set; }
        public int EmployeeId { get; set; }
        public RequestCommand Command { get; set; }
        public string Comment { get; set; }
        public string ViewType { get; set; }
        public IEnumerable<DisciplineReview> Disciplines { get; set; } = new List<DisciplineReview>();
    }
}