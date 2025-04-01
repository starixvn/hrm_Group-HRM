using HRM.Models.Shifts;

namespace HRM.ViewModels.Shifts;

using System.Collections.Generic;
using Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ShiftByStatusViewModel
{
    public int StatusId { get; set; }

    public IList<Shift> Shifts { get; set; }

    public IList<SelectListItem> Status
    {
        get
        {
            var status = new List<SelectListItem>
            {
                new() { Value = ((int)ShiftStatus.All).ToString(), Text = ShiftStatus.All.ToString() },
                new() { Value = ((int)ShiftStatus.Active).ToString(), Text = ShiftStatus.Active.ToString() },
                new() { Value = ((int)ShiftStatus.Disabled).ToString(), Text = ShiftStatus.Disabled.ToString() }
            };

            return status;
        }
    }
}