namespace HRM.ViewModels.Memo
{
    using System;
    using System.Collections.Generic;
    using HRM.Models;

    public class MemoViewModel
    {
        public int Year { get; set; } = DateTimeOffset.Now.Year;

        public int Category { get; set; } = -1;

        public IList<LookupData> MemoCategories { get; set; } = new List<LookupData>();

        public IEnumerable<Memo> Memos { get; set; } = new List<Memo>();
    }
}