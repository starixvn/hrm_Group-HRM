namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Core.Services;
    using HRM.Criteria.Report;
    using HRM.Models.Report;

    public interface IShiftStatisticReportService
    {
        IEnumerable<ShiftStatistic> GetShiftStatistic(GetShiftStatisticParam param);

        IEnumerable<ShiftStatisticDetail> GetShiftStatisticDetail(GetShiftStatisticDetailParam param);
    }

    public class ShiftStatisticReportService : IShiftStatisticReportService
    {
        private readonly IBaseService baseService;

        public ShiftStatisticReportService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<ShiftStatistic> GetShiftStatistic(GetShiftStatisticParam param)
        {
            var criteria = new GetShiftStatisticCriteria
            {
                SupervisorId = param.SupervisorId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                TeamIds = param.TeamIds,
                IsLocal = param.IsLocal,
                IsShowWholeCompany = param.IsShowWholeCompany
            };
            return baseService.GetData<ShiftStatistic>(criteria);
        }

        public IEnumerable<ShiftStatisticDetail> GetShiftStatisticDetail(GetShiftStatisticDetailParam param)
        {
            var criteria = new GetShiftStatisticDetailCriteria
            {
                SupervisorId = param.SupervisorId,
                EmployeeId = param.EmployeeId,
                WorkDate = param.WorkDate,
                ShiftId = param.ShiftId,
                TeamIds = param.TeamIds,
                IsLocal = param.IsLocal,
                IsShowWholeCompany = param.IsShowWholeCompany
            };
            return baseService.GetData<ShiftStatisticDetail>(criteria);
        }
    }
}