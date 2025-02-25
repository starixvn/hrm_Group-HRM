namespace Fanex.NexHR.Models.Supervisor
{
    public class SupervisorNameModel
    {
        public int OwnerId { get; set; }

        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string MailGroup { get; set; }

        public int ActionType { get; set; }
        public bool IsTopManager { get; set; }

        public string DisplayName
        {
            get
            {
                return NickName + " (" + FullName + ")";
            }
        }

        public string LetsTalk { get; set; }
    }
}