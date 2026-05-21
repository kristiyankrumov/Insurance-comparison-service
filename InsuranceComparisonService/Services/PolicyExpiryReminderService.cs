using InsuranceComparisonService.Data;
using InsuranceComparisonService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceComparisonService.Services
{
    /// <summary>
    /// Фонова услуга, която всеки ден проверява за полици,
    /// изтичащи след 30, 14 или 7 дни, и изпраща email напомняния.
    /// </summary>
    public class PolicyExpiryReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PolicyExpiryReminderService> _logger;

        // Изпълнява се веднъж на 24 часа
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(24);

        public PolicyExpiryReminderService(
            IServiceScopeFactory scopeFactory,
            ILogger<PolicyExpiryReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PolicyExpiryReminderService стартира.");

            // Изчакваме малко след стартиране на апликацията
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendRemindersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Грешка при изпращане на напомняния за изтичащи полици.");
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task SendRemindersAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var now = DateTime.UtcNow;
            // Напомняния за 30, 14 и 7 дни преди изтичане
            var reminderDays = new[] { 30, 14, 7 };

            foreach (var days in reminderDays)
            {
                var targetDate = now.AddDays(days);
                var windowStart = targetDate.Date;
                var windowEnd   = windowStart.AddDays(1);

                var policies = await db.InsurancePolicies
                    .Include(p => p.User)
                    .Where(p =>
                        p.Status == PolicyStatus.Active &&
                        p.EndDate >= windowStart &&
                        p.EndDate < windowEnd)
                    .ToListAsync(ct);

                foreach (var policy in policies)
                {
                    if (policy.User?.Email == null) continue;

                    await emailService.SendPolicyExpiryReminderAsync(
                        policy.User.Email,
                        policy.PolicyNumber,
                        policy.EndDate);

                    _logger.LogInformation(
                        "Напомняне изпратено до {Email} за полица {PolicyNumber} (изтича {EndDate:dd.MM.yyyy})",
                        policy.User.Email, policy.PolicyNumber, policy.EndDate);
                }
            }
        }
    }
}
