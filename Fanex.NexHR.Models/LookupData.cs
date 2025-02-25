namespace Fanex.NexHR.Models
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Helpers;

    public class LookupData
    {
        public int Id { get; set; }

        public int LookupId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public int TotalLocal { get; set; }

        public int TotalExpat { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName => EnumHelper.GetStringValue((JobTitleCategory)CategoryId);
    }
}