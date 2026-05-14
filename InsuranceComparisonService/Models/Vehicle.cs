using System.ComponentModel.DataAnnotations;

namespace InsuranceComparisonService.Models
{
    public enum FuelType { Petrol, Diesel, Electric, Hybrid }
    public enum VehicleCategory { Car, Motorcycle, Truck, Bus }

    public class Vehicle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Марката е задължителна")]
        [StringLength(100)]
        public string Make { get; set; } = string.Empty;     // напр. Toyota

        [Required(ErrorMessage = "Моделът е задължителен")]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;    // напр. Corolla

        [Required(ErrorMessage = "Годината е задължителна")]
        [Range(1950, 2026, ErrorMessage = "Невалидна година")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Регистрационният номер е задължителен")]
        [StringLength(10)]
        public string PlateNumber { get; set; } = string.Empty;

        [Range(50, 2000, ErrorMessage = "Мощността трябва да е между 50 и 2000 к.с.")]
        public int HorsePower { get; set; }

        public FuelType FuelType { get; set; }
        public VehicleCategory Category { get; set; }

        // Owner
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation to linked policies
        public ICollection<InsurancePolicy> Policies { get; set; } = new List<InsurancePolicy>();
    }
}
