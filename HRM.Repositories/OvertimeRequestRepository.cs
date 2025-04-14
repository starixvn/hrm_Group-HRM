namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Models.Overtime;
using Models.Request;
using Interfaces;

public class OvertimeRequestRepository : IOvertimeRequestRepository
{
    public async Task<IEnumerable<MonthlyOvertimeRequest>> GetMonthlyRequestByEmployeeId(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryAsync<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, MonthlyOvertimeRequest>(
            (re, stt, verifier, approver) =>
            {
                re.Status = stt;
                re.Verifier = verifier;
                re.Approver = approver;
                return re;
            },
            "StatusId, VerifierId, ApproverId",
            criteria);
    }

    public async Task<MonthlyOvertimeRequest> GetMonthlyRequestDetailsByEmployeeId(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var monthlyRequest = multi.Read<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, Requester, MonthlyOvertimeRequest>(
            (re, stt, requester, verifier, approver) =>
            {
                re.Requester = requester;
                re.Status = stt;
                re.Verifier = verifier;
                re.Approver = approver;
                return re;
            },
            "StatusId, EmpId, VerifierId, ApproverId").FirstOrDefault();

        if (monthlyRequest != null)
        {
            monthlyRequest.DailyOvertimes = multi.Read<DailyOvertimeRequest>();
        }

        return monthlyRequest;
    }

    public async Task<IEnumerable<MonthlyOvertimeRequest>> GetMemberMonthlyRequestAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryAsync<MonthlyOvertimeRequest, RequestStatus, Requester, MonthlyOvertimeRequest>(
            (req, stt, requester) =>
            {
                req.Status = stt;
                req.Requester = requester;
                return req;
            },
            "StatusId, EmpId",
            criteria);
    }
}