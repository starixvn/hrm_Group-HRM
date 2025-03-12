namespace HRM.ViewModels.Overtime
{
    using System;
    using System.Collections.Generic;
    using Models;
    using HRM.Models.Overtime;

    public class MemberMonthlyOvertimeRequestViewModel
    {
        public string Employee { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? Month { get; set; }
        public string TeamIds { get; set; }
        public short StatusId { get; set; } = -1; // pending and verified
        public byte? IsLocal { get; set; }
        public IEnumerable<MonthlyOvertimeRequest> MemberRequests { get; set; } = new List<MonthlyOvertimeRequest>();
        public IEnumerable<EntityBase<int>> Teams { get; set; }
        public string Sort { get; set; }
        public short OvertimeType { get; set; }
    }
}