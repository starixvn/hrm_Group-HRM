namespace HRM.Criteria.Memo
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class EditMemoCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string RefNo { get; set; }

        public string Contents { get; set; }

        public string Subject { get; set; }

        public string SentTo { get; set; }

        public string SentFrom { get; set; }

        public string ForInfo { get; set; }

        public int TotalPages { get; set; }

        public DateTime SentDate { get; set; }

        public int Category { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int MemoType { get; set; }

        [SpParam(Ignored = true)]
        public bool SendMail { get; set; }

        public override string GetSettingKey() => "Memo_Update";

        public override bool IsValid() => Id > 0;
    }
}