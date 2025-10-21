namespace HRM.Models;

using System;

public class EmployeeTransfer : Employee
{
    public short TargetCompanyId { get; set; }
    public short TransferOutCompanyId { get; set; }

    public string TransferRemark { get; set; }

    public DateTime TransferInDate { get; set; }
}