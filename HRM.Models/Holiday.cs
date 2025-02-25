namespace HRM.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRM.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class Holiday : EntityBase<int>
    {
        [Required]
        public string HolidayName { get; set; }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public byte TypeId { get; set; }
    }
}