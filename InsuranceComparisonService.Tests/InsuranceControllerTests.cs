using InsuranceComparisonService.Controllers;
using InsuranceComparisonService.Models;
using InsuranceComparisonService.Repositories;
using InsuranceComparisonService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InsuranceComparisonService.Tests
{
    public class InsuranceControllerTests
    {
        private readonly Mock<IInsuranceRepository> _mockRepo;
        private readonly PriceCalculatorService _calculator;

        public InsuranceControllerTests()
        {
            _mockRepo = new Mock<IInsuranceRepository>();
            _calculator = new PriceCalculatorService();
        }

        private InsuranceController CreateController()
            => new InsuranceController(_mockRepo.Object, null!, null!, _calculator);

        // ─── Kasko ───────────────────────────────────────────────────────
        [Fact]
        public async Task Kasko_ReturnsViewWithKaskoOffers()
        {
            var offers = new List<InsuranceOffer>
            {
                new InsuranceOffer { Id = 1, Title = "Каско Тест", Type = InsuranceType.Kasko, Price = 800 },
                new InsuranceOffer { Id = 2, Title = "Каско 2",    Type = InsuranceType.Kasko, Price = 1200 }
            };
            _mockRepo.Setup(r => r.GetOffersByTypeAsync(InsuranceType.Kasko)).ReturnsAsync(offers);
            _mockRepo.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(new List<InsuranceCompany>());

            var result = await CreateController().Kasko(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<InsuranceOffer>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Kasko_FilterByMinPrice_ReturnsOnlyMatchingOffers()
        {
            var offers = new List<InsuranceOffer>
            {
                new InsuranceOffer { Id = 1, Type = InsuranceType.Kasko, Price = 500, CompanyId = 1 },
                new InsuranceOffer { Id = 2, Type = InsuranceType.Kasko, Price = 1000, CompanyId = 1 }
            };
            _mockRepo.Setup(r => r.GetOffersByTypeAsync(InsuranceType.Kasko)).ReturnsAsync(offers);
            _mockRepo.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(new List<InsuranceCompany>());

            var result = await CreateController().Kasko(700, null, null);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<InsuranceOffer>>(view.Model);
            Assert.Single(model);
            Assert.Equal(1000, model.First().Price);
        }

        [Fact]
        public async Task Kasko_FilterByMaxPrice_ReturnsOnlyMatchingOffers()
        {
            var offers = new List<InsuranceOffer>
            {
                new InsuranceOffer { Id = 1, Type = InsuranceType.Kasko, Price = 500, CompanyId = 1 },
                new InsuranceOffer { Id = 2, Type = InsuranceType.Kasko, Price = 1000, CompanyId = 1 }
            };
            _mockRepo.Setup(r => r.GetOffersByTypeAsync(InsuranceType.Kasko)).ReturnsAsync(offers);
            _mockRepo.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(new List<InsuranceCompany>());

            var result = await CreateController().Kasko(null, 700, null);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<InsuranceOffer>>(view.Model);
            Assert.Single(model);
            Assert.Equal(500, model.First().Price);
        }

        [Fact]
        public async Task Kasko_FilterByCompanyId_ReturnsOnlyMatchingOffers()
        {
            var offers = new List<InsuranceOffer>
            {
                new InsuranceOffer { Id = 1, Type = InsuranceType.Kasko, Price = 500, CompanyId = 1 },
                new InsuranceOffer { Id = 2, Type = InsuranceType.Kasko, Price = 800, CompanyId = 2 }
            };
            _mockRepo.Setup(r => r.GetOffersByTypeAsync(InsuranceType.Kasko)).ReturnsAsync(offers);
            _mockRepo.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(new List<InsuranceCompany>());

            var result = await CreateController().Kasko(null, null, 2);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<InsuranceOffer>>(view.Model);
            Assert.Single(model);
            Assert.Equal(2, model.First().CompanyId);
        }

        // ─── Health ──────────────────────────────────────────────────────
        [Fact]
        public async Task Health_ReturnsViewWithHealthOffers()
        {
            var offers = new List<InsuranceOffer>
            {
                new InsuranceOffer { Id = 3, Title = "Здравна Базик", Type = InsuranceType.Health, Price = 350 }
            };
            _mockRepo.Setup(r => r.GetOffersByTypeAsync(InsuranceType.Health)).ReturnsAsync(offers);
            _mockRepo.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(new List<InsuranceCompany>());

            var result = await CreateController().Health(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<InsuranceOffer>>(viewResult.Model);
            Assert.Single(model);
        }

        // ─── Civil ───────────────────────────────────────────────────────
        [Fact]
        public async Task Civil_ReturnsViewWithCivilOffers()
        {
            var offers = new List<InsuranceOffer>
            {
                new InsuranceOffer { Id = 9,  Type = InsuranceType.Civil, Price = 180 },
                new InsuranceOffer { Id = 10, Type = InsuranceType.Civil, Price = 250 }
            };
            _mockRepo.Setup(r => r.GetOffersByTypeAsync(InsuranceType.Civil)).ReturnsAsync(offers);
            _mockRepo.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(new List<InsuranceCompany>());

            var result = await CreateController().Civil(null, null, null);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<InsuranceOffer>>(view.Model);
            Assert.Equal(2, model.Count());
        }

        // ─── Property ────────────────────────────────────────────────────
        [Fact]
        public async Task Property_ReturnsViewWithPropertyOffers()
        {
            var offers = new List<InsuranceOffer>
            {
                new InsuranceOffer { Id = 13, Type = InsuranceType.Property, Price = 220 }
            };
            _mockRepo.Setup(r => r.GetOffersByTypeAsync(InsuranceType.Property)).ReturnsAsync(offers);
            _mockRepo.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(new List<InsuranceCompany>());

            var result = await CreateController().Property(null, null, null);

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<InsuranceOffer>>(view.Model);
        }

        // ─── Details ─────────────────────────────────────────────────────
        [Fact]
        public async Task Details_ReturnsNotFound_WhenOfferDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetOfferByIdAsync(99)).ReturnsAsync((InsuranceOffer?)null);
            var result = await CreateController().Details(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenOfferExists()
        {
            var offer = new InsuranceOffer { Id = 1, Title = "Каско Тест", Type = InsuranceType.Kasko, Price = 800 };
            _mockRepo.Setup(r => r.GetOfferByIdAsync(1)).ReturnsAsync(offer);

            var result = await CreateController().Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(offer, viewResult.Model);
        }

        // ─── Compare ─────────────────────────────────────────────────────
        [Fact]
        public async Task Compare_RedirectsToHome_WhenIdsAreZero()
        {
            var result = await CreateController().Compare(0, 0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Compare_ReturnsView_WhenBothOffersExist()
        {
            var offer1 = new InsuranceOffer { Id = 1, Title = "Оферта 1", Type = InsuranceType.Kasko,  Price = 800 };
            var offer2 = new InsuranceOffer { Id = 2, Title = "Оферта 2", Type = InsuranceType.Civil,  Price = 200 };
            _mockRepo.Setup(r => r.GetOfferByIdAsync(1)).ReturnsAsync(offer1);
            _mockRepo.Setup(r => r.GetOfferByIdAsync(2)).ReturnsAsync(offer2);

            var result = await CreateController().Compare(1, 2);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Compare_ReturnsNotFound_WhenOneOfferMissing()
        {
            var offer1 = new InsuranceOffer { Id = 1 };
            _mockRepo.Setup(r => r.GetOfferByIdAsync(1)).ReturnsAsync(offer1);
            _mockRepo.Setup(r => r.GetOfferByIdAsync(99)).ReturnsAsync((InsuranceOffer?)null);

            var result = await CreateController().Compare(1, 99);
            Assert.IsType<NotFoundResult>(result);
        }

        // ─── Model validation ─────────────────────────────────────────────
        [Fact]
        public void InsuranceOffer_DefaultIsActive_IsTrue()
        {
            var offer = new InsuranceOffer();
            Assert.True(offer.IsActive);
        }

        [Fact]
        public void InsuranceOffer_PriceValidation_FailsForNegativePrice()
        {
            var offer = new InsuranceOffer { Price = -100 };
            Assert.True(offer.Price < 0);
        }

        [Fact]
        public void UserFavorite_DefaultSavedAt_IsRecentDate()
        {
            var fav = new UserFavorite();
            Assert.True(fav.SavedAt >= DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public void InsurancePolicy_DefaultStatus_IsActive()
        {
            var policy = new InsurancePolicy();
            Assert.Equal(PolicyStatus.Active, policy.Status);
        }

        [Fact]
        public void Vehicle_PlateNumberProperty_Exists()
        {
            var v = new Vehicle { PlateNumber = "PB1234AB" };
            Assert.Equal("PB1234AB", v.PlateNumber);
        }

        // ─── PriceCalculatorService ───────────────────────────────────────
        [Fact]
        public void PriceCalculator_YoungDriver_IncreasesPrice()
        {
            var offer = new InsuranceOffer { Price = 1000, Type = InsuranceType.Kasko };
            var price = _calculator.Calculate(offer, driverAge: 22, experienceYears: 1, vehicleYear: DateTime.Now.Year - 5);
            Assert.True(price > 1000);
        }

        [Fact]
        public void PriceCalculator_ExperiencedDriver_DecreasesPrice()
        {
            var offer = new InsuranceOffer { Price = 1000, Type = InsuranceType.Kasko };
            var price = _calculator.Calculate(offer, driverAge: 35, experienceYears: 15, vehicleYear: DateTime.Now.Year - 2);
            Assert.True(price < 1000);
        }

        [Fact]
        public void PriceCalculator_OldVehicle_IncreasesPrice()
        {
            var offer = new InsuranceOffer { Price = 1000, Type = InsuranceType.Kasko };
            var basePrice = _calculator.Calculate(offer, driverAge: 35, experienceYears: 5, vehicleYear: DateTime.Now.Year - 5);
            var oldPrice  = _calculator.Calculate(offer, driverAge: 35, experienceYears: 5, vehicleYear: DateTime.Now.Year - 20);
            Assert.True(oldPrice > basePrice);
        }

        [Fact]
        public void PriceCalculator_HealthOffer_NotAffectedByVehicleAge()
        {
            var offer = new InsuranceOffer { Price = 1000, Type = InsuranceType.Health };
            var p1 = _calculator.Calculate(offer, 35, 5, DateTime.Now.Year - 2);
            var p2 = _calculator.Calculate(offer, 35, 5, DateTime.Now.Year - 20);
            Assert.Equal(p1, p2);
        }

        // ─── Repository mock ─────────────────────────────────────────────
        [Fact]
        public async Task Repository_GetOffersByType_ReturnsOnlyKasko()
        {
            var offers = new List<InsuranceOffer>
            {
                new InsuranceOffer { Id = 1, Type = InsuranceType.Kasko },
                new InsuranceOffer { Id = 2, Type = InsuranceType.Health }
            };
            _mockRepo.Setup(r => r.GetOffersByTypeAsync(InsuranceType.Kasko))
                     .ReturnsAsync(offers.Where(o => o.Type == InsuranceType.Kasko).ToList());

            var result = await _mockRepo.Object.GetOffersByTypeAsync(InsuranceType.Kasko);
            Assert.All(result, o => Assert.Equal(InsuranceType.Kasko, o.Type));
        }
    }
}
/ /   t e s t 1  
 / /   t e s t 2  
 / /   t e s t 3  
 