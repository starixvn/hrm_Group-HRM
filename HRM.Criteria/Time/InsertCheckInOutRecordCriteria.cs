namespace HRM.Criteria.Time;

using System;
using Fanex.Data.Repository;

public class InsertCheckInOutRecordCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public DateTime RecordTime { get; set; }
    public DateTime ClientRecordTime { get; set; }

    public byte InOutType { get; set; }

    public string IPAddress { get; set; }

    public string Platform { get; set; }

    public override string GetSettingKey() => "TimeRecord_InsertCheckInOutTime";

    public override bool IsValid() => EmpId > 0;
}