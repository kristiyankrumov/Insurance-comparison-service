using InsuranceComparisonService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InsuranceComparisonService.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<InsuranceCompany> InsuranceCompanies { get; set; }
        public DbSet<InsuranceOffer> InsuranceOffers { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<InsurancePolicy> InsurancePolicies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<InsuranceOffer>()
                .Property(o => o.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<InsurancePolicy>()
                .Property(p => p.FinalPrice)
                .HasColumnType("decimal(18,2)");

            // Vehicle -> User (cascade delete)
            builder.Entity<Vehicle>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Policy -> Vehicle (set null when vehicle deleted)
            builder.Entity<InsurancePolicy>()
                .HasOne(p => p.Vehicle)
                .WithMany(v => v.Policies)
                .HasForeignKey(p => p.VehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Policy -> User
            builder.Entity<InsurancePolicy>()
                .HasOne(p => p.User)
                .WithMany(u => u.Policies)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Policy -> Offer (restrict delete)
            builder.Entity<InsurancePolicy>()
                .HasOne(p => p.Offer)
                .WithMany()
                .HasForeignKey(p => p.OfferId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed companies
            builder.Entity<InsuranceCompany>().HasData(
                new InsuranceCompany { Id = 1, Name = "ДЗИ", LogoUrl = "/images/dzi.png", Website = "https://dzi.bg", Description = "Водеща застрахователна компания в България" },
                new InsuranceCompany { Id = 2, Name = "Алианц България", LogoUrl = "/images/allianz.png", Website = "https://allianz.bg", Description = "Международна застрахователна група" },
                new InsuranceCompany { Id = 3, Name = "Лев Инс", LogoUrl = "/images/levins.png", Website = "https://levins.bg", Description = "Бързо развиваща се застрахователна компания" },
                new InsuranceCompany { Id = 4, Name = "Булстрад Виена Иншурънс", LogoUrl = "/images/bulstrad.png", Website = "https://bulstrad.bg", Description = "Bulstrad Vienna Insurance Group" }
            );

            // Seed offers
            builder.Entity<InsuranceOffer>().HasData(
                // Kasko
                new InsuranceOffer { Id = 1,  Title = "Каско Стандарт",      Description = "Пълно покритие на щети по автомобила",                              Type = InsuranceType.Kasko,    Price = 850,  Coverage = "Пълно Каско",                              Conditions = "Автомобили до 15 години", CompanyId = 1, IsActive = true },
                new InsuranceOffer { Id = 2,  Title = "Каско Комфорт",       Description = "Разширено покритие с пътна помощ",                                  Type = InsuranceType.Kasko,    Price = 1100, Coverage = "Пълно Каско + Пътна помощ 24/7",           Conditions = "Автомобили до 10 години", CompanyId = 2, IsActive = true },
                new InsuranceOffer { Id = 3,  Title = "Каско Икономи",       Description = "Икономично покритие за основни рискове",                            Type = InsuranceType.Kasko,    Price = 620,  Coverage = "Частично Каско",                           Conditions = "Автомобили до 20 години", CompanyId = 3, IsActive = true },
                new InsuranceOffer { Id = 4,  Title = "Каско Престиж",       Description = "Премиум покритие за скъпи автомобили",                              Type = InsuranceType.Kasko,    Price = 1800, Coverage = "Пълно Каско + Заместващ автомобил",        Conditions = "Автомобили до 5 години",  CompanyId = 4, IsActive = true },
                // Health
                new InsuranceOffer { Id = 5,  Title = "Здравна Базик",       Description = "Базова здравна застраховка за амбулаторно лечение",                 Type = InsuranceType.Health,   Price = 350,  Coverage = "Амбулаторно лечение",                     Conditions = "Лица от 18 до 75 години", CompanyId = 1, IsActive = true },
                new InsuranceOffer { Id = 6,  Title = "Здравна Плюс",        Description = "Разширена здравна застраховка с болнично лечение",                  Type = InsuranceType.Health,   Price = 680,  Coverage = "Амбулаторно + болнично лечение",           Conditions = "Лица от 18 до 70 години", CompanyId = 2, IsActive = true },
                new InsuranceOffer { Id = 7,  Title = "Здравна Семейна",     Description = "Застраховка за цялото семейство",                                   Type = InsuranceType.Health,   Price = 1200, Coverage = "Пълно медицинско покритие за семейство",   Conditions = "Семейства с деца",        CompanyId = 3, IsActive = true },
                new InsuranceOffer { Id = 8,  Title = "Здравна Премиум",     Description = "Пълно здравно покритие включително дентална помощ",                 Type = InsuranceType.Health,   Price = 1500, Coverage = "Пълно покритие + Дентална помощ",          Conditions = "Лица от 18 до 65 години", CompanyId = 4, IsActive = true },
                // Civil (Гражданска отговорност)
                new InsuranceOffer { Id = 9,  Title = "ГО Стандарт",         Description = "Задължителна гражданска отговорност - базово покритие",             Type = InsuranceType.Civil,    Price = 180,  Coverage = "До 10 млн. лв. за имуществени щети",       Conditions = "Всички МПС",              CompanyId = 1, IsActive = true },
                new InsuranceOffer { Id = 10, Title = "ГО Разширена",        Description = "Гражданска отговорност с разширено покритие",                       Type = InsuranceType.Civil,    Price = 250,  Coverage = "До 10 млн. лв. + правна защита",           Conditions = "Всички МПС",              CompanyId = 2, IsActive = true },
                new InsuranceOffer { Id = 11, Title = "ГО Икономи",          Description = "Най-изгодна гражданска отговорност",                                Type = InsuranceType.Civil,    Price = 150,  Coverage = "Минимално законово покритие",               Conditions = "МПС до 10 години",        CompanyId = 3, IsActive = true },
                new InsuranceOffer { Id = 12, Title = "ГО Премиум",          Description = "Пълна защита при ПТП с бонус за безаварийност",                     Type = InsuranceType.Civil,    Price = 320,  Coverage = "До 10 млн. лв. + пътна помощ + правна защита", Conditions = "Всички МПС",          CompanyId = 4, IsActive = true },
                // Property (Имуществена)
                new InsuranceOffer { Id = 13, Title = "Имуществена Дом",     Description = "Застраховка на жилище срещу пожар, наводнение и кражба",            Type = InsuranceType.Property, Price = 220,  Coverage = "Пожар, наводнение, кражба",                Conditions = "Жилищни имоти",           CompanyId = 1, IsActive = true },
                new InsuranceOffer { Id = 14, Title = "Имуществена Вила",    Description = "Разширена застраховка за вила или къща",                            Type = InsuranceType.Property, Price = 380,  Coverage = "Пожар, наводнение, кражба, земетресение",  Conditions = "Жилищни и вилни имоти",   CompanyId = 2, IsActive = true },
                new InsuranceOffer { Id = 15, Title = "Имуществена Апарт",   Description = "Икономична застраховка за апартамент",                              Type = InsuranceType.Property, Price = 160,  Coverage = "Пожар и природни бедствия",                Conditions = "Апартаменти",             CompanyId = 3, IsActive = true },
                new InsuranceOffer { Id = 16, Title = "Имуществена Премиум", Description = "Пълно покритие на имущество с гражданска отговорност на собственика", Type = InsuranceType.Property, Price = 550, Coverage = "Пълно покритие + ГО на собственик",        Conditions = "Всички жилищни имоти",    CompanyId = 4, IsActive = true }
            );
        }
    }
}
