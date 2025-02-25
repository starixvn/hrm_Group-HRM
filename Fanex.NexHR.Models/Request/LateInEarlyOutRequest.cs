namespace Fanex.NexHR.Models.Request
{
    using System;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public class LateInEarlyOutRequest : BaseRequest
    {
        public DateTime OnDate { get; set; }

        public int LateInMins { get; set; }

        public int EarlyOutMins { get; set; }

        public int ActualLateInEarlyOut { get; set; }

        public string ShiftName { get; set; }

        public byte ReasonId { get; set; }

        public string ReasonName
        {
            get
            {
                switch (ReasonId)
                {
                    case (byte)LateInEarlyOutReason.ManagementAllow:
                        return Labels.ManagementAllow;

                    case (byte)LateInEarlyOutReason.Other:
                        return Labels.Other;

                    default:
                        return string.Empty;
                }
            }
        }
    }
}