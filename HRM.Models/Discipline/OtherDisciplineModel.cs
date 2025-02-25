namespace HRM.Models.Discipline
{
    using System;

    public class OtherDisciplineModel
    {
        public DateTime WorkDate { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public float Score { get; set; }

        public short DisciplineTypeId { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }
    }
}