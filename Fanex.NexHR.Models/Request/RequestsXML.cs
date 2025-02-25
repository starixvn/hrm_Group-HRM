namespace Fanex.NexHR.Models.Request
{
    using System;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class RequestsXML
    {
        public long RequestId { get; set; }
        public int RequestTypeID { get; set; }
    }
}