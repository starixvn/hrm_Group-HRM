namespace Fanex.NexHR.Models
{
    using System;
    using Fanex.NexHR.Resources;

    public class TimeRecordModel
    {
        public int RecordTimeId { get; set; }
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Team { get; set; }
        public DateTime RecordTime { get; set; }
        public byte? InOutType { get; set; }

        public string InOutName
        {
            get
            {
                var typeName = Labels.None;

                if (InOutType == 0)
                {
                    typeName = Labels.In;
                }

                if (InOutType == 1)
                {
                    typeName = Labels.Out;
                }

                return typeName;
            }
        }

        public string SectionCode { get; set; }

        public string DepartmentCode { get; set; }

        public bool IsCheckInOutOnline { get; set; }
    }
}