namespace HRM.Models;

public class EntityBase<TId> : IEntity<TId>
{
    public TId Id { get; set; }

    public string Name { get; set; }

    public virtual string Code { get; set; }
}