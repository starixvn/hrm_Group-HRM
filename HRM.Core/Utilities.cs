namespace HRM.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Enums;
using Resources;
using Microsoft.AspNetCore.Mvc.Rendering;

[SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
public static class Utilities
{
    private const byte PaddingLeftWidth = 2;

    public static IList<SelectListItem> CheckInOutTypes =>
        new List<SelectListItem>
        {
            new() { Text = Labels.CheckIn, Value = ((short)RequestType.CheckIn).ToString() },
            new() { Text = Labels.CheckOut, Value = ((short)RequestType.CheckOut).ToString() }
        };

    public static IList<SelectListItem> LateInEarlyOutTypes =>
        new List<SelectListItem>
        {
            new() { Text = Labels.LateIn, Value = ((short)RequestType.LateIn).ToString() },
            new() { Text = Labels.EarlyOut, Value = ((short)RequestType.EarlyOut).ToString() }
        };

    public static IList<SelectListItem> Marriages =>
        new List<SelectListItem>
        {
            new()
            {
                Text = Labels.SelectMarriageStatus,
                Value = ((byte)MarriageStatus.Default).ToString()
            },
            new()
            {
                Text = MarriageStatus.Married.ToString(),
                Value = ((byte)MarriageStatus.Married).ToString()
            },
            new()
            {
                Text = MarriageStatus.Divorced.ToString(),
                Value = ((byte)MarriageStatus.Divorced).ToString()
            },
            new()
            {
                Text = MarriageStatus.Single.ToString(),
                Value = ((byte)MarriageStatus.Single).ToString()
            },
            new()
            {
                Text = MarriageStatus.Widow.ToString(),
                Value = ((byte)MarriageStatus.Widow).ToString()
            }
        };

    public static string GetTime(DateTime? dateTime)
    {
        if (dateTime == null)
        {
            return string.Empty;
        }

        var value = (DateTime)dateTime;
        return string.Format(Labels.TimeFormat, value.Hour.ToString().PadLeft(PaddingLeftWidth, '0'), value.Minute.ToString().PadLeft(PaddingLeftWidth, '0'));
    }

    public static string ReadTextFile(string filePath)
    {
        using var sr = new StreamReader(filePath);
        return sr.ReadToEnd();
    }

    public static string RandomString(int length)
    {
        // returns random string of length specified with characters including: 0-9, a-z, A-Z
        var ca = new char[length];
        var random = new byte[length];

        RandomNumberGenerator.Fill(random);
        for (var i = 0; i < length; i++)
        {
            var found = false;
            var rand = (int)random[i];

            while (!found)
            {
                var randFrom48To57 = rand is >= 48 and <= 57;
                var randFrom65To90 = rand is >= 65 and <= 90;
                var randFrom97To122 = rand is >= 97 and <= 122;

                if (randFrom48To57 || randFrom65To90 || randFrom97To122)
                {
                    found = true;
                }
                else
                {
                    // get a new random int.
                    var single = new byte[1];
                    RandomNumberGenerator.Fill(single);
                    rand = single[0];
                }
            }

            var ci = (char)rand;
            ca[i] = ci;
        }

        return new string(ca).ToLowerInvariant();
    }

    public static IList<SelectListItem> TimeSteps(int minuteStep)
    {
        var list = new List<SelectListItem>();
        var hour = 0;
        var minute = 0;

        while (hour < 24)
        {
            var value = string.Format(Labels.TimeFormat, hour.ToString().PadLeft(PaddingLeftWidth, '0'), minute.ToString().PadLeft(PaddingLeftWidth, '0'));
            list.Add(new SelectListItem
            {
                Text = value,
                Value = value
            });
            minute += minuteStep;
            if (minute >= 60)
            {
                minute = 0;
                hour++;
            }
        }

        return list;
    }

    public static IList<SelectListItem> TimeSteps(int minuteStep, string selectedItem)
    {
        var list = new List<SelectListItem>();
        var hour = 0;
        var minute = 0;

        while (hour < 24)
        {
            var value = string.Format(Labels.TimeFormat, hour.ToString().PadLeft(PaddingLeftWidth, '0'), minute.ToString().PadLeft(PaddingLeftWidth, '0'));
            list.Add(new SelectListItem
            {
                Text = value,
                Value = value,
                Selected = value == selectedItem
            });
            minute += minuteStep;
            if (minute >= 60)
            {
                minute = 0;
                hour++;
            }
        }

        return list;
    }

    public static IList<SelectListItem> LateInOutTimeSteps(int minuteStep)
    {
        var list = new List<SelectListItem>();
        var hour = 0;
        var minute = 15;

        while (hour < 1)
        {
            var value = minute.ToString().PadLeft(PaddingLeftWidth, '0');
            list.Add(new SelectListItem
            {
                Text = value,
                Value = value
            });
            minute += minuteStep;
            if (minute > 150)
            {
                hour++;
            }
        }

        return list;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        return regex.IsMatch(email);
    }

    public static string TotalHoursToTime(float value)
    {
        var hour = (int)value;
        var minute = (int)Math.Round((value - hour) * 60);

        return hour > 0
            ? hour.ToString().PadLeft(PaddingLeftWidth, '0') + "h" + minute.ToString().PadLeft(PaddingLeftWidth, '0') + "'"
            : minute.ToString().PadLeft(PaddingLeftWidth, '0') + "'";
    }

    public static string MinuteToTime(int value)
    {
        if (value == 0)
        {
            return string.Empty;
        }

        var hour = value / 60;
        var minute = value - hour * 60;

        return hour > 0
            ? hour.ToString().PadLeft(PaddingLeftWidth, '0') + "h" + minute.ToString().PadLeft(PaddingLeftWidth, '0') + "'"
            : minute.ToString().PadLeft(PaddingLeftWidth, '0') + "'";
    }

    public static string FormatString(string input)
    {
        return string.IsNullOrEmpty(input) ? string.Empty : input;
    }

    public static bool CheckUserCanEditRequest(int currentUserId, int employeeId)
    {
        return currentUserId == employeeId;
    }

    public static string GetCurrentStatusName(int statusId)
    {
	    var statusName = statusId switch
	    {
		    (int)CurrentRequestStatus.Pending => Labels.Pending,
		    (int)CurrentRequestStatus.Verified => Labels.Verified,
		    (int)CurrentRequestStatus.NotVerified => Labels.NotVerified,
		    (int)CurrentRequestStatus.Approved => Labels.Approved,
		    (int)CurrentRequestStatus.Rejected => Labels.Rejected,
		    (int)CurrentRequestStatus.Processed => Labels.Processed,
		    (int)CurrentRequestStatus.Void => Labels.Void,
		    _ => Labels.NA
	    };

	    return statusName;
    }

    public static IList<SelectListItem> GetDateRangeByYear()
    {
        var dateRange = new List<SelectListItem> { new() { Text = Labels.DateRange, Value = "0" } };
        var currentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        dateRange.Add(new SelectListItem
        {
            Text = Labels.ThisYear,
            Value = currentDate.Year.ToString()
        });

        dateRange.Add(new SelectListItem
        {
            Text = Labels.LastYear,
            Value = (currentDate.Year - 1).ToString()
        });

        return dateRange;
    }

    public static string DisplayEmptyFloatNumber(float number)
    {
        return number <= 0 ? "-" : number.ToString("0.0");
    }

    public static string DisplayFloatNumber(float number)
    {
        return number.ToString("0.0");
    }

    public static string GetBackgroundColorForLeave(int leaveTypeId)
    {
	    var backgroundColor = leaveTypeId switch
	    {
		    (byte)LeaveTypes.AnnualLeave => "#c7f391",
		    (byte)LeaveTypes.UnpaidLeave => "#eacded",
		    (byte)LeaveTypes.BusinessTrip => "#c1d6ed",
		    (byte)LeaveTypes.WeddingLeave or (byte)LeaveTypes.WeddingOfEmployeeChildren => "#7cd3e9",
		    _ => "#a9a7a7"
	    };

	    return backgroundColor;
    }

    public static string GetMessageDateString(DateTime fromDate, DateTime toDate)
    {
        string result;

        if ((toDate - fromDate).Days <= 0)
        {
            result = fromDate.ToDateMonthYearFormatedDate();
        }
        else if (fromDate.Year != toDate.Year)
        {
            result = fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
        }
        else
        {
            result = fromDate.Month != toDate.Month ?
                fromDate.ToMonthAndDayFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate() :
                fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();
        }

        return result;
    }
}