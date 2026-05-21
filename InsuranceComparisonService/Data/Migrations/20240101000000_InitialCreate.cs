using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceComparisonService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── ASP.NET Core Identity tables ──────────────────────────────────

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_AspNetRoles", x => x.Id));

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: true),
                    DrivingExperienceYears = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // ── Application tables ────────────────────────────────────────────

            migrationBuilder.CreateTable(
                name: "InsuranceCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Website = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_InsuranceCompanies", x => x.Id));

            migrationBuilder.CreateTable(
                name: "InsuranceOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Coverage = table.Column<string>(type: "TEXT", nullable: false),
                    Conditions = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceOffers_InsuranceCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "InsuranceCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Make = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    PlateNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    HorsePower = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelType = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    OfferId = table.Column<int>(type: "INTEGER", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_InsuranceOffers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "InsuranceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    OfferId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_InsuranceOffers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "InsuranceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OfferId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentType = table.Column<int>(type: "INTEGER", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PolicyNumber = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_InsuranceOffers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "InsuranceOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            // ── Indexes ───────────────────────────────────────────────────────

            migrationBuilder.CreateIndex("IX_AspNetRoleClaims_RoleId", "AspNetRoleClaims", "RoleId");
            migrationBuilder.CreateIndex("RoleNameIndex", "AspNetRoles", "NormalizedName", unique: true);
            migrationBuilder.CreateIndex("IX_AspNetUserClaims_UserId", "AspNetUserClaims", "UserId");
            migrationBuilder.CreateIndex("IX_AspNetUserLogins_UserId", "AspNetUserLogins", "UserId");
            migrationBuilder.CreateIndex("IX_AspNetUserRoles_RoleId", "AspNetUserRoles", "RoleId");
            migrationBuilder.CreateIndex("EmailIndex", "AspNetUsers", "NormalizedEmail");
            migrationBuilder.CreateIndex("UserNameIndex", "AspNetUsers", "NormalizedUserName", unique: true);
            migrationBuilder.CreateIndex("IX_InsuranceOffers_CompanyId", "InsuranceOffers", "CompanyId");
            migrationBuilder.CreateIndex("IX_InsurancePolicies_OfferId", "InsurancePolicies", "OfferId");
            migrationBuilder.CreateIndex("IX_InsurancePolicies_UserId", "InsurancePolicies", "UserId");
            migrationBuilder.CreateIndex("IX_InsurancePolicies_VehicleId", "InsurancePolicies", "VehicleId");
            migrationBuilder.CreateIndex("IX_Reviews_OfferId", "Reviews", "OfferId");
            migrationBuilder.CreateIndex("IX_Reviews_UserId", "Reviews", "UserId");
            migrationBuilder.CreateIndex("IX_UserFavorites_OfferId", "UserFavorites", "OfferId");
            migrationBuilder.CreateIndex("IX_UserFavorites_UserId", "UserFavorites", "UserId");
            migrationBuilder.CreateIndex("IX_Vehicles_UserId", "Vehicles", "UserId");

            // ── Seed data ─────────────────────────────────────────────────────

            migrationBuilder.InsertData(
                table: "InsuranceCompanies",
                columns: new[] { "Id", "Name", "LogoUrl", "Website", "Description" },
                values: new object[,]
                {
                    { 1, "ДЗИ", "/images/dzi.png", "https://dzi.bg", "Водеща застрахователна компания в България" },
                    { 2, "Алианц България", "/images/allianz.png", "https://allianz.bg", "Международна застрахователна група" },
                    { 3, "Лев Инс", "/images/levins.png", "https://levins.bg", "Бързо развиваща се застрахователна компания" },
                    { 4, "Булстрад Виена Иншурънс", "/images/bulstrad.png", "https://bulstrad.bg", "Bulstrad Vienna Insurance Group" }
                });

            migrationBuilder.InsertData(
                table: "InsuranceOffers",
                columns: new[] { "Id", "Title", "Description", "Type", "Price", "Coverage", "Conditions", "CompanyId", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    // Kasko (Type=0)
                    { 1,  "Каско Стандарт",      "Пълно покритие на щети по автомобила",                              0, 850m,  "Пълно Каско",                              "Автомобили до 15 години", 1, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 2,  "Каско Комфорт",        "Разширено покритие с пътна помощ",                                  0, 1100m, "Пълно Каско + Пътна помощ 24/7",           "Автомобили до 10 години", 2, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 3,  "Каско Икономи",        "Икономично покритие за основни рискове",                            0, 620m,  "Частично Каско",                           "Автомобили до 20 години", 3, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 4,  "Каско Престиж",        "Премиум покритие за скъпи автомобили",                              0, 1800m, "Пълно Каско + Заместващ автомобил",        "Автомобили до 5 години",  4, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    // Health (Type=1)
                    { 5,  "Здравна Базик",        "Базова здравна застраховка за амбулаторно лечение",                 1, 350m,  "Амбулаторно лечение",                      "Лица от 18 до 75 години", 1, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 6,  "Здравна Плюс",         "Разширена здравна застраховка с болнично лечение",                  1, 680m,  "Амбулаторно + болнично лечение",           "Лица от 18 до 70 години", 2, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 7,  "Здравна Семейна",      "Застраховка за цялото семейство",                                   1, 1200m, "Пълно медицинско покритие за семейство",   "Семейства с деца",        3, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 8,  "Здравна Премиум",      "Пълно здравно покритие включително дентална помощ",                 1, 1500m, "Пълно покритие + Дентална помощ",          "Лица от 18 до 65 години", 4, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    // Civil (Type=2)
                    { 9,  "ГО Стандарт",          "Задължителна гражданска отговорност - базово покритие",             2, 180m,  "До 10 млн. лв. за имуществени щети",       "Всички МПС",              1, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 10, "ГО Разширена",          "Гражданска отговорност с разширено покритие",                       2, 250m,  "До 10 млн. лв. + правна защита",           "Всички МПС",              2, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 11, "ГО Икономи",            "Най-изгодна гражданска отговорност",                                2, 150m,  "Минимално законово покритие",              "МПС до 10 години",        3, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 12, "ГО Премиум",            "Пълна защита при ПТП с бонус за безаварийност",                     2, 320m,  "До 10 млн. лв. + пътна помощ + правна защита", "Всички МПС",         4, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    // Property (Type=3)
                    { 13, "Имуществена Дом",      "Застраховка на жилище срещу пожар, наводнение и кражба",            3, 220m,  "Пожар, наводнение, кражба",                "Жилищни имоти",           1, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 14, "Имуществена Вила",     "Разширена застраховка за вила или къща",                            3, 380m,  "Пожар, наводнение, кражба, земетресение",  "Жилищни и вилни имоти",   2, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 15, "Имуществена Апарт",    "Икономична застраховка за апартамент",                              3, 160m,  "Пожар и природни бедствия",                "Апартаменти",             3, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
                    { 16, "Имуществена Премиум",  "Пълно покритие на имущество с гражданска отговорност на собственика",3, 550m,  "Пълно покритие + ГО на собственик",        "Всички жилищни имоти",    4, true, new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("InsurancePolicies");
            migrationBuilder.DropTable("Reviews");
            migrationBuilder.DropTable("UserFavorites");
            migrationBuilder.DropTable("AspNetRoleClaims");
            migrationBuilder.DropTable("AspNetUserClaims");
            migrationBuilder.DropTable("AspNetUserLogins");
            migrationBuilder.DropTable("AspNetUserRoles");
            migrationBuilder.DropTable("AspNetUserTokens");
            migrationBuilder.DropTable("Vehicles");
            migrationBuilder.DropTable("InsuranceOffers");
            migrationBuilder.DropTable("InsuranceCompanies");
            migrationBuilder.DropTable("AspNetRoles");
            migrationBuilder.DropTable("AspNetUsers");
        }
    }
}
