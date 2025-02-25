namespace HRM.Services
{
    public interface ILetstalkService
    {
        void SendNotification(string letstalkId, string subject, string message);
    }
}
