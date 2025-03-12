namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models.Request;
    using Models.Request.ChangeShift;
    using Models.ScheduleArrange;

    public interface IChangeShiftRequestRepository
    {
        Task<IEnumerable<DailySchedule>> GetWorkingCalendarForRequestAsync(ICriteria criteria);

        Task<IEnumerable<ChangeShiftRequest>> GetByEmployeeAsync(ICriteria criteria);

        Task<ChangeShiftRequest> GetByIdAsync(ICriteria criteria);

        Task<IEnumerable<ChangeShiftRequest>> GetHistoryByIdAsync(ICriteria criteria);
    }

    public class ChangeShiftRequestRepository : IChangeShiftRequestRepository
    {
        public async Task<IEnumerable<DailySchedule>> GetWorkingCalendarForRequestAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<DailySchedule, ShiftBase, DailySchedule>(
                    (dailySchedule, shift) =>
                    {
                        dailySchedule.Shift = shift;
                        return dailySchedule;
                    },
                    splitOn: "ShiftId",
                    parameterSet: criteria);
            }
        }

        public async Task<IEnumerable<ChangeShiftRequest>> GetByEmployeeAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<ChangeShiftRequest, Requester, Requester, Requester, Requester, Requester, ChangeShiftRequest>(
                    (request, employee, verifier, approver, hr, approvedBy) =>
                    {
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;
                        request.HR = hr;
                        request.ApprovedBy = approvedBy;
                        return request;
                    },
                    splitOn: "EmpId, VerifierId, ApproverId, HRId, ApprovedById",
                    parameterSet: criteria);
            }
        }

        public async Task<ChangeShiftRequest> GetByIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryEntityAsync<ChangeShiftRequest, Requester, Requester, Requester, ChangeShiftRequest>(
                    (request, employee, verifier, approver) =>
                    {
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;
                        return request;
                    },
                    splitOn: "EmpId, VerifierId, ApproverId",
                    parameterSet: criteria);
            }
        }

        public async Task<IEnumerable<ChangeShiftRequest>> GetHistoryByIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<ChangeShiftRequest, Requester, Requester, Requester, Requester, ChangeShiftRequest>(
                    (request, employee, verifier, approver, modifier) =>
                    {
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;
                        request.Modifier = modifier;
                        return request;
                    },
                    splitOn: "EmpId, VerifierId, ApproverId, ModifierId",
                    parameterSet: criteria);
            }
        }
    }
}