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
    [Authorize]
    public class PolicyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IInsuranceRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PriceCalculatorService _calculator;
        private readonly IEmailService _emailService;

        public PolicyController(
            ApplicationDbContext context,
            IInsuranceRepository repo,
            UserManager<ApplicationUser> userManager,
            PriceCalculatorService calculator,
            IEmailService emailService)
        {
            _context = context;
            _repo = repo;
            _userManager = userManager;
            _calculator = calculator;
            _emailService = emailService;
        }

        // GET /Policy — история на полици
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Автоматично изтичане на полици
            var now = DateTime.UtcNow;
            var toExpire = await _context.InsurancePolicies
                .Where(p => p.UserId == user.Id && p.Status == PolicyStatus.Active && p.EndDate < now)
                .ToListAsync();
            foreach (var p in toExpire) p.Status = PolicyStatus.Expired;
            if (toExpire.Any()) await _context.SaveChangesAsync();

            var policies = await _context.InsurancePolicies
                .Include(p => p.Offer).ThenInclude(o => o!.Company)
                .Include(p => p.Vehicle)
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(policies);
        }

        // GET /Policy/Buy/5 (offerId)
        public async Task<IActionResult> Buy(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var offer = await _repo.GetOfferByIdAsync(id);
            if (offer == null) return NotFound();

            var vehicles = await _context.Vehicles
                .Where(v => v.UserId == user.Id)
                .ToListAsync();

            ViewBag.Offer = offer;
            ViewBag.Vehicles = vehicles;
            ViewBag.User = user;
            return View();
        }

        // POST /Policy/Buy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(
            int offerId,
            int? vehicleId,
            int driverAge,
            int experienceYears,
            int accidentCount,
            PaymentType paymentType,
            int durationMonths)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var offer = await _repo.GetOfferByIdAsync(offerId);
            if (offer == null) return NotFound();

            // Валидация на входни данни
            if (driverAge < 16 || driverAge > 100)
            {
                TempData["Error"] = "Невалидна възраст на водача.";
                return RedirectToAction(nameof(Buy), new { id = offerId });
            }
            if (experienceYears < 0 || experienceYears > 80)
            {
                TempData["Error"] = "Невалиден шофьорски стаж.";
                return RedirectToAction(nameof(Buy), new { id = offerId });
            }
            if (accidentCount < 0 || accidentCount > 20)
            {
                TempData["Error"] = "Невалиден брой ПТП.";
                return RedirectToAction(nameof(Buy), new { id = offerId });
            }
            if (durationMonths < 1 || durationMonths > 24)
            {
                TempData["Error"] = "Невалиден период (1–24 месеца).";
                return RedirectToAction(nameof(Buy), new { id = offerId });
            }

            Vehicle? vehicle = null;
            int vehicleYear = DateTime.Now.Year;
            VehicleCategory vehicleCategory = VehicleCategory.Car;

            if (vehicleId.HasValue)
            {
                vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == user.Id);
                if (vehicle != null)
                {
                    vehicleYear = vehicle.Year;
                    vehicleCategory = vehicle.Category;
                }
            }

            decimal calculatedPrice = _calculator.Calculate(
                offer, driverAge, experienceYears, vehicleYear, accidentCount, vehicleCategory);

            // При разсрочено плащане — цена за целия период
            decimal finalPrice = paymentType == PaymentType.Monthly
                ? Math.Round(calculatedPrice / 12 * durationMonths, 2)
                : calculatedPrice;

            var policy = new InsurancePolicy
            {
                OfferId = offerId,
                UserId = user.Id,
                VehicleId = vehicle?.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(durationMonths),
                Status = PolicyStatus.Active,
                PaymentType = paymentType,
                FinalPrice = finalPrice,
                PolicyNumber = GeneratePolicyNumber()
            };

            _context.InsurancePolicies.Add(policy);
            await _context.SaveChangesAsync();

            // Email потвърждение
            if (!string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendPolicyConfirmationAsync(
                    user.Email, policy.PolicyNumber, finalPrice, policy.EndDate);
            }

            TempData["Success"] = $"Полица {policy.PolicyNumber} беше създадена успешно!";
            return RedirectToAction(nameof(Index));
        }

        // POST /Policy/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var policy = await _context.InsurancePolicies
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);

            if (policy != null && policy.Status == PolicyStatus.Active)
            {
                policy.Status = PolicyStatus.Cancelled;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Полицата беше анулирана.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET /Policy/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var policy = await _context.InsurancePolicies
                .Include(p => p.Offer).ThenInclude(o => o!.Company)
                .Include(p => p.Vehicle)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);

            if (policy == null) return NotFound();
            return View(policy);
        }

        private static string GeneratePolicyNumber()
        {
            // Използваме Guid за уникалност — избягваме колизии при едновременни заявки
            var suffix = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"POL-{DateTime.Now.Year}-{suffix}";
        }
    }
}
