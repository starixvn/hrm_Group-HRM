namespace HRM.Criteria.Time;

using Fanex.Data.Repository;
using System;

public class InsertCheckInOutRecordCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public DateTime RecordTime { get; set; }

    public short InOutType { get; set; }

    public string IPAddress { get; set; }

    public string Platform { get; set; }

    public override string GetSettingKey()
    {
        return "TimeRecord_InsertCheckInOutTime";
    }

    public override bool IsValid()
    {
        return EmpId > 0;
    }
}