namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Core.Enums;
    using HRM.Models.API.Request;

    public class APITodolistRequestRepository : IAPITodolistRequestRepository
    {
        public IList<RequestEntity> GetPendingRequestByUserId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var requests = multi.Read<RequestEntity>();
                    var leaveDetails = multi.Read<GeneralLeaveDetail>();
                    var changeShiftDetails = multi.Read<ChangeShiftDetail>();

                    foreach (RequestEntity request in requests)
                    {
                        if (request.RequestTypeId == (byte)RequestType.Leave)
                        {
                            request.LeaveDetails = leaveDetails.Where(x => x.RequestId == request.RequestId).ToList();
                        }

                        if (request.RequestTypeId == (byte)RequestType.ChangeShift)
                        {
                            request.ChangeShiftDetails = changeShiftDetails.Where(x => x.RequestId == request.RequestId).ToList();
                        }
                    }

                    return requests.ToList();
                }
            }
        }
    }
}