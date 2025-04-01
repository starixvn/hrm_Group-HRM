namespace HRM.ViewModels;

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

public class PagerOptionModel
{
    public int ItemsPerPage { get; set; }

    public int CurrentPage { get; set; }

    public int TotalItem { get; set; }

    public IList<SelectListItem> ItemsPerPageArray
    {
        get
        {
            var itemsPerPages = new List<SelectListItem>
            {
                new() { Text = "50", Value = "50" },
                new() { Text = "100", Value = "100" },
                new() { Text = "200", Value = "200" }
            };

            return itemsPerPages;
        }
    }
}