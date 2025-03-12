namespace HRM.Criteria;

using Fanex.Data.Repository;

public class GetUnpaidLeaveCriteria : CriteriaBase
{
    public int? EmpId { get; set; }

    public string TeamIds { get; set; }

    public int Year { get; set; }

    public byte? IsLocal { get; set; }

    public int? SupervisorId { get; set; }

    public string LeaveType { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "Report_UnpaidLeaveSummary";
    }

    public override bool IsValid()
    {
        return Year > 0;
    }
}