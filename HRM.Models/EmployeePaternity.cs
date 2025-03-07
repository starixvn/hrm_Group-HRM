namespace HRM.Models
{
    using HRM.Core;
    using System;

    public class EmployeePaternity
    {
        private DateTime serverCreatedDate;
        public int PaternityId { get; set; }

        public int EmpId { get; set; }

        public string Nickname { get; set; }

        public string Fullname { get; set; }

        public DateTime ChildBornDate { get; set; }

        public int LeaveDay { get; set; }

        public int CreatedBy { get; set; }

        public string Creator { get; set; }

        public DateTime CreatedDate
        {
            get => serverCreatedDate;
            set => serverCreatedDate = value.ToClientTimeZone();
        }

        public int ModifiedBy { get; set; }

        public string Modifier { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string Remark { get; set; }
    }
}