using System.ComponentModel.DataAnnotations;

namespace InsuranceComparisonService.Models
{
    public enum InsuranceType
    {
        Kasko,
        Health,
        Civil,      // Гражданска отговорност
        Property    // Имуществена застраховка
    }

    public class InsuranceOffer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Наименованието е задължително")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Видът застраховка е задължителен")]
        public InsuranceType Type { get; set; }

        [Required(ErrorMessage = "Цената е задължителна")]
        [Range(1, 100000, ErrorMessage = "Цената трябва да е между 1 и 100000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Покритието е задължително")]
        public string Coverage { get; set; } = string.Empty;

        public string Conditions { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CompanyId { get; set; }
        public InsuranceCompany? Company { get; set; }

        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
