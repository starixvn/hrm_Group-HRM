namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        public IList<LeaveRequest> GetByDate(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var leaves = multi.Read<LeaveRequest, LeaveType, Requester, Requester, Requester, Requester, Requester, LeaveRequest >(
                        (request, leaveType, employee, verifier, approver, hr,approvedBy) =>
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

        public LeaveRequest GetById(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
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

        public IEnumerable<LeaveRequest> GetHistoryById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var leaves = multi.Read<LeaveRequest, LeaveType, Requester, Requester, Requester, Requester, Requester, LeaveRequest >(
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

        public IEnumerable<Models.ScheduleArrange.DailySchedule> GetEmployeeCalendar(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<Models.ScheduleArrange.DailySchedule,
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
}