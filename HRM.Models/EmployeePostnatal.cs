namespace HRM.Models
{
    using System;

    public class EmployeePostnatal
    {
        public int PostnatalId { get; set; }

        public int EmpId { get; set; }

        public string Nickname { get; set; }

        public string Fullname { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int CreatedBy { get; set; }

        public string Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ModifiedBy { get; set; }

        public string Modifier { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string Remark { get; set; }

        public byte WorkingHours { get; set; }
    }
}