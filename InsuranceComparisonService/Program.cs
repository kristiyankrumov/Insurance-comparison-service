using InsuranceComparisonService.Data;
using InsuranceComparisonService.Middleware;
using InsuranceComparisonService.Models;
using InsuranceComparisonService.Repositories;
using InsuranceComparisonService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ── Порт за Render ────────────────────────────────────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// ── База данни ────────────────────────────────────────────────────────────────
// PRIORITY: Environment variable (Render) > appsettings.json > default SQLite
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=insurance.db";

Console.WriteLine($"[DB] Connection string source: {(Environment.GetEnvironmentVariable("DATABASE_URL") != null ? "DATABASE_URL env var" : "config file")}");

var isPostgres = !string.IsNullOrEmpty(connectionString) &&
                 (connectionString.Contains("Host=") ||
                  connectionString.Contains("postgres://") ||
                  connectionString.Contains("postgresql://") ||
                  (connectionString.Contains("@") && connectionString.Contains(":") && !connectionString.StartsWith("Data Source=")));

// Log database type
Console.WriteLine($"[DB] Using {(isPostgres ? "PostgreSQL" : "SQLite")} database");
if (!isPostgres)
{
    Console.WriteLine($"[DB] Connection string: {connectionString ?? "Data Source=insurance.db"}");
}

// Ensure SQLite directory exists (for local dev)
if (!isPostgres)
{
    var sqlite_conn = connectionString ?? "Data Source=insurance.db";
    if (sqlite_conn.Contains("Data Source="))
    {
        var dbPath = sqlite_conn.Split("Data Source=")[1].Split(";")[0];
        if (!dbPath.StartsWith(":"))
        {
            var dbDir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
        }
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (isPostgres)
    {
        options.UseNpgsql(ConvertPostgresUrlToConnectionString(connectionString));
    }
    else
    {
        options.UseSqlite(connectionString ?? "Data Source=insurance.db");
    }
});

// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── Услуги ────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
builder.Services.AddScoped<PriceCalculatorService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Фонова услуга за email напомняния
builder.Services.AddHostedService<PolicyExpiryReminderService>();

// ── Кеширане ──────────────────────────────────────────────────────────────────
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

// ── Rate Limiting ─────────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
    options.AddFixedWindowLimiter("login", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(15);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });
    options.RejectionStatusCode = 429;
});

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Застрахователна Платформа API",
        Version = "v1",
        Description = "API за сравнение на застрахователни оферти и управление на полици",
        Contact = new OpenApiContact
        {
            Name = "Insurance Comparison Service",
            Email = "support@insurance.bg"
        }
    });
});

builder.Services.AddControllersWithViews();

// ── Cookie настройки ──────────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // SameAsRequest за dev+prod
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// ── Инициализация на база данни ───────────────────────────────────────────────
await InitializeDatabaseAsync(app.Services);

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Swagger само в development среда
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Insurance API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=31536000";
    }
});

app.UseRouting();
app.UseRateLimiter();
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();

// ── Security headers ──────────────────────────────────────────────────────────
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ── Helpers ───────────────────────────────────────────────────────────────────
async Task InitializeDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Log pending migrations
        var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
        var pendingList = pendingMigrations.ToList();
        Console.WriteLine($"[DB] Pending migrations: {(pendingList.Any() ? string.Join(", ", pendingList) : "none")}");

        // Apply all pending migrations
        Console.WriteLine("[DB] Applying migrations...");
        await db.Database.MigrateAsync();
        Console.WriteLine("[DB] Migrations applied successfully");

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Роли
        foreach (var role in new[] { "SuperAdmin", "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Администратор
        var adminEmail = "admin@insurance.bg";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail, Email = adminEmail,
                FirstName = "Администратор", LastName = "Системен",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Супер Администратор
        var superAdminEmail = "superadmin@insurance.bg";
        if (await userManager.FindByEmailAsync(superAdminEmail) == null)
        {
            var superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail, Email = superAdminEmail,
                FirstName = "Супер", LastName = "Администратор",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(superAdmin, "SuperAdmin123!");
            if (result.Succeeded) await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
        }

        Console.WriteLine("[DB] Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "[DB] Error during database initialization");
        Console.WriteLine($"[DB] ERROR: {ex.Message}");
        Console.WriteLine($"[DB] StackTrace: {ex.StackTrace}");
        throw;
    }
}

string ConvertPostgresUrlToConnectionString(string url)
{
    if (string.IsNullOrWhiteSpace(url)) return "Host=localhost;Port=5432;Database=insurance;";
    
    // If already in connection string format (contains Host=), return as-is
    if (url.Contains("Host=") && url.Contains("Port=")) return url;
    
    // Handle postgresql:// and postgres:// URLs
    if (!url.StartsWith("postgres://") && !url.StartsWith("postgresql://"))
    {
        return url; // Assume it's already a connection string
    }

    try
    {
        var uri = new Uri(url);
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var dbPort = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        
        Console.WriteLine($"[DB] Converted PostgreSQL URL to connection string - Host: {host}, Port: {dbPort}, Database: {database}");

        return $"Host={host};Port={dbPort};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB] Error converting PostgreSQL URL: {ex.Message}");
        return url;
    }
}
