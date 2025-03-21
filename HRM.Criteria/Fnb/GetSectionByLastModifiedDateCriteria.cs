using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb
{
    public class GetSectionByLastModifiedDateCriteria : CriteriaBase
    {
        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey() => "FnB_Section_GetByLastModifiedDate";

        public override bool IsValid() => true;
    }
}
