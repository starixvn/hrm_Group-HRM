namespace Fanex.NexHR.Core.Models
{
    using Fanex.NexHR.Resources;

    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = Messages.InvalidInputData;
        }

        public int Code { get; set; }
        public string Message { get; set; }

        public bool IsSuccessful
        {
            get
            {
                return Code == 0;
            }
        }

        public long OutputId { get; set; }
    }
}