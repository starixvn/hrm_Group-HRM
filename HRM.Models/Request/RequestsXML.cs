namespace HRM.Models.Request
{
    using System;
    using Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class RequestsXML
    {
        public long RequestId { get; set; }
        public int RequestTypeID { get; set; }
    }
}