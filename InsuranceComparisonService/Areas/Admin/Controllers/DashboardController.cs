using InsuranceComparisonService.Data;
using InsuranceComparisonService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceComparisonService.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var superAdmins = new List<ApplicationUser>();
            var admins = new List<ApplicationUser>();
            var users = new List<ApplicationUser>();

            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                if (roles.Contains("SuperAdmin")) superAdmins.Add(u);
                else if (roles.Contains("Admin")) admins.Add(u);
                else users.Add(u);
            }

            var totalOffers = await _context.InsuranceOffers.CountAsync();
            var activeOffers = await _context.InsuranceOffers.CountAsync(o => o.IsActive);
            var totalCompanies = await _context.InsuranceCompanies.CountAsync();
            var totalFavorites = await _context.UserFavorites.CountAsync();
            var totalVehicles = await _context.Vehicles.CountAsync();
            var totalPolicies = await _context.InsurancePolicies.CountAsync();
            var activePolicies = await _context.InsurancePolicies.CountAsync(p => p.Status == PolicyStatus.Active);
            var totalReviews = await _context.Reviews.CountAsync();

            var offersByType = await _context.InsuranceOffers
                .GroupBy(o => o.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            var avgPriceByType = await _context.InsuranceOffers
                .Where(o => o.IsActive)
                .GroupBy(o => o.Type)
                .Select(g => new { Type = g.Key, AvgPrice = g.Average(o => o.Price) })
                .ToListAsync();

            ViewBag.TotalUsers = allUsers.Count;
            ViewBag.SuperAdminCount = superAdmins.Count;
            ViewBag.AdminCount = admins.Count;
            ViewBag.UserCount = users.Count;
            ViewBag.TotalOffers = totalOffers;
            ViewBag.ActiveOffers = activeOffers;
            ViewBag.TotalCompanies = totalCompanies;
            ViewBag.TotalFavorites = totalFavorites;
            ViewBag.TotalVehicles = totalVehicles;
            ViewBag.TotalPolicies = totalPolicies;
            ViewBag.ActivePolicies = activePolicies;
            ViewBag.TotalReviews = totalReviews;
            ViewBag.OffersByType = offersByType;
            ViewBag.AvgPriceByType = avgPriceByType;

            return View();
        }
    }
}
