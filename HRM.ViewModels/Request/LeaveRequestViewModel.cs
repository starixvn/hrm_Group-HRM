namespace HRM.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Models;
    using HRM.Models.Request;
    using Models.Supervisor;

    public class LeaveRequestViewModel : IValidatableObject
    {
        public long RequestId { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public string Remark { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.##}")]
        public decimal TotalDays { get; set; }

        public int LeaveTypeId { get; set; }

        public string FromTime { get; set; }

        public int FromTimeHour { get; set; }

        public int FromTimeMinute { get; set; }

        public int ToTimeHour { get; set; }

        public int ToTimeMinute { get; set; }

        public int Minutes { get; set; }

        public bool IsFromTimeAfterDay { get; set; }

        public string ToTime { get; set; }

        public bool IsToTimeAfterDay { get; set; }

        public bool HalfDayOff { get; set; }

        public bool IsHospitalApplication { get; set; }

        public string LeaveTypeName { get; set; }

        public string LeaveTypeDescription
        {
            get
            {
                return LeaveTypeList.FirstOrDefault(t => t.Id == LeaveTypeId).Description;
            }
        }

        public string RefNo { get; set; }

        public int VerifierId { get; set; }

        public int ApproverId { get; set; }

        public decimal TotalWeekendLeave { get; set; }

        public IEnumerable<DateTime> DisabledDates { get; set; }

        public IList<LeaveType> LeaveTypeList { get; set; }

        public EmployeeLeaveBalance EmployeeLeaveBalance { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime MinDate { get; set; }

        public DateTime MaxDate { get; set; }

        public IList<SupervisorNameModel> Verifiers { get; set; } = new List<SupervisorNameModel>();
        public IList<SupervisorNameModel> Approvers { get; set; } = new List<SupervisorNameModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            var diff = ToDate.Subtract(FromDate);
            if (diff.TotalDays < 0)
            {
                var validation = new ValidationResult(
                    Resources.Messages.InvalidToDate,
                    new[] { "ToDate" });
                result.Add(validation);
            }

            if (HalfDayOff && (TotalDays % 1 == 0))
            {
                var validation = new ValidationResult(
                    Resources.Messages.InvalidTotalDays,
                    new[] { "TotalDays" });
                result.Add(validation);
            }

            return result;
        }

        public List<string> Files { get; set; }
        public bool EnableHospitalizationLeaveBalance { get; set; }
        public bool EnableSickLeaveBalance { get; set; }
    }
}