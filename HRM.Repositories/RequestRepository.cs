namespace HRM.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
using Criteria.Request;
using Fanex.Data;
using Fanex.Data.Repository;
using Interfaces;
using Models;
using Models.Request;
using Models.Request.ChangeShift;
using Models.Shifts;
using Models.Supervisor;

public class RequestRepository : IRequestRepository
{
    public async Task<IList<RequestEntity>> GetRequestAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var entityList = FillData(multi.Read<dynamic>());
        var changeOffDateDetails = await multi.ReadAsync<ChangeShiftDetail>();

        foreach (var request in entityList.Where(entity => entity.RequestType.Id == (byte)RequestType.ChangeShift))
        {
            request.ChangeShiftDetails = changeOffDateDetails.Where(detail => detail.RequestID == request.Id);
        }

        return entityList;
    }

    public async Task<RequestRelatedPeopleModel> GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var relatedPeople = new RequestRelatedPeopleModel
        {
            Requester = multi.ReadFirstOrDefault<SupervisorNameModel>(),
            Verifier = multi.ReadFirstOrDefault<SupervisorNameModel>(),
            Approver = multi.ReadFirstOrDefault<SupervisorNameModel>(),
            ListHR = multi.Read<SupervisorNameModel>().ToList(),
            EmailSetting = multi.ReadFirstOrDefault<EmailSettingsModel>()
        };

        return relatedPeople;
    }

    public async Task<VerifierAndApproverForRequest> GetVerifierAndApproverAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var supervisor = new VerifierAndApproverForRequest
        {
            Verifiers = multi.Read<SupervisorNameModel>().ToList(),
            Approvers = multi.Read<SupervisorNameModel>().ToList()
        };

        return supervisor;
    }

    public async Task<RequestEntity> GetRequestToIgnoreById(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryEntityAsync<RequestEntity, Shift, Employee, RequestTypeModel, RequestEntity>(
            (request, shift, requester, type) =>
            {
                request.RequestType = type;
                request.FromShift = shift;
                request.Requester = requester;

                return request;
            },
            "ShiftId, RequesterId, RequestType",
            criteria);
    }

    public async Task<List<LeaveDisciplineModel>> GetLeaveRequestToReviewDiscipline(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var disciplines = multi.Read<LeaveDisciplineModel>().ToList();
        var leaveDetails = multi.Read<LeaveDisciplineDetailModel>().ToList();
        foreach (var discipline in disciplines)
        {
            discipline.LeaveDetail = leaveDetails.Where(t => t.WorkDate == discipline.WorkDate).ToList();
        }

        return disciplines;
    }

    private static List<RequestEntity> FillData(dynamic data)
    {
        var requests = new List<RequestEntity>();
        foreach (var request in data)
        {
            RequestEntity entity = FillDataEntity(request);
            requests.Add(entity);
        }

        return requests;
    }

    private static RequestEntity FillDataEntity(dynamic requestInfo)
    {
        var entity = new RequestEntity
        {
            Id = requestInfo.RequestId,
            RequestType = new RequestTypeModel { Id = (short)requestInfo.RequestType },
            Status = new RequestStatus { Id = requestInfo.Status },
            FromDate = requestInfo.FromDate,
            ToDate = requestInfo.ToDate,
            TotalDays = requestInfo.TotalDays,
            IsHalfDay = requestInfo.IsHalfDay ?? false,
            FromTime = requestInfo.FromTime ?? DateTimeOffset.Now.DateTime,
            ToTime = requestInfo.ToTime ?? DateTimeOffset.Now.DateTime,
            KeepInViewType = (byte)(requestInfo.KeepInViewType ?? 0),
            LeaveCountDetails = requestInfo.LeaveCountDetails ?? string.Empty,
            LeaveType = new LeaveType { Id = requestInfo.LeaveTypeId ?? 0, Name = requestInfo.LeaveTypeName ?? string.Empty },
            Remark = requestInfo.Remark ?? string.Empty,
            TotalDaysRequestInMonth = requestInfo.TotalDaysRequestInMonth ?? 0,
            CreatedDate = requestInfo.CreatedDate,
            ReasonId = (byte)requestInfo.ReasonId,
            RefNO = requestInfo.RefNo ?? string.Empty,
            Requester = GetRequester(requestInfo),
            VerifierId = requestInfo.VerifierId,
            VerifierNickName = requestInfo.VerifierNickName,
            VerifierFullName = requestInfo.VerifierFullName,
            VerifiedDate = requestInfo.VerifiedDate,
            ApproverId = requestInfo.ApproverId,
            ApproverNickName = requestInfo.ApproverNickName,
            ApproverFullName = requestInfo.ApproverFullName,
            ApprovedDate = requestInfo.ApprovedDate,
            ProcessedByNickName = requestInfo.ProcessedByNickName,
            ProcessedByFullName = requestInfo.ProcessedByFullName,
            ProcessedDate = requestInfo.ProcessedDate,
            FromShift = new Shift
            {
                Id = requestInfo.FromShiftId ?? 0,
                Name = requestInfo.FromShiftName ?? string.Empty
            },
            RequestedDate = requestInfo.RequestedDate ?? DateTimeOffset.Now.DateTime,
            TotalMins = requestInfo.TotalMinutes ?? 0,
            ActualLateInEarlyOut = requestInfo.ActualInOutMins ?? 0,
            CrossDay = (requestInfo.FromTime == null || requestInfo.ToTime == null) ?? requestInfo.FromTime.DayOfYear > requestInfo.ToTime.DayOfYear,
            ToShift = new Shift
            {
                Id = requestInfo.ToShiftId ?? 0,
                Name = requestInfo.ToShiftName ?? string.Empty
            },
            ChangeShiftType = (byte)(requestInfo.ChangeShiftType ?? 0),
            IsWarning = requestInfo.IsWarning ?? false,
            ModifiedDate = requestInfo.ModifiedDate,
            VerifiedStatus = requestInfo.VerifiedStatus ?? 0,
            ApprovedStatus = requestInfo.ApprovedStatus ?? 0,
            ProcessedStatus = requestInfo.ProcessedStatus ?? 0,
            ReworkCount = requestInfo.ReworkCount ?? 0
        };

        return entity;
    }

    private static Employee GetRequester(dynamic requestInfo)
    {
        return new Employee
        {
            Id = requestInfo.RequesterId,
            NickName = requestInfo.RequesterNickName,
            FullName = requestInfo.RequesterFullName,
            AnnualLeaveBalance = requestInfo.AnnualLeaveBalance,
            UnitCode = requestInfo.UnitCode,
            Team = new Team
            {
                Id = requestInfo.TeamId,
                Code = requestInfo.TeamCode,
                Name = requestInfo.TeamName,
                Department = new Department
                {
                    Id = requestInfo.DepartmentId,
                    Code = requestInfo.DepartmentCode,
                    Name = requestInfo.DepartmentName
                }
            }
        };
    }
}