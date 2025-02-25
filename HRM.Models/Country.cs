namespace HRM.Models
{
    public class Country : EntityBase<int>
    {
        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public int CountryOrder { get; set; }
    }
}
