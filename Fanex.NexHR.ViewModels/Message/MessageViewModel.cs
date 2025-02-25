namespace Fanex.NexHR.ViewModels.Message
{
    using System;

    public class MessageViewModel
    {
        public int MessageId { get; set; }

        public string Subject { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string SendTo { get; set; }
    }
}
