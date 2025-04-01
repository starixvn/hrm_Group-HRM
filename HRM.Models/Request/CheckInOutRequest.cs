namespace HRM.Models.Request;

using System;
using Core.Enums;
using Resources;

public class CheckInOutRequest : BaseRequest
{
    public DateTime OnDate { get; set; }

    public DateTime RequestTime { get; set; }

    public byte ReasonId { get; set; }

    public string ShiftName { get; set; }

    public string ReasonName
    {
        get
        {
            return ReasonId switch
            {
                (byte)CheckInOutReason.VanLate => Labels.VanLate,
                (byte)CheckInOutReason.BusinessTrip => Labels.BusinessTrip,
                (byte)CheckInOutReason.Forgot => Labels.Forgot,
                (byte)CheckInOutReason.Other => Labels.Other,
                _ => string.Empty
            };
        }
    }
}