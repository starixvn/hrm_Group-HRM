namespace Fanex.NexHR.ViewModels.Memo
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;

    public class MemoViewModel
    {
        public int Year { get; set; } = DateTimeOffset.Now.Year;

        public int Category { get; set; } = -1;

        public IList<LookupData> MemoCategories { get; set; } = new List<LookupData>();

        public IList<Memo> Memos { get; set; } = new List<Memo>();
    }
}
