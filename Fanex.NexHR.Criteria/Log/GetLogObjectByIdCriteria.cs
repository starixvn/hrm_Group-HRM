namespace Fanex.NexHR.Criteria.Log
{
    using Fanex.Data.Repository;

    public class GetLogObjectByIdCriteria : CriteriaBase
    {
        public byte ObjectTypeId { get; set; }
        public int ObjectId { get; set; }

        public override string GetSettingKey() => "ActivityLog_GetByObjectId";

        public override bool IsValid() => ObjectId > 0 && ObjectTypeId > 0;
    }
}