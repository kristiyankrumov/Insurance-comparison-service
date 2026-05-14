using InsuranceComparisonService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceComparisonService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInsuranceRepository _repo;

        public HomeController(IInsuranceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var offers = await _repo.GetAllOffersAsync();
            return View(offers);
        }

        [Route("Home/Error/{statusCode?}")]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == 404)
                return View("Error404");
            return View("Error500");
        }
    }
}
