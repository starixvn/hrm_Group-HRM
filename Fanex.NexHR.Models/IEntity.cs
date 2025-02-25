namespace Fanex.NexHR.Models
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }

        string Name { get; set; }

        string Code { get; set; }
    }
}