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

    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Коментарът е задължителен")]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Оценката трябва да е между 1 и 5")]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int OfferId { get; set; }
        public InsuranceOffer? Offer { get; set; }
    }
}
