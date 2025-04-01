namespace HRM.Core.Models;

using Resources;

public class CommandResult
{
    public int Code { get; set; } = 1;
    public string Message { get; set; } = Messages.InvalidInputData;

    public short CompanyId { get; set; }

    public bool IsSuccessful => Code == 0;

    public long OutputId { get; set; }
}