namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Shift;
    using HRM.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ShiftService : IShiftService
    {
        private readonly IBaseService baseService;

        public ShiftService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IList<Shift>> GetAllAsync(int statusId = 1, bool isShowOff = false)
        {
            var shifts = (await baseService.GetDataAsync<Shift>(new GetAllShiftCriteria())).ToList();

            if (statusId == (int)ShiftStatus.Active)
            {
                shifts = shifts.Where(t => !t.IsDisabled).ToList();
            }

            if (statusId == (int)ShiftStatus.Disabled)
            {
                shifts = shifts.Where(t => t.IsDisabled).ToList();
            }

            if (isShowOff)
            {
                shifts.Insert(0, new Shift { Code = "Off", Id = -1 });
            }

            return shifts;
        }

        public async Task<IList<Shift>> GetByQueryAsync(string query, bool isShowOff)
        {
            var shifts = await GetAllAsync(isShowOff: isShowOff);
            IList<Shift> queriedShifts = null;

            if (shifts != null && shifts.Any())
            {
                queriedShifts = shifts.Where(t => t.Code.ToUpperInvariant().StartsWith(query.ToUpperInvariant(), StringComparison.CurrentCulture)).ToList();
            }

            return queriedShifts ?? new List<Shift>();
        }

        public async Task<IList<SelectListItem>> GetSelectListAsync(int statusId = 1, bool isShowOff = false)
        {
            var list = new List<SelectListItem>();
            var shifts = await GetAllAsync(statusId, isShowOff);

            if (shifts.Any())
            {
                list = shifts.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            }

            return list;
        }

        public async Task<Shift> GetByDateAsync(int employeeId, DateTime date)
        {
            var criteria = new GetEmployeeShiftByDateCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            return await baseService.GetDataEntityAsync<Shift>(criteria);
        }

        public async Task<Shift> GetByIdAsync(int shiftId)
        {
            var criteria = new GetShiftByIdCriteria { ShiftId = shiftId };
            return await baseService.GetDataEntityAsync<Shift>(criteria);
        }

        public async Task<CommandResult> CreateAsync(int actorId, CreateShiftModel model)
        {
            var criteria = new CreateShiftCriteria
            {
                Name = model.Name,
                CheckInTime = model.TimeIn,
                CheckOutTime = model.TimeOut,
                IsInAfterDay = model.IsInAfterDay,
                IsOutAfterDay = model.IsOutAfterDay,
                ActorId = actorId
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> EditAsync(int actorId, EditShiftModel model)
        {
            var criteria = new EditShiftCriteria
            {
                ShiftId = model.ShiftId,
                Name = model.Name,
                CheckInTime = model.TimeIn,
                CheckOutTime = model.TimeOut,
                IsInAfterDay = model.IsInAfterDay,
                IsOutAfterDay = model.IsOutAfterDay,
                IsDisabled = model.IsDisabled,
                ActorId = actorId
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> DeleteAsync(int shiftId)
        {
            var criteria = new DeleteShiftCriteria { ShiftId = shiftId };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}