namespace HRM.Models.Section;

public class Section : EntityBase<int>
{
    public Department Department { get; set; }

    public bool IsActive { get; set; }
}