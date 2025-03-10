namespace HRM.ViewModels.Memo;

using System;
using System.Collections.Generic;
using HRM.Core;
using HRM.Models;

public class MemoViewModel
{
    public int Year { get; set; } = DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;

    public int Category { get; set; } = -1;

    public IEnumerable<LookupData> MemoCategories { get; set; } = new List<LookupData>();

    public IEnumerable<Memo> Memos { get; set; } = new List<Memo>();
}