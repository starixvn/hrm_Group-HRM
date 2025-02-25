namespace HRM.Core
{
    using HRM.Core.Enums;
    using HRM.Resources;

    using System.Linq.Expressions;
    using System;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using System.Web.Mvc.Html;
    using Microsoft.AspNetCore.Html;
    using System.IO;
    using System.Text.Encodings.Web;

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


    }
}