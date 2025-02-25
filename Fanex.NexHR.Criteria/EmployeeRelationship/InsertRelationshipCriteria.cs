namespace Fanex.NexHR.Criteria.EmployeeRelationship
{
    using System;
    using Fanex.Data.Repository;

    public class InsertRelationshipCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Relationship { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int Gender { get; set; }

        public bool IsEmergency { get; set; }

        public int Creator { get; set; }

        public override string GetSettingKey() => "EmployeeRelationship_Insert";

        public override bool IsValid() => EmployeeId > 0 && Creator > 0 && !string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(Relationship);
    }
}