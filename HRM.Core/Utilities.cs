namespace HRM.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using HRM.Core.Enums;
    using HRM.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public static class Utilities
    {
        private const byte PaddingLeftWidth = 2;

        public static IList<SelectListItem> CheckInOutTypes
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem { Text = Labels.CheckIn, Value = ((short)RequestType.CheckIn).ToString() },
                        new SelectListItem { Text = Labels.CheckOut, Value = ((short)RequestType.CheckOut).ToString() }
                    };
            }
        }

        public static IList<SelectListItem> LateInEarlyOutTypes
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem { Text = Labels.LateIn, Value = ((short)RequestType.LateIn).ToString() },
                        new SelectListItem { Text = Labels.EarlyOut, Value = ((short)RequestType.EarlyOut).ToString() }
                    };
            }
        }

        public static IList<SelectListItem> Marriages
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Text = Labels.SelectMarriageStatus,
                            Value = ((byte)MarriageStatus.Default).ToString()
                        },
                        new SelectListItem
                        {
                            Text = MarriageStatus.Married.ToString(),
                            Value = ((byte)MarriageStatus.Married).ToString()
                        },
                        new SelectListItem
                        {
                            Text = MarriageStatus.Divorced.ToString(),
                            Value = ((byte)MarriageStatus.Divorced).ToString()
                        },
                        new SelectListItem
                        {
                            Text = MarriageStatus.Single.ToString(),
                            Value = ((byte)MarriageStatus.Single).ToString()
                        },
                        new SelectListItem
                        {
                            Text = MarriageStatus.Widow.ToString(),
                            Value = ((byte)MarriageStatus.Widow).ToString()
                        }
                    };
            }
        }

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
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static string RandomString(int length)
        {
            // returns random string of length specified with characters including: 0-9, a-z, A-Z
            var ca = new char[length];
            var random = new byte[length];

            // RNGCryptoServiceProvider is an implementation of a random number generator.
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(random); // The array is now filled with cryptographically strong random bytes.
                for (var i = 0; i < length; i++)
                {
                    var found = false;
                    var rand = (int)random[i];

                    while (!found)
                    {
                        bool randFrom48To57 = (rand >= 48) && (rand <= 57);
                        bool randFrom65To90 = (rand >= 65) && (rand <= 90);
                        bool randFrom97To122 = (rand >= 97) && (rand <= 122);

                        if (randFrom48To57 || randFrom65To90 || randFrom97To122)
                        {
                            found = true;
                        }
                        else
                        {
                            // get a new random int.
                            var single = new byte[1];
                            rng.GetBytes(single);
                            rand = single[0];
                        }
                    }

                    var ci = (char)rand;
                    ca[i] = ci;
                }

                return new string(ca).ToLowerInvariant();
            }
        }

        public static IList<SelectListItem> MinuteSteps(int step)
        {
            var list = new List<SelectListItem>();
            var index = 60;
            while (index > 0)
            {
                list.Add(new SelectListItem
                {
                    Text = string.Format(Labels.MinuteFormat, index),
                    Value = index.ToString()
                });
                index -= step;
            }

            return list;
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
            int hour = (int)value;
            int minute = (int)Math.Round((value - hour) * 60);

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

            int hour = value / 60;
            int minute = value - (hour * 60);

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
            string statusName;
            switch (statusId)
            {
                case (int)CurrentRequestStatus.Pending:
                    statusName = Labels.Pending;
                    break;

                case (int)CurrentRequestStatus.Verified:
                    statusName = Labels.Verified;
                    break;

                case (int)CurrentRequestStatus.NotVerified:
                    statusName = Labels.NotVerified;
                    break;

                case (int)CurrentRequestStatus.Approved:
                    statusName = Labels.Approved;
                    break;

                case (int)CurrentRequestStatus.Rejected:
                    statusName = Labels.Rejected;
                    break;

                case (int)CurrentRequestStatus.Processed:
                    statusName = Labels.Processed;
                    break;

                case (int)CurrentRequestStatus.Void:
                    statusName = Labels.Void;
                    break;

                default:
                    statusName = Labels.NA;
                    break;
            }

            return statusName;
        }

        public static IList<SelectListItem> GetDateRangeByYear()
        {
            var dateRange = new List<SelectListItem> { new SelectListItem { Text = Labels.DateRange, Value = "0" } };
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
            string backgroundColor;
            switch (leaveTypeId)
            {
                case (byte)LeaveTypes.AnnualLeave:
                    backgroundColor = "#c7f391";
                    break;

                case (byte)LeaveTypes.UnpaidLeave:
                    backgroundColor = "#eacded";
                    break;

                case (byte)LeaveTypes.BusinessTrip:
                    backgroundColor = "#c1d6ed";
                    break;

                case (byte)LeaveTypes.WeddingLeave:
                case (byte)LeaveTypes.WeddingOfEmployeeChildren:
                    backgroundColor = "#7cd3e9";
                    break;

                default:
                    backgroundColor = "#a9a7a7";
                    break;
            }

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
                result = (fromDate.Month != toDate.Month) ?
                fromDate.ToMonthAndDayFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate() :
                fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();
            }

            return result;
        }
    }
}