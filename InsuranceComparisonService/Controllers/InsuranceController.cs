using InsuranceComparisonService.Data;
using InsuranceComparisonService.Models;
using InsuranceComparisonService.Repositories;
using InsuranceComparisonService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceComparisonService.Controllers
{
    public class InsuranceController : Controller
    {
        private readonly IInsuranceRepository _repo;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PriceCalculatorService _calculator;

        public InsuranceController(
            IInsuranceRepository repo,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            PriceCalculatorService calculator)
        {
            _repo = repo;
            _context = context;
            _userManager = userManager;
            _calculator = calculator;
        }

        private async Task SetFilterViewBag(decimal? minPrice, decimal? maxPrice, int? companyId)
        {
            ViewBag.Companies = await _repo.GetAllCompaniesAsync();
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CompanyId = companyId;
        }

        private static IEnumerable<InsuranceOffer> ApplyFilters(
            IEnumerable<InsuranceOffer> offers, decimal? minPrice, decimal? maxPrice, int? companyId)
        {
            if (minPrice.HasValue && minPrice > 0)  offers = offers.Where(o => o.Price >= minPrice.Value);
            if (maxPrice.HasValue && maxPrice > 0)  offers = offers.Where(o => o.Price <= maxPrice.Value);
            if (companyId.HasValue && companyId > 0) offers = offers.Where(o => o.CompanyId == companyId.Value);
            return offers;
        }

        public async Task<IActionResult> Kasko(decimal? minPrice, decimal? maxPrice, int? companyId)
        {
            var offers = await _repo.GetOffersByTypeAsync(InsuranceType.Kasko);
            await SetFilterViewBag(minPrice, maxPrice, companyId);
            return View(ApplyFilters(offers, minPrice, maxPrice, companyId));
        }

        public async Task<IActionResult> Health(decimal? minPrice, decimal? maxPrice, int? companyId)
        {
            var offers = await _repo.GetOffersByTypeAsync(InsuranceType.Health);
            await SetFilterViewBag(minPrice, maxPrice, companyId);
            return View(ApplyFilters(offers, minPrice, maxPrice, companyId));
        }

        public async Task<IActionResult> Civil(decimal? minPrice, decimal? maxPrice, int? companyId)
        {
            var offers = await _repo.GetOffersByTypeAsync(InsuranceType.Civil);
            await SetFilterViewBag(minPrice, maxPrice, companyId);
            return View(ApplyFilters(offers, minPrice, maxPrice, companyId));
        }

        public async Task<IActionResult> Property(decimal? minPrice, decimal? maxPrice, int? companyId)
        {
            var offers = await _repo.GetOffersByTypeAsync(InsuranceType.Property);
            await SetFilterViewBag(minPrice, maxPrice, companyId);
            return View(ApplyFilters(offers, minPrice, maxPrice, companyId));
        }

        public async Task<IActionResult> Details(int id)
        {
            var offer = await _repo.GetOfferByIdAsync(id);
            if (offer == null) return NotFound();

            // If user is logged in, calculate personalised price
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.Age != null && user.DrivingExperienceYears != null)
                {
                    int vehicleYear = DateTime.Now.Year;
                    ViewBag.PersonalisedPrice = _calculator.Calculate(
                        offer, user.Age.Value, user.DrivingExperienceYears.Value, vehicleYear);
                }
            }

            return View(offer);
        }

        public async Task<IActionResult> Compare(int id1, int id2)
        {
            if (id1 == 0 || id2 == 0)
                return RedirectToAction("Index", "Home");

            var offer1 = await _repo.GetOfferByIdAsync(id1);
            var offer2 = await _repo.GetOfferByIdAsync(id2);

            if (offer1 == null || offer2 == null) return NotFound();

            return View((offer1, offer2));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int offerId, string comment, int rating)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var existing = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.OfferId == offerId);

            if (existing == null)
            {
                _context.Reviews.Add(new Review
                {
                    OfferId = offerId,
                    UserId = user.Id,
                    Comment = comment,
                    Rating = rating
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = offerId });
        }
    }
}
 
