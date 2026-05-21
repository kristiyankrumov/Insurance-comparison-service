using InsuranceComparisonService.Models;
using InsuranceComparisonService.Services;
using Xunit;

namespace InsuranceComparisonService.Tests
{
    public class PriceCalculatorTests
    {
        private readonly PriceCalculatorService _calculator = new();

        private InsuranceOffer MakeOffer(decimal price, InsuranceType type = InsuranceType.Kasko)
            => new InsuranceOffer { Price = price, Type = type };

        // ─── Възраст ──────────────────────────────────────────────────────────

        [Fact]
        public void Calculate_YoungDriver_AddsAgeMultiplier()
        {
            var offer = MakeOffer(1000);
            var result = _calculator.Calculate(offer, driverAge: 22, experienceYears: 3, vehicleYear: 2020);
            Assert.True(result > 1000, "Млад водач трябва да плаща повече");
        }

        [Fact]
        public void Calculate_OlderDriver_AddsAgeMultiplier()
        {
            var offer = MakeOffer(1000);
            var result = _calculator.Calculate(offer, driverAge: 72, experienceYears: 30, vehicleYear: 2020);
            Assert.True(result > 1000, "По-възрастен водач трябва да плаща повече");
        }

        [Fact]
        public void Calculate_MiddleAgedExperiencedDriver_GetsDiscount()
        {
            var offer = MakeOffer(1000);
            var result = _calculator.Calculate(offer, driverAge: 40, experienceYears: 15, vehicleYear: 2020);
            Assert.True(result <= 1000, "Опитен средновъзрастен водач трябва да получи отстъпка или без надбавка");
        }

        // ─── Стаж ─────────────────────────────────────────────────────────────

        [Fact]
        public void Calculate_InexperiencedDriver_AddsExperienceMultiplier()
        {
            var offer = MakeOffer(1000);
            var result = _calculator.Calculate(offer, driverAge: 30, experienceYears: 1, vehicleYear: 2020);
            Assert.True(result > 1000, "Неопитен водач трябва да плаща повече");
        }

        [Fact]
        public void Calculate_ExperiencedDriver_GetsDiscount()
        {
            var offer = MakeOffer(1000);
            var result = _calculator.Calculate(offer, driverAge: 40, experienceYears: 15, vehicleYear: 2020);
            Assert.True(result < 1000, "Опитен водач трябва да получи отстъпка");
        }

        // ─── Година на МПС ────────────────────────────────────────────────────

        [Fact]
        public void Calculate_OldVehicleKasko_AddsSurcharge()
        {
            var offer = MakeOffer(1000, InsuranceType.Kasko);
            var oldYear = DateTime.Now.Year - 16;
            var result = _calculator.Calculate(offer, driverAge: 40, experienceYears: 10, vehicleYear: oldYear);
            Assert.True(result > 900, "Стар автомобил трябва да има надбавка при Каско");
        }

        [Fact]
        public void Calculate_NewVehicleKasko_GetsDiscount()
        {
            var offer = MakeOffer(1000, InsuranceType.Kasko);
            var newYear = DateTime.Now.Year - 1;
            var result = _calculator.Calculate(offer, driverAge: 40, experienceYears: 10, vehicleYear: newYear);
            Assert.True(result < 1000, "Нов автомобил трябва да получи отстъпка при Каско");
        }

        [Fact]
        public void Calculate_HealthInsurance_IgnoresVehicleAge()
        {
            var offer = MakeOffer(500, InsuranceType.Health);
            var oldYear = DateTime.Now.Year - 20;
            var newYear = DateTime.Now.Year - 1;
            var resultOld = _calculator.Calculate(offer, 40, 10, oldYear);
            var resultNew = _calculator.Calculate(offer, 40, 10, newYear);
            Assert.Equal(resultOld, resultNew);
        }

        // ─── ПТП история ──────────────────────────────────────────────────────

        [Fact]
        public void Calculate_OneAccident_AddsSurcharge()
        {
            var offer = MakeOffer(1000, InsuranceType.Kasko);
            var withoutAccident = _calculator.Calculate(offer, 35, 5, 2020, accidentCount: 0);
            var withAccident    = _calculator.Calculate(offer, 35, 5, 2020, accidentCount: 1);
            Assert.True(withAccident > withoutAccident, "1 ПТП трябва да увеличи цената");
        }

        [Fact]
        public void Calculate_TwoAccidents_AddMoreSurcharge()
        {
            var offer = MakeOffer(1000, InsuranceType.Kasko);
            var one = _calculator.Calculate(offer, 35, 5, 2020, accidentCount: 1);
            var two = _calculator.Calculate(offer, 35, 5, 2020, accidentCount: 2);
            Assert.True(two > one, "2 ПТП трябва да дадат по-висока цена от 1 ПТП");
        }

        [Fact]
        public void Calculate_AccidentsIgnoredForHealth()
        {
            var offer = MakeOffer(1000, InsuranceType.Health);
            var without = _calculator.Calculate(offer, 35, 5, 2020, accidentCount: 0);
            var withTwo = _calculator.Calculate(offer, 35, 5, 2020, accidentCount: 2);
            Assert.Equal(without, withTwo, "ПТП не трябва да влияят на Здравна застраховка");
        }

        // ─── Категория МПС ────────────────────────────────────────────────────

        [Fact]
        public void Calculate_Motorcycle_HigherThanCar()
        {
            var offer = MakeOffer(1000, InsuranceType.Kasko);
            var car  = _calculator.Calculate(offer, 35, 5, 2020, 0, VehicleCategory.Car);
            var moto = _calculator.Calculate(offer, 35, 5, 2020, 0, VehicleCategory.Motorcycle);
            Assert.True(moto > car, "Мотоциклет трябва да е по-скъп от лека кола");
        }

        [Fact]
        public void Calculate_Truck_HigherThanCar()
        {
            var offer = MakeOffer(1000, InsuranceType.Civil);
            var car   = _calculator.Calculate(offer, 35, 5, 2020, 0, VehicleCategory.Car);
            var truck = _calculator.Calculate(offer, 35, 5, 2020, 0, VehicleCategory.Truck);
            Assert.True(truck > car, "Камион трябва да е по-скъп от лека кола при ГО");
        }

        [Fact]
        public void Calculate_VehicleCategoryIgnoredForHealth()
        {
            var offer = MakeOffer(1000, InsuranceType.Health);
            var car  = _calculator.Calculate(offer, 35, 5, 2020, 0, VehicleCategory.Car);
            var bus  = _calculator.Calculate(offer, 35, 5, 2020, 0, VehicleCategory.Bus);
            Assert.Equal(car, bus, "Категорията МПС не трябва да влияе на Здравна застраховка");
        }

        // ─── Граница (floor) ──────────────────────────────────────────────────

        [Fact]
        public void Calculate_ResultAlwaysPositive()
        {
            var offer = MakeOffer(100);
            var result = _calculator.Calculate(offer, 30, 5, DateTime.Now.Year - 2);
            Assert.True(result > 0);
        }

        [Fact]
        public void Calculate_MaxDiscountFloor_NotBelow70Percent()
        {
            var offer = MakeOffer(1000, InsuranceType.Health);
            var result = _calculator.Calculate(offer, 40, 20, DateTime.Now.Year);
            Assert.True(result >= 700, "Цената никога не трябва да пада под 70% от базата");
        }
    }
}
