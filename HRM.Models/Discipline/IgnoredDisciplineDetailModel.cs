namespace HRM.Models.Discipline
{
    using System;
    using Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class IgnoredDisciplineDetailModel
    {
        public DateTime WorkDate { get; set; }

        public int EmployeeId { get; set; }

        public short DisciplineTypeId { get; set; }

        public string IgnoreBy { get; set; }

        public DateTime IgnoreDate { get; set; }

        public bool IsIgnored { get; set; }
    }
}