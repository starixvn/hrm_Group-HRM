namespace HRM.ViewModels.Time;

using System;
using System.ComponentModel.DataAnnotations;

public class TimeRecordViewModel
{
    private const byte MaxHour = 23;
    private const byte MaxMinute = 59;

    [Required]
    public int EmpId { get; set; }

    [Required]
    public string Employee { get; set; }

    public DateTime RecordTime { get; set; }

    public DateTime InsertDate { get; set; }

    [Required]
    [Display(Name = "In Out Type")]
    public short InOutType { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
    public DateTime ForDate { get; set; }

    [Required]
    [Range(0, MaxHour, ErrorMessage = "Hour is less than 23.")]
    public int Hour { get; set; }

    [Required]
    [Range(0, MaxMinute, ErrorMessage = "Minutes is less than 59.")]
    public int Minute { get; set; }
}