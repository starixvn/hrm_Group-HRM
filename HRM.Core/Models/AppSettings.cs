namespace HRM.Core.Models
{
    public class AppSettings
    {
        public short AppMailId { get; set; }
        public string Domain { get; set; }
        public bool EnableSendMail { get; set; }
    }
}