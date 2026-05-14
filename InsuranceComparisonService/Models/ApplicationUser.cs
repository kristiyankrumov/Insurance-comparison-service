using Microsoft.AspNetCore.Identity;

namespace InsuranceComparisonService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Driver info for price calculation
        public int? Age { get; set; }
        public int? DrivingExperienceYears { get; set; }

        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<InsurancePolicy> Policies { get; set; } = new List<InsurancePolicy>();
    }
}
