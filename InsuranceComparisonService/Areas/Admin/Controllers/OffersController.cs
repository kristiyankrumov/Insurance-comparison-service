using InsuranceComparisonService.Models;
using InsuranceComparisonService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceComparisonService.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class OffersController : Controller
    {
        private readonly IInsuranceRepository _repo;

        public OffersController(IInsuranceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var offers = await _repo.GetAllOffersAsync();
            return View(offers);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = await _repo.GetAllCompaniesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InsuranceOffer offer)
        {
            if (ModelState.IsValid)
            {
                await _repo.AddOfferAsync(offer);
                return RedirectToAction("Index");
            }
            ViewBag.Companies = await _repo.GetAllCompaniesAsync();
            return View(offer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var offer = await _repo.GetOfferByIdAsync(id);
            if (offer == null) return NotFound();
            ViewBag.Companies = await _repo.GetAllCompaniesAsync();
            return View(offer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InsuranceOffer offer)
        {
            if (ModelState.IsValid)
            {
                await _repo.UpdateOfferAsync(offer);
                return RedirectToAction("Index");
            }
            ViewBag.Companies = await _repo.GetAllCompaniesAsync();
            return View(offer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteOfferAsync(id);
            return RedirectToAction("Index");
        }
    }
}
