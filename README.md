# Застрахователна Платформа / Insurance Comparison Service

Уеб платформа за сравнение на застрахователни оферти, разработена с ASP.NET Core 8 MVC.

## Технологии

- **Backend:** ASP.NET Core 8 MVC, C# 12
- **База данни:** SQLite (dev) / PostgreSQL (prod) — Entity Framework Core 8 (Code First)
- **Автентикация:** ASP.NET Core Identity
- **Frontend:** Bootstrap 5, Razor Views
- **Плащания:** Stripe, PayPal
- **Email:** SMTP (Gmail / SendGrid)
- **Тестове:** xUnit, Moq, EF InMemory (25+ теста)
- **CI/CD:** GitHub Actions
- **Документация:** Swagger / OpenAPI (достъпна на `/swagger` в Development)
- **Архитектура:** Repository Pattern, MVC, Dependency Injection, Background Services

## Функционалности

- Преглед и сравнение на застраховки (Каско, Здравна, ГО, Имуществена)
- **Динамично изчисляване на цена** спрямо:
  - Възраст на водача
  - Шофьорски стаж
  - Година и категория на МПС (лека кола / мотоциклет / камион / автобус)
  - История на ПТП (брой катастрофи)
- Регистрация и вход с ASP.NET Identity
- Управление на застрахователни полици (купуване, анулиране, преглед)
- Връзка между МПС и полица
- Плащане чрез Stripe и PayPal
- Запазване на любими оферти
- Административен панел (CRUD за оферти и управление на потребители)
- **Email известия:** потвърждение при покупка + автоматични напомняния (30/14/7 дни преди изтичане)
- Rate limiting и security headers
- Health check endpoint (`/health`)
- Swagger документация (`/swagger` в dev среда)

## Инсталация

### Предварителни изисквания

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) или [VS Code](https://code.visualstudio.com/)

### Стъпки

1. Клонирайте хранилището:
   ```bash
   git clone https://github.com/YOUR_USERNAME/Insurance-comparison-service.git
   cd Insurance-comparison-service
   ```

2. Конфигурирайте чувствителните данни чрез **User Secrets** (не ги слагайте в appsettings.json!):
   ```bash
   cd InsuranceComparisonService
   dotnet user-secrets set "Stripe:SecretKey" "sk_test_ВАШИЯ_КЛЮЧ"
   dotnet user-secrets set "Stripe:PublishableKey" "pk_test_ВАШИЯ_КЛЮЧ"
   dotnet user-secrets set "PayPal:ClientId" "ВАШИЯ_CLIENT_ID"
   dotnet user-secrets set "PayPal:Secret" "ВАШИЯ_SECRET"
   dotnet user-secrets set "Email:FromAddress" "вашия@gmail.com"
   dotnet user-secrets set "Email:Password" "ВАШАТА_APP_ПАРОЛА"
   ```

3. Стартирайте приложението (миграциите се прилагат автоматично):
   ```bash
   dotnet run
   ```

4. Отворете браузъра на `https://localhost:5001`

### Администраторски акаунти

При първото стартиране автоматично се създават:

| Email | Парола | Роля |
|-------|--------|------|
| `admin@insurance.bg` | `Admin123!` | Admin |
| `superadmin@insurance.bg` | `SuperAdmin123!` | SuperAdmin |

## Структура на проекта

```
InsuranceComparisonService/
├── .github/workflows/       # CI/CD (GitHub Actions)
├── Areas/Admin/             # Административен панел
├── Controllers/             # MVC контролери
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/          # EF Core миграции
├── Middleware/              # GlobalExceptionMiddleware
├── Models/                  # Entity модели + ViewModels
├── Repositories/            # Repository Pattern
├── Services/
│   ├── PriceCalculatorService.cs    # Динамично ценообразуване
│   ├── EmailService.cs              # SMTP email
│   ├── IEmailService.cs
│   └── PolicyExpiryReminderService.cs  # Фонова услуга за напомняния
├── Views/                   # Razor изгледи
└── wwwroot/                 # Статични файлове
InsuranceComparisonService.Tests/
├── InsuranceControllerTests.cs   # Тестове за контролери
├── PolicyControllerTests.cs      # Тестове за полици + InMemory DB
└── PriceCalculatorTests.cs       # Тестове за ценообразуване (вкл. ПТП, категория МПС)
```

## Тестване

```bash
cd InsuranceComparisonService.Tests
dotnet test
```

Покрити сценарии:
- InsuranceController (Kasko/Health/Civil/Property, филтри, Compare, Details)
- PriceCalculatorService (възраст, стаж, година МПС, категория МПС, ПТП история, граница)
- PolicyController (модел тестове, InMemory DB тестове)

## CI/CD

GitHub Actions workflow (`.github/workflows/ci.yml`) се изпълнява при всеки push:
1. Компилация в Release режим
2. Изпълнение на всички тестове
3. Публикуване на артефакт (само при merge в main)

## Деплой

Проектът е конфигуриран за деплой в [Render](https://render.com) чрез `render.yaml` и `Dockerfile`.
При деплой задайте environment variables за Stripe, PayPal и Email в таблото на Render.

## Сигурност

- HTTPS задължително в production
- Rate limiting (100 req/min глобално, 5 опита за вход/15 мин)
- Account lockout след 5 неуспешни опита
- Security headers (X-Frame-Options, X-XSS-Protection, X-Content-Type-Options)
- HttpOnly и Secure cookies
- Anti-CSRF токени на всички форми
- Параметризирани SQL заявки (EF Core)
- **Чувствителни данни само в User Secrets / environment variables, не в appsettings.json**
