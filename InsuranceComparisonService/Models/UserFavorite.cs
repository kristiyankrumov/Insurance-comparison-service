using System.ComponentModel.DataAnnotations;

namespace InsuranceComparisonService.Models
{
    public class UserFavorite
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public int OfferId { get; set; }
        public InsuranceOffer? Offer { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
