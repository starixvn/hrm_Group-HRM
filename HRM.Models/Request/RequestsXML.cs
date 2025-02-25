namespace HRM.Models.Request
{
    using System;
    using HRM.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class RequestsXML
    {
        public long RequestId { get; set; }
        public int RequestTypeID { get; set; }
    }
}