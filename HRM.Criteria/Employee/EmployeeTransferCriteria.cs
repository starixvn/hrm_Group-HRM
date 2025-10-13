namespace HRM.Criteria.Employee;

using System;
using Fanex.Data.Repository;

public class EmployeeTransferCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public int GroupId { get; set; }

    public short TargetCompanyId { get; set; }

    public string Remark { get; set; }

    public int CreatedBy { get; set; }

    public DateTime TransferInDate { get; set; }

    public override string GetSettingKey() => "Employee_Transfer";

    public override bool IsValid() => EmpId > 0 && GroupId > 0 && TargetCompanyId > 0;
}