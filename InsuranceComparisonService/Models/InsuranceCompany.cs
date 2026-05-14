namespace InsuranceComparisonService.Models
{
    public class InsuranceCompany
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<InsuranceOffer> Offers { get; set; } = new List<InsuranceOffer>();
    }
}
