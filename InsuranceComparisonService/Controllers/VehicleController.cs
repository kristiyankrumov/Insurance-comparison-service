using InsuranceComparisonService.Data;
using InsuranceComparisonService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceComparisonService.Controllers
{
    [Authorize]
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehicleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET /Vehicle
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var vehicles = await _context.Vehicles
                .Where(v => v.UserId == user.Id)
                .OrderByDescending(v => v.AddedAt)
                .ToListAsync();

            return View(vehicles);
        }

        // GET /Vehicle/Create
        public IActionResult Create() => View();

        // POST /Vehicle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                vehicle.UserId = user.Id;
                vehicle.PlateNumber = vehicle.PlateNumber.ToUpper().Trim();
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Превозното средство беше добавено успешно!";
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET /Vehicle/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == user.Id);

            if (vehicle == null) return NotFound();
            return View(vehicle);
        }

        // POST /Vehicle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Vehicle vehicle)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Verify ownership
            var existing = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicle.Id && v.UserId == user.Id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                existing.Make = vehicle.Make;
                existing.Model = vehicle.Model;
                existing.Year = vehicle.Year;
                existing.PlateNumber = vehicle.PlateNumber.ToUpper().Trim();
                existing.HorsePower = vehicle.HorsePower;
                existing.FuelType = vehicle.FuelType;
                existing.Category = vehicle.Category;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Данните на превозното средство бяха обновени!";
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // POST /Vehicle/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == user.Id);

            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Превозното средство беше изтрито.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
