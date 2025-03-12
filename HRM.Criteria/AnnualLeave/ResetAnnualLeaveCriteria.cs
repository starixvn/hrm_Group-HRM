namespace HRM.Criteria.AnnualLeave;

using System;
using System.Collections.Generic;
using System.Text;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;

public class ResetAnnualLeaveCriteria : CriteriaBase
{
    private const string _AnnualLeaveTpl = "<Employee Id=\"{0}\" LeaveBalance=\"{1}\"/>";

    [SpParam(Ignored = true)]
    public IList<LeaveSummary> AnnualLeaves { get; set; }

    public string EmpValue
    {
        get
        {
            var value = new StringBuilder("<ROOT>");

            if (AnnualLeaves != null)
            {
                foreach (var leaveSummary in AnnualLeaves)
                {
                    value.AppendFormat(_AnnualLeaveTpl, leaveSummary.EmpId, leaveSummary.LeaveBalance);
                }
            }

            return value.Append("</ROOT>").ToString();
        }
    }

    public string Description { get; set; }

    public DateTime FromDate { get; set; }

    public decimal ResetDays { get; set; }

    public int ActorId { get; set; }

    public override string GetSettingKey() => "AnnualLeave_ResetMultipleTransactions";

    public override bool IsValid() => true;
}