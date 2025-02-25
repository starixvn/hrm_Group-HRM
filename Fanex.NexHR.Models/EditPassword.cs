namespace Fanex.NexHR.Models
{
    public class EditPassword
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string Password { get; set; }
        public bool IsReset { get; set; }
    }
}