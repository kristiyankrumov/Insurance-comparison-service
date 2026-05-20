# Setup Instructions

## 1. Open the solution
Open `InsuranceComparison.sln` in Visual Studio 2022.

## 2. Run Migrations (REQUIRED - first time only)
In Visual Studio: Tools → NuGet Package Manager → Package Manager Console

Make sure the default project is set to `InsuranceComparisonService`, then run:

```
Add-Migration InitialCreate
Update-Database
```

Or via terminal in the `InsuranceComparisonService` folder:
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 3. Run the project
Press F5 or Ctrl+F5.

## Default admin credentials
- Admin: admin@insurance.bg / Admin123!
- SuperAdmin: superadmin@insurance.bg / SuperAdmin123!

## Database
Uses LocalDB (built into Visual Studio) - no separate SQL Server installation needed.
Connection string is already configured in appsettings.json.
