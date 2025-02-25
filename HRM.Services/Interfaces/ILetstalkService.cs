namespace HRM.Services.Interfaces
{
    public interface ILetstalkService
    {
        void SendNotification(string letstalkId, string subject, string message);
    }
}
