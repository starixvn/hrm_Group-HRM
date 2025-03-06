namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Models;
using HRM.Models.Request;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    public async Task<IList<LeaveRequest>> GetByDateAsync(ICriteria criteria)
    {
        using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        {
            using (var multi = await db.QueryMultipleAsync(criteria))
            {
                var leaves = multi.Read<LeaveRequest, LeaveType, Requester, Requester, Requester, Requester, Requester, LeaveRequest>(
                    (request, leaveType, employee, verifier, approver, hr, approvedBy) =>
                    {
                        request.LeaveType = leaveType;
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;
                        request.HR = hr;
                        request.ApprovedBy = approvedBy;
                        return request;
                    },
                    splitOn: "LeaveTypeId, EmpId, VerifierId, ApproverId, HRId, ApprovedById").ToList();

                var leaveDetails = multi.Read<LeaveRequestDetail>();
                return MappingLeaveDetail(leaves, leaveDetails);
            }
        }
    }

    private static IList<LeaveRequest> MappingLeaveDetail(IList<LeaveRequest> leaves, IEnumerable<LeaveRequestDetail> leaveDetails)
    {
        foreach (var leave in leaves)
        {
            leave.LeaveDetail = leaveDetails.Where(t => t.RequestId == leave.RequestId).ToList();

            if (leave.LeaveDetail.Any())
            {
                leave.TotalDays = leave.LeaveDetail.Where(x => x.WorkType == (byte)WorkType.Leave)
                    .Sum(x => x.IsHalfDay ? Constants.HalfDayOffValue : 1);
            }
        }

        return leaves;
    }

    public async Task<LeaveRequest> GetByIdAsync(ICriteria criteria)
    {
        using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        {
            using (var multi = await db.QueryMultipleAsync(criteria))
            {
                var leave = multi.Read<LeaveRequest, LeaveType, Requester, Requester, Requester, Requester, LeaveRequest>(
                    (request, leaveType, employee, verifier, approver, hr) =>
                    {
                        request.LeaveType = leaveType;
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;
                        request.HR = hr;
                        return request;
                    },
                    splitOn: "LeaveTypeId, EmpId, VerifierId, ApproverId, HRId").FirstOrDefault();

                if (leave != null)
                {
                    leave.LeaveDetail = multi.Read<LeaveRequestDetail>().ToList();
                }

                return leave;
            }
        }
    }

    public async Task<IEnumerable<LeaveRequest>> GetHistoryByIdAsync(ICriteria criteria)
    {
        using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        {
            using (var multi = await db.QueryMultipleAsync(criteria))
            {
                var leaves = multi.Read<LeaveRequest, LeaveType, Requester, Requester, Requester, Requester, Requester, LeaveRequest>(
                    (request, leaveType, employee, verifier, approver, hr, modifier) =>
                    {
                        request.LeaveType = leaveType;
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;
                        request.HR = hr;
                        request.Modifier = modifier;
                        return request;
                    },
                    splitOn: "LeaveTypeId, EmpId, VerifierId, ApproverId, HRId, ModifierId").ToList();
                var leaveDetails = multi.Read<LeaveRequestDetail>();

                foreach (var leave in leaves)
                {
                    leave.LeaveDetail = leaveDetails.Where(x => x.HistoryId == leave.HistoryId && x.RequestId == leave.RequestId).ToList();
                }

                return leaves;
            }
        }
    }

    public async Task<IEnumerable<Models.ScheduleArrange.DailySchedule>> GetEmployeeCalendarAsync(ICriteria criteria)
    {
        using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        {
            return await db.QueryAsync<Models.ScheduleArrange.DailySchedule,
                Models.ScheduleArrange.ShiftBase,
                Models.ScheduleArrange.LeaveRequest,
                Models.ScheduleArrange.LeaveRequest,
                Models.ScheduleArrange.DailySchedule>(
                (dailySchedule, shift, leave, leave2) =>
                {
                    dailySchedule.Shift = shift;
                    dailySchedule.LeaveType = leave;
                    dailySchedule.LeaveType2 = leave2;

                    return dailySchedule;
                },
                splitOn: "ShiftId, LeaveTypeId, LeaveTypeId2",
                parameterSet: criteria);
        }
    }
}