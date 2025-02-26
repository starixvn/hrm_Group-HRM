namespace HRM.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Core.Enums;
    using HRM.Criteria.Request;
    using HRM.Models;
    using HRM.Models.Request;
    using HRM.Models.Request.ChangeShift;
    using HRM.Models.Supervisor;

    public class RequestRepository : IRequestRepository
    {
        public IList<RequestEntity> GetRequests(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var entityList = FillData(multi.Read<dynamic>());
                    var changeOffDateDetails = multi.Read<ChangeShiftDetail>();

                    foreach (var request in entityList.Where(entity => entity.RequestType.Id == (byte)RequestType.ChangeShift))
                    {
                        request.ChangeShiftDetails = changeOffDateDetails.Where(detail => detail.RequestID == request.Id);
                    }

                    return entityList;
                }
            }
        }

        public RequestRelatedPeopleModel GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria)
        {
            RequestRelatedPeopleModel relatedPeople = null;
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    relatedPeople = new RequestRelatedPeopleModel
                    {
                        Requester = multi.ReadFirstOrDefault<SupervisorNameModel>(),
                        Verifier = multi.ReadFirstOrDefault<SupervisorNameModel>(),
                        Approver = multi.ReadFirstOrDefault<SupervisorNameModel>(),
                        ListHR = multi.Read<SupervisorNameModel>().ToList(),
                        EmailSetting = multi.ReadFirstOrDefault<EmailSettingsModel>()
                    };
                }
            }

            return relatedPeople;
        }

        public async Task<VerifierAndApproverForRequest> GetVerifierAndApproverAsync(ICriteria criteria)
        {
            VerifierAndApproverForRequest supervisor = null;
            using (IObjectDb db =  ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    supervisor = new VerifierAndApproverForRequest
                    {
                        Verifiers = multi.Read<SupervisorNameModel>().ToList(),
                        Approvers = multi.Read<SupervisorNameModel>().ToList()
                    };
                }
            }

            return supervisor;
        }

        public RequestEntity GetRequestToIgnoreById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<RequestEntity, Shift, Employee, RequestTypeModel, RequestEntity>(
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
        }

        public IList<LeaveDisciplineModel> GetLeaveRequestToReviewDiscipline(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var disciplines = multi.Read<LeaveDisciplineModel>().ToList();
                    var leaveDetails = multi.Read<LeaveDisciplineDetailModel>().ToList();
                    foreach (var discipline in disciplines)
                    {
                        discipline.LeaveDetail = leaveDetails.Where(t => t.WorkDate == discipline.WorkDate).ToList();
                    }

                    return disciplines;
                }
            }
        }

        private IList<RequestEntity> FillData(dynamic data)
        {
            var entityList = new List<RequestEntity>();
            foreach (var request in data)
            {
                RequestEntity entity = FillDataEntity(request);
                entityList.Add(entity);
            }

            return entityList;
        }

        [SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        private RequestEntity FillDataEntity(dynamic requestInfo)
        {
            var entity = new RequestEntity();

            entity.Id = requestInfo.RequestId;
            entity.RequestType = new RequestTypeModel { Id = (short)requestInfo.RequestType };
            entity.Status = new RequestStatus { Id = requestInfo.Status };
            entity.FromDate = requestInfo.FromDate;
            entity.ToDate = requestInfo.ToDate;
            entity.TotalDays = requestInfo.TotalDays;
            entity.IsHalfDay = requestInfo.IsHalfDay ?? false;
            entity.FromTime = requestInfo.FromTime ?? DateTimeOffset.Now.DateTime;
            entity.ToTime = requestInfo.ToTime ?? DateTimeOffset.Now.DateTime;
            entity.KeepInViewType = (byte)(requestInfo.KeepInViewType ?? 0);
            entity.LeaveCountDetails = requestInfo.LeaveCountDetails ?? string.Empty;
            entity.LeaveType = new LeaveType
            {
                Id = requestInfo.LeaveTypeId ?? 0,
                Name = requestInfo.LeaveTypeName ?? string.Empty
            };
            entity.Remark = requestInfo.Remark ?? string.Empty;
            entity.TotalDaysRequestInMonth = requestInfo.TotalDaysRequestInMonth ?? 0;
            entity.CreatedDate = requestInfo.CreatedDate;
            entity.ReasonId = (byte)requestInfo.ReasonId;
            entity.RefNO = requestInfo.RefNo ?? string.Empty;
            entity.ApprovedDate = requestInfo.ApprovedDate;
            entity.VerifiedDate = requestInfo.VerifiedDate;
            entity.ProcessedDate = requestInfo.ProcessedDate;
            entity.Requester = GetRequester(requestInfo);
            entity.ModifiedBy = new Employee
            {
                Id = requestInfo.ModifiedById ?? 0,
                NickName = requestInfo.ModifiedByNickName ?? string.Empty,
                FullName = requestInfo.ModifiedByFullName ?? string.Empty
            };
            entity.VerifiedBy = new Employee
            {
                Id = requestInfo.VerifierId ?? 0,
                NickName = requestInfo.VerifierNickName ?? string.Empty,
                FullName = requestInfo.VerifierFullName ?? string.Empty
            };
            entity.Approver = new Employee
            {
                Id = requestInfo.ApproverId ?? 0,
                NickName = requestInfo.ApproverNickName ?? string.Empty,
                FullName = requestInfo.ApproverFullName ?? string.Empty
            };
            entity.ApprovedBy = new Employee
            {
                Id = requestInfo.ApprovedId ?? 0,
                NickName = requestInfo.ApprovedNickName ?? string.Empty,
                FullName = requestInfo.ApprovedFullName ?? string.Empty
            };
            entity.ProcessedBy = new Employee
            {
                Id = requestInfo.ProcessedId ?? 0,
                NickName = requestInfo.ProcessedNickName ?? string.Empty,
                FullName = requestInfo.ProcessedFullName ?? string.Empty
            };
            entity.FromShift = new Shift
            {
                Id = requestInfo.FromShiftId ?? 0,
                Name = requestInfo.FromShiftName ?? string.Empty
            };
            entity.RequestedDate = requestInfo.RequestedDate ?? DateTimeOffset.Now.DateTime;
            entity.TotalMins = requestInfo.TotalMinutes ?? 0;
            entity.ActualLateInEarlyOut = requestInfo.ActualInOutMins ?? 0;
            entity.CrossDay = (requestInfo.FromTime == null || requestInfo.ToTime == null) ??
                requestInfo.FromTime.DayOfYear > requestInfo.ToTime.DayOfYear;
            entity.ToShift = new Shift
            {
                Id = requestInfo.ToShiftId ?? 0,
                Name = requestInfo.ToShiftName ?? string.Empty
            };
            entity.ChangeShiftType = (byte)(requestInfo.ChangeShiftType ?? 0);
            entity.IsWarning = requestInfo.IsWarning ?? false;
            entity.ModifiedDate = requestInfo.ModifiedDate;
            entity.VerifiedStatus = requestInfo.VerifiedStatus ?? 0;
            entity.ApprovedStatus = requestInfo.ApprovedStatus ?? 0;
            entity.ProcessedStatus = requestInfo.ProcessedStatus ?? 0;
            entity.ReworkCount = requestInfo.ReworkCount ?? 0;

            return entity;
        }

        private Employee GetRequester(dynamic requestInfo)
        {
            return new Employee
            {
                Id = requestInfo.RequesterId,
                NickName = requestInfo.RequesterNickName,
                FullName = requestInfo.RequesterFullName,
                AnnualLeaveBalance = requestInfo.AnnualLeaveBalance,
                Team = new Team
                {
                    Id = requestInfo.TeamId,
                    Code = requestInfo.TeamCode,
                    Name = requestInfo.TeamName,
                    Section = new Section
                    {
                        Id = requestInfo.SectionId,
                        Code = requestInfo.SectionCode,
                        Department = new Department
                        {
                            Id = requestInfo.DepartmentId,
                            Code = requestInfo.DepartmentCode
                        }
                    }
                }
            };
        }
    }
}