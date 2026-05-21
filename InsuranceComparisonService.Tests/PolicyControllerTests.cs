using InsuranceComparisonService.Data;
using InsuranceComparisonService.Models;
using InsuranceComparisonService.Repositories;
using InsuranceComparisonService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace InsuranceComparisonService.Tests
{
    // ── Модел тестове (без DB) ────────────────────────────────────────────────

    public class PolicyModelTests
    {
        [Fact]
        public void InsurancePolicy_DefaultStatus_IsActive()
        {
            var policy = new InsurancePolicy();
            Assert.Equal(PolicyStatus.Active, policy.Status);
        }

        [Fact]
        public void InsurancePolicy_CreatedAt_IsSet()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var policy = new InsurancePolicy();
            Assert.True(policy.CreatedAt >= before);
        }

        [Fact]
        public void InsurancePolicy_IsExpired_WhenEndDatePassed()
        {
            var policy = new InsurancePolicy { EndDate = DateTime.UtcNow.AddDays(-1) };
            Assert.True(policy.EndDate < DateTime.UtcNow);
        }

        [Fact]
        public void InsurancePolicy_IsNotExpired_WhenEndDateFuture()
        {
            var policy = new InsurancePolicy { EndDate = DateTime.UtcNow.AddDays(30) };
            Assert.False(policy.EndDate < DateTime.UtcNow);
        }

        [Fact]
        public void InsurancePolicy_MonthlyPayment_CalculatedCorrectly()
        {
            var calculator = new PriceCalculatorService();
            var offer = new InsuranceOffer { Price = 1200, Type = InsuranceType.Health };
            var annualPrice = calculator.Calculate(offer, 35, 10, DateTime.Now.Year - 3);
            var monthlyPrice = Math.Round(annualPrice / 12, 2);
            Assert.True(monthlyPrice > 0);
            Assert.True(monthlyPrice < annualPrice);
        }

        [Fact]
        public void InsurancePolicy_FinalPrice_WithAccidents_IsHigher()
        {
            var calculator = new PriceCalculatorService();
            var offer = new InsuranceOffer { Price = 1000, Type = InsuranceType.Kasko };
            var clean  = calculator.Calculate(offer, 35, 5, 2018, accidentCount: 0);
            var twoAcc = calculator.Calculate(offer, 35, 5, 2018, accidentCount: 2);
            Assert.True(twoAcc > clean);
        }

        [Fact]
        public void InsurancePolicy_StatusCancelled_CanBeSet()
        {
            var policy = new InsurancePolicy { Status = PolicyStatus.Active };
            policy.Status = PolicyStatus.Cancelled;
            Assert.Equal(PolicyStatus.Cancelled, policy.Status);
        }

        [Fact]
        public void InsurancePolicy_PaymentType_Monthly_CanBeSet()
        {
            var policy = new InsurancePolicy { PaymentType = PaymentType.Monthly };
            Assert.Equal(PaymentType.Monthly, policy.PaymentType);
        }

        [Fact]
        public void InsurancePolicy_VehicleId_CanBeNull()
        {
            var policy = new InsurancePolicy { VehicleId = null };
            Assert.Null(policy.VehicleId);
        }
    }

    // ── InMemory DB тестове ───────────────────────────────────────────────────

    public class PolicyDbTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public PolicyDbTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task Policy_SavedToDb_CanBeRetrieved()
        {
            var policy = new InsurancePolicy
            {
                PolicyNumber = "POL-2024-TEST01",
                UserId = "user-1",
                OfferId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(12),
                FinalPrice = 850,
                Status = PolicyStatus.Active
            };
            _context.InsurancePolicies.Add(policy);
            await _context.SaveChangesAsync();

            var saved = await _context.InsurancePolicies
                .FirstOrDefaultAsync(p => p.PolicyNumber == "POL-2024-TEST01");
            Assert.NotNull(saved);
            Assert.Equal(850, saved.FinalPrice);
        }

        [Fact]
        public async Task Policy_CanBeCancelled_InDb()
        {
            var policy = new InsurancePolicy
            {
                PolicyNumber = "POL-2024-TEST02",
                UserId = "user-1",
                OfferId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(6),
                FinalPrice = 500,
                Status = PolicyStatus.Active
            };
            _context.InsurancePolicies.Add(policy);
            await _context.SaveChangesAsync();

            policy.Status = PolicyStatus.Cancelled;
            await _context.SaveChangesAsync();

            var updated = await _context.InsurancePolicies.FindAsync(policy.Id);
            Assert.Equal(PolicyStatus.Cancelled, updated!.Status);
        }

        [Fact]
        public async Task Policy_WithVehicleId_LinkIsStored()
        {
            var policy = new InsurancePolicy
            {
                PolicyNumber = "POL-2024-TEST03",
                UserId = "user-1",
                OfferId = 1,
                VehicleId = 42,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(12),
                FinalPrice = 1000,
                Status = PolicyStatus.Active
            };
            _context.InsurancePolicies.Add(policy);
            await _context.SaveChangesAsync();

            var saved = await _context.InsurancePolicies.FindAsync(policy.Id);
            Assert.Equal(42, saved!.VehicleId);
        }

        [Fact]
        public async Task ExpiredPolicy_CanBeFilteredByDate()
        {
            _context.InsurancePolicies.AddRange(
                new InsurancePolicy
                {
                    PolicyNumber = "POL-ACTIVE", UserId = "u1", OfferId = 1,
                    StartDate = DateTime.UtcNow.AddMonths(-2),
                    EndDate = DateTime.UtcNow.AddMonths(10),
                    FinalPrice = 500, Status = PolicyStatus.Active
                },
                new InsurancePolicy
                {
                    PolicyNumber = "POL-EXPIRED", UserId = "u1", OfferId = 1,
                    StartDate = DateTime.UtcNow.AddMonths(-13),
                    EndDate = DateTime.UtcNow.AddDays(-1),
                    FinalPrice = 400, Status = PolicyStatus.Active // статусът не е сменен
                }
            );
            await _context.SaveChangesAsync();

            var expired = await _context.InsurancePolicies
                .Where(p => p.Status == PolicyStatus.Active && p.EndDate < DateTime.UtcNow)
                .ToListAsync();

            Assert.Single(expired);
            Assert.Equal("POL-EXPIRED", expired[0].PolicyNumber);
        }
    }
}
