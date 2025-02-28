namespace HRM.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using HRM.Core.Enums;
    using HRM.Resources;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class Extensions
    {
        public static string RequestTypeToText(this short type)
        {
            return type switch
            {
                (byte)RequestType.CheckIn => Labels.CheckIn,
                (byte)RequestType.CheckOut => Labels.CheckOut,
                (byte)RequestType.LateIn => Labels.LateIn,
                (byte)RequestType.EarlyOut => Labels.EarlyOut,
                (byte)RequestType.ChangeShift => Labels.ChangeShift,
                (byte)RequestType.Leave => Labels.Leave,
                (byte)RequestType.KeepInView => Labels.CompensationLeave,
                (byte)RequestType.Overtime => Labels.Overtime,
                _ => string.Empty
            };
        }

        public static List<SelectListItem> YearPicker(
            int fromYear = 2000,
            int currentYear = 2000,
            bool isShowAll = false)
        {
            var list = new List<SelectListItem>();
            var toYear = DateTimeOffset.Now.DateTime.Year;

            if (isShowAll)
            {
                list.Add(new SelectListItem
                {
                    Text = Labels.All,
                    Value = "0"
                });
            }

            for (var i = toYear + 1; i >= fromYear; i--)
            {
                list.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == currentYear
                });
            }

            return list;
        }

        public static List<SelectListItem> YearPickerFor(
            int fromYear = 2000,
            bool isShowAll = false)
        {
            var currentYear = DateTimeOffset.Now.DateTime.Year;

            return YearPicker(fromYear, currentYear, isShowAll);
        }

        public static DateTime GetMaxDate(this DateTime date)
        {
            if (date.AddMonths(1).Year > DateTimeOffset.Now.Year)
            {
                return DateTimeOffset.Now.AddYears(1).DateTime;
            }

            return date;
        }

        public static HtmlString AttendanceNoteHtml(string notes)
        {
            var htmlString = new StringBuilder();

            if (!string.IsNullOrEmpty(notes))
            {
                string[] noteArray = notes.Split(',');

                for (int i = 0; i < noteArray.Length; i++)
                {
                    if (noteArray[i] == "No Data" || noteArray[i] == "PH" || noteArray[i].Contains("Off day"))
                    {
                        htmlString.Append(noteArray[i]);
                    }
                    else
                    {
                        bool isLastElement = (i + 1 == noteArray.Length);
                        SetLinkValue(noteArray[i], ref htmlString, isLastElement);
                    }
                }
            }

            return new HtmlString(htmlString.ToString());
        }

        private static void SetLinkValue(string noteValue, ref StringBuilder htmlString, bool isLastElement)
        {
            string link;

            if (noteValue.Contains("Missing"))
            {
                link = "/checkinout/employeeview";
            }
            else if (noteValue.Contains("Late") || noteValue.Contains("Early"))
            {
                link = "/lateinearlyout/employeeview";
            }
            else if (noteValue.Contains("KIV"))
            {
                link = "/keepinview/employeeview";
            }
            else
            {
                link = "/leave/employeeview";
            }

            string safeNoteValue = HtmlEncoder.Default.Encode(noteValue);

            htmlString.Append($"<a href='{link}'>{safeNoteValue}</a>");

            if (!isLastElement)
            {
                htmlString.Append(", ");
            }
        }
    }
}