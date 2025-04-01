namespace HRM.Models.Dashboard;

using System;
using Core.Enums;

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
            var filterTypeId = RequestTypeId switch
            {
                (ushort)Core.Enums.RequestType.CheckIn or (ushort)Core.Enums.RequestType.CheckOut =>
                    (ushort)RequestFilter.CheckInOut,
                (ushort)Core.Enums.RequestType.LateIn or (ushort)Core.Enums.RequestType.EarlyOut =>
                    (ushort)RequestFilter.LateInEarlyOut,
                (ushort)Core.Enums.RequestType.ChangeShift => (ushort)RequestFilter.ChangeShift,
                (ushort)Core.Enums.RequestType.Leave => (ushort)RequestFilter.Leave,
                (ushort)Core.Enums.RequestType.KeepInView => (ushort)RequestFilter.KeepInView,
                _ => (ushort)RequestFilter.All
            };

            return filterTypeId;
        }
    }
}