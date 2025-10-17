namespace HRM.Criteria.Report;

using Fanex.Data.Repository;

public class GetEmployeeByDataPermissionCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public short CompanyId { get; set; }

    public bool IsViewAll { get; set; }

    public bool IsViewEmployeeAssigned { get; set; }

    public bool IsShowUserInTeam { get; set; }

    public override bool IsValid() => CompanyId > 0 && UserId > 0;

    public override string GetSettingKey() => "Employee_GetByDataPermission";
}