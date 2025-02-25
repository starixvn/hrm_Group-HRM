namespace HRM.Models.Discipline
{
    using System;

    public class DisciplineModel
    {
        public int DisciplineId { get; set; }

        public decimal Score { get; set; }

        public int Value { get; set; }

        public bool HasViolated { get; set; }

        public bool HasIgnored { get; set; }

        public bool HasApprovedBy { get; set; }

        public string IgnoreBy { get; set; }

        public DateTime IgnoreDate { get; set; }
    }
}