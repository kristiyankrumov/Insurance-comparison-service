using InsuranceComparisonService.Models;

namespace InsuranceComparisonService.Repositories
{
    public interface IInsuranceRepository
    {
        Task<IEnumerable<InsuranceOffer>> GetAllOffersAsync();
        Task<IEnumerable<InsuranceOffer>> GetOffersByTypeAsync(InsuranceType type);
        Task<InsuranceOffer?> GetOfferByIdAsync(int id);
        Task AddOfferAsync(InsuranceOffer offer);
        Task UpdateOfferAsync(InsuranceOffer offer);
        Task DeleteOfferAsync(int id);
        Task<IEnumerable<InsuranceCompany>> GetAllCompaniesAsync();
    }
}
