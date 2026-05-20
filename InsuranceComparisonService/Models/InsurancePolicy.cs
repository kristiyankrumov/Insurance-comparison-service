using System.ComponentModel.DataAnnotations;

namespace InsuranceComparisonService.Models
{
    public enum PolicyStatus { Active, Expired, Cancelled }
    public enum PaymentType { OneTime, Monthly }

    public class InsurancePolicy
    {
        public int Id { get; set; }

        // Linked offer
        public int OfferId { get; set; }
        public InsuranceOffer? Offer { get; set; }

        // Linked user
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        // Optionally linked vehicle (for Kasko/Civil)
        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public PolicyStatus Status { get; set; } = PolicyStatus.Active;
        public PaymentType PaymentType { get; set; } = PaymentType.OneTime;

        // Calculated price (may differ from offer base price due to age/experience etc.)
        [Range(1, 200000)]
        public decimal FinalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Policy number
        public string PolicyNumber { get; set; } = string.Empty;
    }
}
