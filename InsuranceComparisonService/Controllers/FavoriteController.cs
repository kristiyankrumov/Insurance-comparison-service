using InsuranceComparisonService.Data;
using InsuranceComparisonService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceComparisonService.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var favorites = await _context.UserFavorites
                .Include(f => f.Offer)
                    .ThenInclude(o => o!.Company)
                .Where(f => f.UserId == user.Id)
                .ToListAsync();

            return View(favorites);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int offerId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var exists = await _context.UserFavorites
                .AnyAsync(f => f.UserId == user.Id && f.OfferId == offerId);

            if (!exists)
            {
                _context.UserFavorites.Add(new UserFavorite
                {
                    UserId = user.Id,
                    OfferId = offerId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Insurance", new { id = offerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var fav = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == user.Id);

            if (fav != null)
            {
                _context.UserFavorites.Remove(fav);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
