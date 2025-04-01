namespace HRM.Models.Request;

using Core;

public class RequestTypeModel : EntityBase<short>
{
    public new string Name => Id.RequestTypeToText();

    public override string ToString() => Name;
}