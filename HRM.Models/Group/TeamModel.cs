namespace HRM.Models.Group
{
    using System;

    public class TeamModel
    {
        public int TeamId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Section Section { get; set; } = new Section();

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int NoOfMembers { get; set; }
        public int TotalLocal { get; set; }
        public int TotalExpat { get; set; }
        public string MailGroup { get; set; }
    }
}