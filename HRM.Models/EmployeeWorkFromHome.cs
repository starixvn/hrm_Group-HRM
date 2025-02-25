namespace HRM.Models
{
    using System;
    using System.Collections.Generic;

    public class EmployeeWorkFromHome
    {
        public int EmployeeWorkFromHomeId { get; set; }

        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string Department { get; set; }
        public string Section { get; set; }
        public string Team { get; set; }

        public int ModifiedBy { get; set; }

        public string Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Remark { get; set; }
        public IList<AttachFile> AttachFiles { get; set; }
    }
}