namespace HRM.Models.Request;

using System;
using Core.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class DisciplineReview
{
    public DateTime WorkDate { get; set; }
    public int EmployeeId { get; set; }
    public bool IsIgnored { get; set; }
}