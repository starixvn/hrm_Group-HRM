using Fanex.Data;
using Fanex.Data.Repository;

namespace HRM.Criteria.Group;

public class GetEmployeesInGroupScopeCriteria : CriteriaBase
{
    public string Query { get; set; }

    public string TeamIds { get; set; }

    public int? Enable
    {
        get
        {
            int? disableUsers = null;
            if (EnableUser.HasValue)
            {
                disableUsers = EnableUser.Value ? 1 : 0;
            }

            return disableUsers;
        }
    }

    public int? IsDelete
    {
        get
        {
            int? deleteUsers = null;
            if (IsDeleteUser.HasValue)
            {
                deleteUsers = IsDeleteUser.Value ? 1 : 0;
            }

            return deleteUsers;
        }
    }

    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }

    public int? TotalRow { get; set; }

    public int? TitleId { get; set; }

    [SpParam(Ignored = true)]
    public bool? EnableUser { get; set; }

    [SpParam(Ignored = true)]
    public bool? IsDeleteUser { get; set; }

    [SpParam(Ignored = true)]
    public bool? IsAllUser { get; set; }

    public string CompanyIds { get; set; }

    public override string GetSettingKey()
    {
        return "Employee_GroupScope_Get";
    }

    public override bool IsValid() => true;
}