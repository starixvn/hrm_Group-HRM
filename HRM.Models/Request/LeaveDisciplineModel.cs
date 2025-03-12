namespace HRM.Models.Request
{
    using System;
    using System.Collections.Generic;
    using Core.Enums;
    using Resources;

    public class LeaveDisciplineModel
    {
        public DateTime WorkDate { get; set; }
        public bool IsOffDayOrHoliday { get; set; }
        public string ScheduleStatus { get; set; }
        public bool IsUnpaidLeaveAbsent { get; set; }
        public bool IsUnpaidLeave { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsUnplannedLeave { get; set; }
        public bool IsWeekendLeave { get; set; }
        public bool IsSevenDaysLeave { get; set; }

        public IList<LeaveDisciplineDetailModel> LeaveDetail { get; set; }

        public short DisciplineTypeId
        {
            get
            {
                short disciplineTypeId;
                if (IsUnpaidLeaveAbsent)
                {
                    disciplineTypeId = (short)DisciplineType.UnpaidLeaveAbsent;
                }
                else if (IsAbsent)
                {
                    disciplineTypeId = (short)DisciplineType.Absent;
                }
                else if (IsUnplannedLeave)
                {
                    disciplineTypeId = (short)DisciplineType.UnplannedLeave;
                }
                else if (IsUnpaidLeave)
                {
                    disciplineTypeId = (short)DisciplineType.UnpaidLeave;
                }
                else if (IsSevenDaysLeave)
                {
                    disciplineTypeId = (short)DisciplineType.SevenDaysLeave;
                }
                else
                {
                    disciplineTypeId = -1;
                }

                return disciplineTypeId;
            }
        }

        public string DisciplineTypeName
        {
            get
            {
                string disciplineName;
                if (IsUnpaidLeaveAbsent)
                {
                    disciplineName = Labels.UnpaidLeaveAbsent;
                }
                else if (IsAbsent)
                {
                    disciplineName = Labels.Absent;
                }
                else if (IsUnplannedLeave)
                {
                    disciplineName = Labels.LateRequestLeave;
                }
                else if (IsUnpaidLeave)
                {
                    disciplineName = Labels.UnpaidLeave;
                }
                else if (IsSevenDaysLeave)
                {
                    disciplineName = Labels.SevenDaysLeave;
                }
                else
                {
                    disciplineName = string.Empty;
                }

                return disciplineName;
            }
        }
    }
}
