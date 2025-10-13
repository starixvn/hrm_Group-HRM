namespace HRM.Models;

using System;

public class EmployeeTransfer : Employee
{
    public short TargetCompanyId { get; set; }

    public string Remark { get; set; }

    public DateTime TransferInDate { get; set; }
}