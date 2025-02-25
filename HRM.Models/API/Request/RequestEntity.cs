namespace HRM.Models.API.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class RequestEntity
    {
        public long RequestId { get; set; }

        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public int VerifierId { get; set; }
        public int ApproverId { get; set; }

        public byte RequestTypeId { get; set; }
        public byte StatusId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalDays { get; set; }
        public bool IsHalfDay { get; set; }
        public int TotalMinutes { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Remark { get; set; }

        public short? LeaveTypeId { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public IList<GeneralLeaveDetail> LeaveDetails { get; set; }

        public string LeaveCountDetails
        {
            get
            {
                var details = new List<string>();
                if (LeaveDetails != null && LeaveDetails.Any())
                {
                    var offday = LeaveDetails.Where(x => x.IsOffDay).Sum(x => x.LeaveDay);
                    if (offday > 0)
                    {
                        details.Add(GetLeaveCountDetails(offday, "Off"));
                    }

                    var holiday = LeaveDetails.Where(x => !x.IsOffDay && x.IsHoliday).Sum(x => x.LeaveDay);
                    if (holiday > 0)
                    {
                        details.Add(GetLeaveCountDetails(holiday, "PH"));
                    }

                    var leave = LeaveDetails.Where(x => !x.IsOffDay && !x.IsHoliday).GroupBy(x => x.LeaveTypeCode)
                        .Select(x => GetLeaveCountDetails(x.Sum(y => y.LeaveDay), x.Key)).ToList();
                    if (leave != null && leave.Any())
                    {
                        details.AddRange(leave);
                    }
                }

                return string.Join(", ", details);
            }
        }

        public byte ChangeShiftTypeId { get; set; }
        public IList<ChangeShiftDetail> ChangeShiftDetails { get; set; }

        public short? FromShiftId { get; set; }
        public string FromShiftName { get; set; }
        public short? ToShiftId { get; set; }
        public string ToShiftName { get; set; }

        public short InOutMinutes { get; set; }
        public short ActualInOutMinutes { get; set; }

        // Keep In View
        public byte? KeepInViewTypeId { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        public bool CrossDay => FromTime.HasValue && ToTime.HasValue && FromTime.Value.DayOfYear > ToTime.Value.DayOfYear; 

        private static string GetLeaveCountDetails(decimal totaldays, string leaveCode)
        {
            return string.Format("{0} day{1} {2}", totaldays, totaldays > 1 ? "s" : string.Empty, leaveCode);
        }
    }
}