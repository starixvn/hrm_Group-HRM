using System;

namespace BIO.WebAPI.Models;

public class TimeRecord
{
    public string EmpNo { get; set; }
    public byte InOutType { get; set; }
    public DateTime RecordTime { get; set; }
    public string Company { get; set; }
}