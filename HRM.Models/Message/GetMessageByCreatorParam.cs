namespace HRM.Models.Message;

using System;

public class GetMessageByCreatorParam
{
    public int CreatedBy { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? IsDisabled { get; set; }
}