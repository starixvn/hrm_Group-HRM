using System.Collections.Generic;
using System.Globalization;
using System.Text;
using HRM.Core.Enums;
using HRM.Models;
using HRM.Resources;

namespace HRM.Website.Helper;

public static class RazorHelpers
{
    public static string CategoryDropDownList(string id, string classHTML = "", string selectedId = "")
    {
        return $"""
                <select id='{id}' name='{id}' class='{classHTML}'>
                    <option value='' {(string.IsNullOrWhiteSpace(selectedId) ? "selected" : "")}>-- All --</option>
                    <option value='1' {(selectedId == "1" ? "selected" : "")}>{Labels.Local}</option>
                    <option value='0' {(selectedId == "0" ? "selected" : "")}>{Labels.Expat}</option>
                </select>
            """;
    }

    public static string UserStatusDropDownList(string id, string classHTML = "", byte? selectedId = (byte)EmployeeStatus.Enabled)
    {
        return $"""
                <select id='{id}' name='{id}' class='{classHTML}'>
                    <option {(!selectedId.HasValue ? "selected" : "")} value=''>-- All --</option>
                    <option {(selectedId == (byte)EmployeeStatus.Enabled ? "selected" : "")} value='{(byte)EmployeeStatus.Enabled}'>{Labels.Enable}</option>
                    <option {(selectedId == (byte)EmployeeStatus.Disabled ? "selected" : "")} value='{(byte)EmployeeStatus.Disabled}'>{Labels.Disabled}</option>
                    <option {(selectedId == (byte)EmployeeStatus.Deleted ? "selected" : "")} value='{(byte)EmployeeStatus.Deleted}'>{Labels.Deleted}</option>
                </select>
            """;
    }

    public static string DisplayNoInformation(int colspan)
    {
        return $"""
                <tr>
                    <td colspan='{colspan}'>
                        {Messages.NoInformation}
                    </td>
                </tr>
            """;
    }

    public static string StatusDropDownList(
        int status = (int)CurrentRequestStatus.Pending,
        bool isShowAll = true,
        bool isShowPendingAndVerified = true,
        string controlName = "status",
        string htmlClass = "")
    {
        return $"""
                    <select name='{controlName}' id='{controlName}' class='{htmlClass}'>
                        {(isShowAll ? $@"<option {getSelectedAttribute(0)} value='0'>All</option>" : "")}
                        {(isShowPendingAndVerified ? $@"<option {getSelectedAttribute(-1)} value='-1'>Pending & Verified</option>" : "")}
                        <option {getSelectedAttribute((byte)CurrentRequestStatus.Pending)} value='{(byte)CurrentRequestStatus.Pending}'>Pending</option>
                        <option {getSelectedAttribute((byte)CurrentRequestStatus.Verified)} value='{(byte)CurrentRequestStatus.Verified}'>Verified</option>
                        <option {getSelectedAttribute((byte)CurrentRequestStatus.NotVerified)} value='{(byte)CurrentRequestStatus.NotVerified}'>Not Verified</option>
                        <option {getSelectedAttribute((byte)CurrentRequestStatus.Approved)} value='{(byte)CurrentRequestStatus.Approved}'>Approved</option>
                        <option {getSelectedAttribute((byte)CurrentRequestStatus.Rejected)} value='{(byte)CurrentRequestStatus.Rejected}'>Rejected</option>
                        <option {getSelectedAttribute((byte)CurrentRequestStatus.Processed)} value='{(byte)CurrentRequestStatus.Processed}'>Processed</option>
                        <option {getSelectedAttribute((byte)CurrentRequestStatus.Void)} value='{(byte)CurrentRequestStatus.Void}'>Void</option>
                    </select>
                """;

        string getSelectedAttribute(int value) => status == value ? "selected" : string.Empty;
    }

    public static string LateInEarlyOutTimeFormat(int mins, short type)
    {
        return type == (short)RequestType.LateIn ? $"+{mins}'" : $"-{mins}'";
    }

    public static string MinTotalLeaveDropDownList(int minTotalLeave, string name = "MinTotalLeave")
    {
        var options = new StringBuilder();
        options.Append($"<option {(minTotalLeave == 0 ? "selected" : "")} value='0'>All</option>");
        for (var i = 14; i <= 20; i++)
        {
            options.Append($"<option {(i == minTotalLeave ? "selected" : "")} value='{i}'>{i}</option>");
        }
        return $"""
                <select id='{name}' name='{name}' class='minWidthDropdown form-control'>
                    {options}
                </select>
            """;
    }

    public static string MessageCategoryDropDownList(short action = 0, string controlName = "", string classObject = "")
    {
        return $"""
                <select id='{controlName}'>
                    <option {(action == 0 ? "selected" : "")} value='{(byte)MessageCategory.Notification}'>{Labels.Notification}</option>
                    <option {(action == 1 ? "selected" : "")} value='{(byte)MessageCategory.Warning}'>{Labels.Warning}</option>
                    <option {(action == 2 ? "selected" : "")} value='{(byte)MessageCategory.Violation}'>{Labels.Violation}</option>
                </select>
            """;
    }

    public static string MessageStatusDropDownList(short action = -1, string controlName = "", string classObject = "")
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"<select id='{controlName}'>");
        sb.AppendLine($@"<option {(action == -1 ? "selected" : "")} value=''>-- All --</option>");
        sb.AppendLine($@"<option {(action == 0 ? "selected" : "")} value='0'>Active</option>");
        sb.AppendLine($@"<option {(action == 1 ? "selected" : "")} value='1'>Disabled</option>");
        sb.AppendLine("</select>");
        return sb.ToString();
    }

    public static string LeaveTypeDropDownList(string id, IList<LeaveType> leaveTypes, string className, int? currentValue)
    {
        var value = currentValue != null ? currentValue.ToString() : string.Empty;
        var sb = new StringBuilder();
        sb.AppendLine($@"<select id='{id}' class='{className}'>");
        foreach (var leaveType in leaveTypes)
        {
            var selected = leaveType.Id.ToString() == value ? "selected" : "";
            sb.AppendLine($@"<option value='{leaveType.Id}' {selected} title='{leaveType.Description}'>{leaveType.Name}</option>");
        }

        sb.AppendLine("</select>");
        return sb.ToString();
    }

    public static string DisplayZeroCell(decimal value)
    {
        return value > 0 ? value.ToString(CultureInfo.InvariantCulture) : "-";
    }

    public static string CheckInOutReasonDropDownList(byte reasonId = (byte)CheckInOutReason.Forgot, string controlName = "reasonId", string htmlClass = "")
    {
        return $"""
                <select name='{controlName}' id='{controlName}' class='{htmlClass}'>
                    <option {(reasonId == (byte)CheckInOutReason.VanLate ? "selected" : "")} value='{(byte)CheckInOutReason.VanLate}'>{Labels.VanLate}</option>
                    <option {(reasonId == (byte)CheckInOutReason.BusinessTrip ? "selected" : "")} value='{(byte)CheckInOutReason.BusinessTrip}'>{Labels.BusinessTrip}</option>
                    <option {(reasonId == (byte)CheckInOutReason.Forgot ? "selected" : "")} value='{(byte)CheckInOutReason.Forgot}'>{Labels.Forgot}</option>
                    <option {(reasonId == (byte)CheckInOutReason.Other ? "selected" : "")} value='{(byte)CheckInOutReason.Other}'>{Labels.Other}</option>
                </select>
            """;
    }

    public static string LateInEarlyOutReasonDropDownList(byte reasonId = (byte)LateInEarlyOutReason.Other, string controlName = "reasonId", string htmlClass = "")
    {
        return $"""
                <select name='{controlName}' id='{controlName}' class='{htmlClass}'>
                    <option {(reasonId == (byte)LateInEarlyOutReason.ManagementAllow ? "selected" : "")} value='{(byte)LateInEarlyOutReason.ManagementAllow}'>{Labels.ManagementAllow}</option>
                    <option {(reasonId == (byte)LateInEarlyOutReason.Other ? "selected" : "")} value='{(byte)LateInEarlyOutReason.Other}'>{Labels.Other}</option>
                </select>
            """;
    }

    public static string FormatThousandSeparator(string value)
    {
       var formattedValue = decimal.TryParse(value, out var salaryValue) 
            ? salaryValue.ToString("#,##0.##")
            : value;
        return formattedValue;
    }

    public static string WorkFromHomeTypeDropDownList(string id, string classHTML = "", string selectedId = "")
    {
        return $"""
                <select id='{id}' name='{id}' class='{classHTML}'>
                    <option value='' {(string.IsNullOrWhiteSpace(selectedId) ? "selected" : "")}>All</option>
                    <option value='0' {(selectedId == "0" ? "selected" : "")}>Fixed-period</option>
                    <option value='1' {(selectedId == "1" ? "selected" : "")}>Permanent</option>
                </select>
            """;
    }
}
