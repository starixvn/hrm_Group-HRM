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
                    <option selected='{string.IsNullOrWhiteSpace(selectedId)}' value=''>-- All --</option>
                    <option selected='{selectedId == "1"}' value='1'>{Labels.Local}</option>
                    <option selected='{selectedId == "0"}' value='0'>{Labels.Expat}</option>
                </select>";
        }

        public static string UserStatusDropDownList(string id, string classHTML = "", byte? selectedId = (byte)EmployeeStatus.Enabled)
        {
            return $@"
                <select id='{id}' name='{id}' class='{classHTML}'>
                    <option selected='{!selectedId.HasValue}' value=''>-- All --</option>
                    <option selected='{selectedId == (byte)EmployeeStatus.Enabled}' value='{(byte)EmployeeStatus.Enabled}'>{Labels.Enable}</option>
                    <option selected='{selectedId == (byte)EmployeeStatus.Disabled}' value='{(byte)EmployeeStatus.Disabled}'>{Labels.Disabled}</option>
                    <option selected='{selectedId == (byte)EmployeeStatus.Deleted}' value='{(byte)EmployeeStatus.Deleted}'>{Labels.Deleted}</option>
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
                    {(isShowAll ? $"<option selected='{status == 0}' value='0'>All</option>" : "")}
                    {(isShowPendingAndVerified ? $"<option selected='{status == -1}' value='-1'>Pending & Verified</option>" : "")}
                    <option selected='{status == (byte)CurrentRequestStatus.Pending}' value='{(byte)CurrentRequestStatus.Pending}'>{Labels.Pending}</option>
                    <option selected='{status == (byte)CurrentRequestStatus.Verified}' value='{(byte)CurrentRequestStatus.Verified}'>{Labels.Verified}</option>
                    <option selected='{status == (byte)CurrentRequestStatus.NotVerified}' value='{(byte)CurrentRequestStatus.NotVerified}'>{Labels.NotVerified}</option>
                    <option selected='{status == (byte)CurrentRequestStatus.Approved}' value='{(byte)CurrentRequestStatus.Approved}'>{Labels.Approved}</option>
                    <option selected='{status == (byte)CurrentRequestStatus.Rejected}' value='{(byte)CurrentRequestStatus.Rejected}'>{Labels.Rejected}</option>
                    <option selected='{status == (byte)CurrentRequestStatus.Processed}' value='{(byte)CurrentRequestStatus.Processed}'>{Labels.Processed}</option>
                    <option selected='{status == (byte)CurrentRequestStatus.Void}' value='{(byte)CurrentRequestStatus.Void}'>{Labels.Void}</option>
                </select>";
        }

        public static string LateInEarlyOutTimeFormat(int mins, short type)
        {
            return type == (short)RequestType.LateIn ? $"+{mins}'" : $"-{mins}'";
        }

        public static string MinTotalLeaveDropDownList(int minTotalLeave, string name = "MinTotalLeave")
        {
            var options = "<option selected='{minTotalLeave == 0}' value='0'>All</option>";
            for (int i = 14; i <= 20; i++)
            {
                options += $"<option selected='{i == minTotalLeave}' value='{i}'>{i}</option>";
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
