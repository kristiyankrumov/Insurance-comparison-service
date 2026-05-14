using InsuranceComparisonService.Data;
using InsuranceComparisonService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceComparisonService.Repositories
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly ApplicationDbContext _context;

        public InsuranceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InsuranceOffer>> GetAllOffersAsync()
        {
            return await _context.InsuranceOffers
                .Include(o => o.Company)
                .Where(o => o.IsActive)
                .OrderBy(o => o.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<InsuranceOffer>> GetOffersByTypeAsync(InsuranceType type)
        {
            return await _context.InsuranceOffers
                .Include(o => o.Company)
                .Where(o => o.Type == type && o.IsActive)
                .OrderBy(o => o.Price)
                .ToListAsync();
        }

        public async Task<InsuranceOffer?> GetOfferByIdAsync(int id)
        {
            return await _context.InsuranceOffers
                .Include(o => o.Company)
                .Include(o => o.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddOfferAsync(InsuranceOffer offer)
        {
            _context.InsuranceOffers.Add(offer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOfferAsync(InsuranceOffer offer)
        {
            _context.InsuranceOffers.Update(offer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOfferAsync(int id)
        {
            var offer = await _context.InsuranceOffers.FindAsync(id);
            if (offer != null)
            {
                offer.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<InsuranceCompany>> GetAllCompaniesAsync()
        {
            return await _context.InsuranceCompanies.ToListAsync();
        }
    }
}
