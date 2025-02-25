namespace HRM.Models
{
    public class EmailModel
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        public string SendTo { get; set; }

        public string SendCC { get; set; }

        public string Sender { get; set; }

        public string SendToNickName { get; set; }

        public bool IsAllowSendRequester { get; set; }

        public string LetsTalk { get; set; }
    }
}
