namespace HRM.Models.Request
{
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
                switch (ReasonId)
                {
                    case (byte)CheckInOutReason.VanLate:
                        return Labels.VanLate;

                    case (byte)CheckInOutReason.BusinessTrip:
                        return Labels.BusinessTrip;

                    case (byte)CheckInOutReason.Forgot:
                        return Labels.Forgot;

                    case (byte)CheckInOutReason.Other:
                        return Labels.Other;

                    default:
                        return string.Empty;
                }
            }
        }
    }
}