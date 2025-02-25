namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Discipline;

    public class DisciplineRepository : IDisciplineRepository
    {
        public IList<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryModel, EmployeeInfoModel, DisciplineSummaryModel>(
                    (discipline, employee) =>
                    {
                        discipline.Employee = employee;

                        return discipline;
                    },
                    "EmployeeId",
                    criteria).ToList();
            }
        }

        public IList<DisciplineDetailModel> GetDisciplineDetail(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IList<dynamic> disciplines = multi.Read<dynamic>().ToList();
                    IList<IgnoredDisciplineDetailModel> ignoreDiciplines = multi.Read<IgnoredDisciplineDetailModel>().ToList();
                    IList<DisciplineDetailModel> disciplineDetails = MappingObjectForDisciplineDetail(disciplines);
                    CalculateScore(disciplineDetails);
                    AddIgnoreDisciplineDetail(disciplineDetails, ignoreDiciplines);

                    return disciplineDetails;
                }
            }
        }

        [SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        private static void AddIgnoreDisciplineDetail(IList<DisciplineDetailModel> disciplineDetails, IList<IgnoredDisciplineDetailModel> ignoreDiciplines)
        {
            foreach (DisciplineDetailModel disciplineDetail in disciplineDetails)
            {
                foreach (IgnoredDisciplineDetailModel ignoreDicipline in
                    ignoreDiciplines.Where(ignore => ignore.EmployeeId == disciplineDetail.Employee.EmployeeId && ignore.WorkDate == disciplineDetail.WorkDate))
                {
                    switch (ignoreDicipline.DisciplineTypeId)
                    {
                        case (short)DisciplineType.ChangeShift:
                            disciplineDetail.ChangeShift.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.ChangeShift.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.MissingCheckInOut:
                            disciplineDetail.MissingCheckInOut.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.MissingCheckInOut.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.LateIn:
                            disciplineDetail.LateIn.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.LateIn.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.EarlyOut:
                            disciplineDetail.EarlyOut.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.EarlyOut.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.Absent:
                            disciplineDetail.Absent.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.Absent.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.UnpaidLeave:
                            disciplineDetail.UnpaidLeave.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.UnpaidLeave.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.WorkingUnderTime:
                            disciplineDetail.WorkingUnderTime.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.WorkingUnderTime.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.UnpaidLeaveAbsent:
                            disciplineDetail.UnpaidLeaveAbsent.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.UnpaidLeaveAbsent.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.UnplannedLeave:
                            disciplineDetail.UnplannedLeave.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.UnplannedLeave.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.WeekendLeave:
                            disciplineDetail.WeekendLeave.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.WeekendLeave.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        case (short)DisciplineType.SevenDaysLeave:
                            disciplineDetail.SevenDaysLeave.IgnoreBy = ignoreDicipline.IgnoreBy;
                            disciplineDetail.SevenDaysLeave.IgnoreDate = ignoreDicipline.IgnoreDate;
                            break;

                        default:
                            // nothing
                            break;
                    }
                }
            }
        }

        [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
        private static IList<DisciplineDetailModel> MappingObjectForDisciplineDetail(IList<dynamic> disciplines)
        {
            return disciplines.Select(discipline =>
            {
                decimal? totalHours = discipline.TotalHours;
                int? lateIn = discipline.LateIn;
                int? earlyOut = discipline.EarlyOut;
                DateTime? checkIn = discipline.CheckIn;
                DateTime? checkOut = discipline.CheckOut;

                return new DisciplineDetailModel
                {
                    WorkDate = discipline.WorkDate,
                    ShiftName = discipline.ShiftName,
                    CheckIn = (checkIn.HasValue && checkIn.Value != default(DateTime)) ? checkIn.Value : (DateTime?)null,
                    CheckOut = (checkOut.HasValue && checkOut.Value != default(DateTime)) ? checkOut.Value : (DateTime?)null,
                    TotalHours = totalHours.HasValue ? ConvertNumberToTime(totalHours.Value * 60) : string.Empty,
                    MinutesLateIn = lateIn.HasValue ? ConvertNumberToTime(lateIn.Value) : string.Empty,
                    MinutesEarlyOut = earlyOut.HasValue ? ConvertNumberToTime(earlyOut.Value) : string.Empty,
                    Employee = new EmployeeInfoModel
                    {
                        EmployeeId = discipline.EmployeeId,
                        NickName = discipline.NickName,
                        FullName = discipline.FullName,
                        Code = discipline.Code,
                        IsLocal = discipline.IsLocal,
                        TeamName = discipline.TeamName,
                        TeamCode = discipline.TeamCode
                    },
                    ChangeShift = GetChangeShift(discipline),
                    MissingCheckInOut = GetMissingCheckInOut(discipline),
                    LateIn = GetLateIn(discipline),
                    EarlyOut = GetEarlyOut(discipline),
                    Absent = GetAbsent(discipline),
                    UnplannedLeave = GetUnplannedLeave(discipline),
                    UnpaidLeave = GetUnpaidLeave(discipline),
                    WorkingUnderTime = GetWorkingUnderTime(discipline),
                    UnpaidLeaveAbsent = GetUnpaidLeaveAbsent(discipline),
                    WeekendLeave = GetWeekendLeave(discipline),
                    SevenDaysLeave = GetSevenDaysLeave(discipline),
                    EmployeeComment = discipline.EmployeeComment,
                    ApproverComment = discipline.ApproverComment,
                    IsOutOfControlReason = discipline.IsOutOfControlReason
                };
            }).ToList();
        }

        private static DisciplineModel GetChangeShift(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.ChangeShiftId,
                Value = discipline.ChangeShiftValue,
                Score = discipline.ChangeShiftScore,
                HasViolated = discipline.ChangeShiftHasViolated,
                HasIgnored = discipline.ChangeShiftHasIgnored,
                HasApprovedBy = discipline.ChangeShiftHasApprovedBy
            };
        }

        private static DisciplineModel GetMissingCheckInOut(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.MissingCheckInOutId,
                Value = discipline.MissingCheckInOutValue,
                Score = discipline.MissingCheckInOutScore,
                HasViolated = discipline.MissingCheckInOutHasViolated,
                HasIgnored = discipline.MissingCheckInOutHasIgnored,
                HasApprovedBy = discipline.MissingCheckInOutHasApprovedBy
            };
        }

        private static DisciplineModel GetLateIn(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.LateInId,
                Value = discipline.LateInValue,
                Score = discipline.LateInScore,
                HasViolated = discipline.LateInHasViolated,
                HasIgnored = discipline.LateInHasIgnored,
                HasApprovedBy = discipline.LateInHasApprovedBy
            };
        }

        private static DisciplineModel GetEarlyOut(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.EarlyOutId,
                Value = discipline.EarlyOutValue,
                Score = discipline.EarlyOutScore,
                HasViolated = discipline.EarlyOutHasViolated,
                HasIgnored = discipline.EarlyOutHasIgnored,
                HasApprovedBy = discipline.EarlyOutHasApprovedBy
            };
        }

        private static DisciplineModel GetAbsent(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.AbsentId,
                Value = discipline.AbsentValue,
                Score = discipline.AbsentScore,
                HasViolated = discipline.AbsentHasViolated,
                HasIgnored = discipline.AbsentHasIgnored,
                HasApprovedBy = discipline.AbsentHasApprovedBy
            };
        }

        private static DisciplineModel GetUnplannedLeave(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.UnplannedLeaveId,
                Value = discipline.UnplannedLeaveValue,
                Score = discipline.UnplannedLeaveScore,
                HasViolated = discipline.UnplannedLeaveHasViolated,
                HasIgnored = discipline.UnplannedLeaveHasIgnored,
                HasApprovedBy = discipline.UnplannedLeaveHasApprovedBy
            };
        }

        private static DisciplineModel GetUnpaidLeave(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.UnpaidLeaveId,
                Value = discipline.UnpaidLeaveValue,
                Score = discipline.UnpaidLeaveScore,
                HasViolated = discipline.UnpaidLeaveHasViolated,
                HasIgnored = discipline.UnpaidLeaveHasIgnored,
                HasApprovedBy = discipline.UnpaidLeaveHasApprovedBy
            };
        }

        private static DisciplineModel GetWorkingUnderTime(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.WorkingUnderTimeId,
                Value = discipline.WorkingUnderTimeValue,
                Score = discipline.WorkingUnderTimeScore,
                HasViolated = discipline.WorkingUnderTimeHasViolated,
                HasIgnored = discipline.WorkingUnderTimeHasIgnored,
                HasApprovedBy = discipline.WorkingUnderTimeHasApprovedBy
            };
        }

        private static DisciplineModel GetUnpaidLeaveAbsent(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.UnpaidLeaveAbsentId,
                Value = discipline.UnpaidLeaveAbsentValue,
                Score = discipline.UnpaidLeaveAbsentScore,
                HasViolated = discipline.UnpaidLeaveAbsentHasViolated,
                HasIgnored = discipline.UnpaidLeaveAbsentHasIgnored,
                HasApprovedBy = discipline.UnpaidLeaveAbsentHasApprovedBy
            };
        }

        private static DisciplineModel GetWeekendLeave(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.WeekendLeaveId,
                Value = discipline.WeekendLeaveValue,
                Score = discipline.WeekendLeaveScore,
                HasViolated = discipline.WeekendLeaveHasViolated,
                HasIgnored = discipline.WeekendLeaveHasIgnored,
                HasApprovedBy = discipline.WeekendLeaveHasApprovedBy
            };
        }

        private static DisciplineModel GetSevenDaysLeave(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.SevenDaysLeaveId,
                Value = discipline.SevenDaysLeaveValue,
                Score = discipline.SevenDaysLeaveScore,
                HasViolated = discipline.SevenDaysLeaveHasViolated,
                HasIgnored = discipline.SevenDaysLeaveHasIgnored,
                HasApprovedBy = discipline.SevenDaysLeaveHasApprovedBy
            };
        }

        private static void CalculateScore(IList<DisciplineDetailModel> disciplineDetails)
        {
            foreach (DisciplineDetailModel disciplineDetail in disciplineDetails)
            {
                CalculateSubtractScore(disciplineDetail);
            }
        }

        private static void CalculateSubtractScore(DisciplineDetailModel disciplineDetail)
        {
            var violations = new List<DisciplineModel>
            {
                disciplineDetail.MissingCheckInOut,
                disciplineDetail.LateIn,
                disciplineDetail.EarlyOut,
                disciplineDetail.WorkingUnderTime,
                disciplineDetail.Absent,
                disciplineDetail.UnplannedLeave,
                disciplineDetail.UnpaidLeave,
                disciplineDetail.UnpaidLeaveAbsent,
                disciplineDetail.ChangeShift,
                disciplineDetail.WeekendLeave,
                disciplineDetail.SevenDaysLeave
            };

            foreach (DisciplineModel violation in violations.Where(violation => violation != null && violation.HasViolated))
            {
                disciplineDetail.SystemSubtractScore += violation.Score;
                disciplineDetail.ApproverSubtractScore += !violation.HasIgnored ? violation.Score : 0;
            }
        }

        private static string ConvertNumberToTime(decimal number)
        {
            decimal numberMinutes = Math.Round(number);
            var timeSpan = new TimeSpan(0, (int)numberMinutes, 0);
            string time = number > 0 ? timeSpan.ToString(@"hh\:mm") : string.Empty;

            return time;
        }
    }
}