using InsuranceComparisonService.Models;

namespace InsuranceComparisonService.Services
{
    public class PriceCalculatorService
    {
        /// <summary>
        /// Изчислява крайната цена на застраховка спрямо лични данни на водача/МПС.
        /// </summary>
        public decimal Calculate(InsuranceOffer offer, int driverAge, int experienceYears, int vehicleYear)
        {
            decimal basePrice = offer.Price;
            decimal multiplier = 1.0m;

            // Age factor
            if (driverAge < 25)
                multiplier += 0.25m;   // млади водачи +25%
            else if (driverAge >= 70)
                multiplier += 0.15m;   // по-възрастни +15%

            // Experience factor
            if (experienceYears < 2)
                multiplier += 0.30m;   // без опит +30%
            else if (experienceYears >= 10)
                multiplier -= 0.10m;   // опитни -10%

            // Vehicle age factor (Kasko / Civil only)
            int vehicleAge = DateTime.Now.Year - vehicleYear;
            if (offer.Type == InsuranceType.Kasko || offer.Type == InsuranceType.Civil)
            {
                if (vehicleAge > 15)
                    multiplier += 0.20m;   // стар автомобил +20%
                else if (vehicleAge <= 3)
                    multiplier -= 0.05m;   // нов автомобил -5%
            }

            // Floor at 0.7 (max 30% discount)
            if (multiplier < 0.7m) multiplier = 0.7m;

            return Math.Round(basePrice * multiplier, 2);
        }
    }
}
