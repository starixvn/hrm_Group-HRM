namespace Fanex.NexHR.Models.Dashboard
{
    using System;
    using Fanex.NexHR.Core.Enums;

    public class MemberRequest
    {
        public int RoleId { get; set; }
        public ushort RequestTypeId { get; set; }
        public string RequestType { get; set; }
        public int TotalPending { get; set; }
        public int TotalApproved { get; set; }
        public int TotalRejected { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public ushort RequestFilterTypeId
        {
            get
            {
                var filterTypeId = (ushort)RequestFilter.All;
                switch (RequestTypeId)
                {
                    case (ushort)Core.Enums.RequestType.CheckIn:
                    case (ushort)Core.Enums.RequestType.CheckOut:
                        filterTypeId = (ushort)RequestFilter.CheckInOut;
                        break;

                    case (ushort)Core.Enums.RequestType.LateIn:
                    case (ushort)Core.Enums.RequestType.EarlyOut:
                        filterTypeId = (ushort)RequestFilter.LateInEarlyOut;
                        break;

                    case (ushort)Core.Enums.RequestType.ChangeShift:
                        filterTypeId = (ushort)RequestFilter.ChangeShift;
                        break;

                    case (ushort)Core.Enums.RequestType.Leave:
                        filterTypeId = (ushort)RequestFilter.Leave;
                        break;

                    case (ushort)Core.Enums.RequestType.KeepInView:
                        filterTypeId = (ushort)RequestFilter.KeepInView;
                        break;

                    default:
                        // nothing
                        break;
                }

                return filterTypeId;
            }
        }
    }
}