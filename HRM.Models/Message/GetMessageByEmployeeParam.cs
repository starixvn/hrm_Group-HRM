namespace HRM.Models.Message;

using System;

public class GetMessageByEmployeeParam
{
    public int EmployeeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int? IsDisabled { get; set; }
}