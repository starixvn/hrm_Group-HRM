using HRM.Core.Enums;
using HRM.Resources;
using HRM.Models;
using System.Collections.Generic;

namespace HRM.Website.Helper
{
    public static class RazorHelpers
    {
        public static string CategoryDropDownList(string id, string classHTML = "", string selectedId = "")
        {
            return $@"
                <select id='{id}' name='{id}' class='{classHTML}'>
                    <option value='' {(string.IsNullOrWhiteSpace(selectedId) ? "selected" : "")}>-- All --</option>
                    <option value='1' {(selectedId == "1" ? "selected" : "")}>{Labels.Local}</option>
                    <option value='0' {(selectedId == "0" ? "selected" : "")}>{Labels.Expat}</option>
                </select>";
        }

        public static string UserStatusDropDownList(string id, string classHTML = "", byte? selectedId = (byte)EmployeeStatus.Enabled)
        {
            return $@"
                <select id='{id}' name='{id}' class='{classHTML}'>
                    <option {(!selectedId.HasValue ? "selected" : "")} value=''>-- All --</option>
                    <option {(selectedId == (byte)EmployeeStatus.Enabled ? "selected": "")} value='{(byte)EmployeeStatus.Enabled}'>{Labels.Enable}</option>
                    <option {(selectedId == (byte)EmployeeStatus.Disabled ? "selected": "")} value='{(byte)EmployeeStatus.Disabled}'>{Labels.Disabled}</option>
                    <option {(selectedId == (byte)EmployeeStatus.Deleted ? "selected" : "")} value='{(byte)EmployeeStatus.Deleted}'>{Labels.Deleted}</option>
                </select>";
        }

        public static string DisplayNoInformation(int colspan)
        {
            return $@"
                <tr>
                    <td colspan='{colspan}'>
                        {Messages.NoInformation}
                    </td>
                </tr>";
        }

        public static string StatusDropDownList(int status = (int)CurrentRequestStatus.Pending, bool isShowAll = true, bool isShowPendingAndVerified = true, string controlName = "status", string htmlClass = "")
        {
            return $@"
                <select name='{controlName}' id='{controlName}' class='{htmlClass}'>
                    {(isShowAll ? $@"<option {(status == 0 ? "selected" : "")} value='0'>All</option>" : "")}
                    {(isShowPendingAndVerified ? $@"<option {(status == -1 ? "selected" : "")} value='-1'>Pending & Verified</option>" : "")}
                    <option {(status == (byte)CurrentRequestStatus.Pending ? "selected" : "")} value='{(byte)CurrentRequestStatus.Pending}'>Pending</option>
                    <option {(status == (byte)CurrentRequestStatus.Verified ? "selected" : "")} value='{(byte)CurrentRequestStatus.Verified}'>Verified</option>
                    <option {(status == (byte)CurrentRequestStatus.NotVerified ? "selected" : "")} value='{(byte)CurrentRequestStatus.NotVerified}'>Not Verified</option>
                    <option {(status == (byte)CurrentRequestStatus.Approved ? "selected" : "")} value='{(byte)CurrentRequestStatus.Approved}'>Approved</option>
                    <option {(status == (byte)CurrentRequestStatus.Rejected ? "selected" : "")} value='{(byte)CurrentRequestStatus.Rejected}'>Rejected</option>
                    <option {(status == (byte)CurrentRequestStatus.Processed ? "selected" : "")} value='{(byte)CurrentRequestStatus.Processed}'>Processed</option>
                    <option {(status == (byte)CurrentRequestStatus.Void ? "selected" : "")} value='{(byte)CurrentRequestStatus.Void}'>Void</option>
                </select>";
        }

        public static string LateInEarlyOutTimeFormat(int mins, short type)
        {
            return type == (short)RequestType.LateIn ? $"+{mins}'" : $"-{mins}'";
        }

        public static string MinTotalLeaveDropDownList(int minTotalLeave, string name = "MinTotalLeave")
        {
            var options = $"<option {(minTotalLeave == 0 ? "selected" : "")} value='0'>All</option>";
            for (int i = 14; i <= 20; i++)
            {
                options += $"<option {(i == minTotalLeave ? "selected" : "")} value='{i}'>{i}</option>";
            }
            return $@"
                <select id='{name}' name='{name}' class='minWidthDropdown form-control'>
                    {options}
                </select>";
        }

        public static string MessageCategoryDropDownList(short action = 0, string controlName = "", string classObject = "")
        {
            return $@"
                <select id='{controlName}'>
                    <option {(action == 0 ? "selected" : "")} value='{(byte)MessageCategory.Notification}'>{Labels.Notification}</option>
                    <option {(action == 1 ? "selected" : "")} value='{(byte)MessageCategory.Warning}'>{Labels.Warning}</option>
                    <option {(action == 2 ? "selected" : "")} value='{(byte)MessageCategory.Violation}'>{Labels.Violation}</option>
                </select>";
        }

        public static string MessageStatusDropDownList(short action = -1, string controlName = "", string classObject = "")
        {
            return $@"
                <select id='{controlName}'>
                    <option {(action == -1 ? "selected" : "")} value=''>-- All --</option>
                    <option {(action == 0 ? "selected" : "")} value='0'>Active</option>
                    <option {(action == 1 ? "selected" : "")} value='1'>Disabled</option>
                </select>";
        }

        public static string LeaveTypeDropDownList(string id, IList<LeaveType> leaveTypes, string className, int? currentValue)
        {
            var value = currentValue != null ? currentValue.ToString() : string.Empty;
            var options = "";
            foreach (var leaveType in leaveTypes)
            {
                var selected = leaveType.Id.ToString() == value ? "selected" : "";
                options += $"<option value='{leaveType.Id}' {selected} title='{leaveType.Description}'>{leaveType.Name}</option>";
            }
            return $@"
                <select id='{id}' class='{className}'>
                    {options}
                </select>";
        }

        public static string DisplayZeroCell(decimal value)
        {
            return value > 0 ? value.ToString() : "-";
        }

        public static string CheckInOutReasonDropDownList(byte reasonId = (byte)CheckInOutReason.Forgot, string controlName = "reasonId", string htmlClass = "")
        {
            return $@"
                <select name='{controlName}' id='{controlName}' class='{htmlClass}'>
                    <option {(reasonId == (byte)CheckInOutReason.VanLate ? "selected" : "")} value='{(byte)CheckInOutReason.VanLate}'>{Labels.VanLate}</option>
                    <option {(reasonId == (byte)CheckInOutReason.BusinessTrip ? "selected" : "")} value='{(byte)CheckInOutReason.BusinessTrip}'>{Labels.BusinessTrip}</option>
                    <option {(reasonId == (byte)CheckInOutReason.Forgot ? "selected" : "")} value='{(byte)CheckInOutReason.Forgot}'>{Labels.Forgot}</option>
                    <option {(reasonId == (byte)CheckInOutReason.Other ? "selected" : "")} value='{(byte)CheckInOutReason.Other}'>{Labels.Other}</option>
                </select>";
        }

        public static string LateInEarlyOutReasonDropDownList(byte reasonId = (byte)LateInEarlyOutReason.Other, string controlName = "reasonId", string htmlClass = "")
        {
            return $@"
                <select name='{controlName}' id='{controlName}' class='{htmlClass}'>
                    <option {(reasonId == (byte)LateInEarlyOutReason.ManagementAllow ? "selected" : "")} value='{(byte)LateInEarlyOutReason.ManagementAllow}'>{Labels.ManagementAllow}</option>
                    <option {(reasonId == (byte)LateInEarlyOutReason.Other ? "selected" : "")} value='{(byte)LateInEarlyOutReason.Other}'>{Labels.Other}</option>
                </select>";
        }
    }
}
