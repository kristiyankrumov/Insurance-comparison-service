using InsuranceComparisonService.Models;

namespace InsuranceComparisonService.Services
{
    public class PriceCalculatorService
    {
        /// <summary>
        /// Изчислява крайната цена на застраховка спрямо лични данни на водача/МПС.
        /// Фактори: възраст на водача, стаж, година на МПС, категория МПС, брой ПТП.
        /// </summary>
        public decimal Calculate(
            InsuranceOffer offer,
            int driverAge,
            int experienceYears,
            int vehicleYear,
            int accidentCount = 0,
            VehicleCategory vehicleCategory = VehicleCategory.Car)
        {
            decimal basePrice = offer.Price;
            decimal multiplier = 1.0m;

            // ── Възраст на водача ─────────────────────────────────────────────
            if (driverAge < 25)
                multiplier += 0.25m;   // млади водачи +25%
            else if (driverAge >= 70)
                multiplier += 0.15m;   // по-възрастни +15%

            // ── Шофьорски стаж ────────────────────────────────────────────────
            if (experienceYears < 2)
                multiplier += 0.30m;   // без опит +30%
            else if (experienceYears >= 10)
                multiplier -= 0.10m;   // опитни -10%

            // ── Година на МПС (само Каско и ГО) ──────────────────────────────
            int vehicleAge = DateTime.Now.Year - vehicleYear;
            if (offer.Type == InsuranceType.Kasko || offer.Type == InsuranceType.Civil)
            {
                if (vehicleAge > 15)
                    multiplier += 0.20m;   // стар автомобил +20%
                else if (vehicleAge <= 3)
                    multiplier -= 0.05m;   // нов автомобил -5%
            }

            // ── Категория МПС (само Каско и ГО) ──────────────────────────────
            if (offer.Type == InsuranceType.Kasko || offer.Type == InsuranceType.Civil)
            {
                multiplier += vehicleCategory switch
                {
                    VehicleCategory.Motorcycle => 0.15m,  // мотоциклети +15% (по-рискови)
                    VehicleCategory.Truck      => 0.25m,  // камиони +25%
                    VehicleCategory.Bus        => 0.30m,  // автобуси +30%
                    _                          => 0.0m    // леки коли - без допълнение
                };
            }

            // ── История на ПТП ────────────────────────────────────────────────
            // Важи за Каско и ГО; всяко ПТП добавя 15%, без горна граница
            if ((offer.Type == InsuranceType.Kasko || offer.Type == InsuranceType.Civil)
                && accidentCount > 0)
            {
                multiplier += accidentCount * 0.15m;
            }

            // ── Граница: максимум 30% отстъпка (floor 0.70) ──────────────────
            if (multiplier < 0.70m) multiplier = 0.70m;

            return Math.Round(basePrice * multiplier, 2);
        }
    }
}
