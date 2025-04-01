namespace HRM.Models.Dashboard;

public class SummaryRequest
{
    public ushort RequestTypeId { get; set; }
    public string RequestType { get; set; }
    public ushort TotalRequest { get; set; }
    public ushort TotalPending { get; set; }
}