namespace Fanex.NexHR.Models.Request.ChangeShift
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class CreateChangeShift
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte TypeId { get; set; }
        public int? VerifierId { get; set; }
        public int? ApproverId { get; set; }

        [AllowHtml]
        public string Remark { get; set; }

        public IEnumerable<ChangeShiftDetail> Details { get; set; } = new List<ChangeShiftDetail>();
    }
}