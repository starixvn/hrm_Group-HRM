using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.AccessCounter;

public class UpdateAccessCounterCommand : CriteriaBase
{
    public int EmpId { get; set; }
    public DateTime Date { get; set; }
    public string FunctionName { get; set; }

    public override string GetSettingKey() => "AccessCounter_Update";

    public override bool IsValid() => EmpId > 0;
}