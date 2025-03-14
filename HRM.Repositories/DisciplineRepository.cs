using HRM.Repositories.Interfaces;

namespace HRM.Repositories;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Core.Enums;
using Models.Discipline;

public class DisciplineRepository : IDisciplineRepository
{
    public async Task<IList<DisciplineSummaryModel>> GetDisciplineSummaryAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var result = await db.QueryAsync<DisciplineSummaryModel, EmployeeInfoModel, DisciplineSummaryModel>(
            (discipline, employee) =>
            {
                discipline.Employee = employee;

                return discipline;
            },
            "EmployeeId",
            criteria);
        return result.ToList();
    }

    public async Task<IList<DisciplineDetailModel>> GetDisciplineDetailAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var disciplines = await multi.ReadAsync<dynamic>();
        var ignoreDisciplines = await multi.ReadAsync<IgnoredDisciplineDetailModel>();
        var disciplineDetails = MappingObjectForDisciplineDetail(disciplines);
        CalculateScore(disciplineDetails);
        AddIgnoreDisciplineDetail(disciplineDetails, ignoreDisciplines);

        return disciplineDetails;
    }

    [SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
    private static void AddIgnoreDisciplineDetail(IEnumerable<DisciplineDetailModel> disciplineDetails, IEnumerable<IgnoredDisciplineDetailModel> ignoreDisciplines)
    {
        foreach (var disciplineDetail in disciplineDetails)
        {
            foreach (var ignoreDiscipline in
                     ignoreDisciplines.Where(ignore => ignore.EmployeeId == disciplineDetail.Employee.EmployeeId && ignore.WorkDate == disciplineDetail.WorkDate))
            {
                switch (ignoreDiscipline.DisciplineTypeId)
                {
                    case (short)DisciplineType.ChangeShift:
                        disciplineDetail.ChangeShift.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.ChangeShift.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.MissingCheckInOut:
                        disciplineDetail.MissingCheckInOut.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.MissingCheckInOut.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.LateIn:
                        disciplineDetail.LateIn.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.LateIn.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.EarlyOut:
                        disciplineDetail.EarlyOut.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.EarlyOut.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.Absent:
                        disciplineDetail.Absent.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.Absent.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.UnpaidLeave:
                        disciplineDetail.UnpaidLeave.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.UnpaidLeave.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.WorkingUnderTime:
                        disciplineDetail.WorkingUnderTime.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.WorkingUnderTime.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.UnpaidLeaveAbsent:
                        disciplineDetail.UnpaidLeaveAbsent.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.UnpaidLeaveAbsent.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.UnplannedLeave:
                        disciplineDetail.UnplannedLeave.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.UnplannedLeave.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.WeekendLeave:
                        disciplineDetail.WeekendLeave.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.WeekendLeave.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;

                    case (short)DisciplineType.SevenDaysLeave:
                        disciplineDetail.SevenDaysLeave.IgnoreBy = ignoreDiscipline.IgnoreBy;
                        disciplineDetail.SevenDaysLeave.IgnoreDate = ignoreDiscipline.IgnoreDate;
                        break;
                }
            }
        }
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    private static IList<DisciplineDetailModel> MappingObjectForDisciplineDetail(IEnumerable<dynamic> disciplines)
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
                CheckIn = (checkIn.HasValue && checkIn.Value != default(DateTime)) ? checkIn.Value : null,
                CheckOut = (checkOut.HasValue && checkOut.Value != default(DateTime)) ? checkOut.Value : null,
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
        foreach (var disciplineDetail in disciplineDetails)
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

        foreach (var violation in violations.Where(violation => violation != null && violation.HasViolated))
        {
            disciplineDetail.SystemSubtractScore += violation.Score;
            disciplineDetail.ApproverSubtractScore += !violation.HasIgnored ? violation.Score : 0;
        }
    }

    private static string ConvertNumberToTime(decimal number)
    {
        var numberMinutes = Math.Round(number);
        var timeSpan = new TimeSpan(0, (int)numberMinutes, 0);
        var time = number > 0 ? timeSpan.ToString(@"hh\:mm") : string.Empty;

        return time;
    }
}