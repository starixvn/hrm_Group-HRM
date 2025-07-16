namespace HRM.Models.Request;

using Core.Enums;

public class ProcessRequest
{
    public string RequestIds { get; set; }
    public RequestCommand Command { get; set; }
    public int ProcessedBy { get; set; }
    public string CallBackUrl { get; set; }
}