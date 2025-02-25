namespace HRM.Models
{
    public class Department : EntityBase<int>
    {
        public Unit Unit { get; set; }

        public bool IsActive { get; set; }
    }
}
