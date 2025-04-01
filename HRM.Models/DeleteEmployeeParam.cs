namespace HRM.Models;

using System;

public class DeleteEmployeeParam
{
    public int UserId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime ResignedDate { get; set; }
}