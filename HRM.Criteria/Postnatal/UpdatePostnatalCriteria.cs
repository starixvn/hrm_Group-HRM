namespace HRM.Criteria.Postnatal
{
    using System;
    using Fanex.Data.Repository;

    public class UpdatePostnatalCriteria : CriteriaBase
    {
        public int PostnatalId { get; set; }
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ModifiedBy { get; set; }
        public string Remark { get; set; }
        public byte WorkingHours { get; set; }

        public override string GetSettingKey() => "Postnatal_Update";

        public override bool IsValid() => PostnatalId > 0 && EmpId > 0 && ModifiedBy > 0 && FromDate <= ToDate;
    }
}