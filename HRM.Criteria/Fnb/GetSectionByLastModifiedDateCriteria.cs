using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb;

public class GetSectionByLastModifiedDateCriteria : CriteriaBase
{
    public short CompanyId { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "FnB_Section_GetByLastModifiedDate";

    public override bool IsValid() => true;
}