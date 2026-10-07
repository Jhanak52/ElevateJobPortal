# Elevate Workforce Solutions - Job Portal

ASP.NET Core MVC (.NET 10) + Entity Framework Core + PostgreSQL (Docker) + ASP.NET Identity (cookie auth, roles).

## Run it

```bash
# 1. start PostgreSQL
docker compose up -d

# 2. run the web app
cd ElevateJobPortal.Web
dotnet run --launch-profile http      # opens http://localhost:5080
```

On first start in Development the app creates the database, the two roles and demo data (24 jobs, so pagination is visible).

### Demo accounts (password for all: `Password@123`)
| Role | Email |
|---|---|
| Employer (Himalayan Tech) | employer1@elevate.com |
| Employer (Everest Hospitality) | employer2@elevate.com |
| Job seeker | seeker@elevate.com |

### Optional: use EF Core migrations (recommended for your report evidence)
```bash
dotnet tool install --global dotnet-ef
cd ElevateJobPortal.Web
dotnet ef migrations add InitialCreate
dotnet ef database update
```
If no migrations exist, the app falls back to `EnsureCreated()`. If you add migrations later, drop the dev database first (`docker compose down -v`).

## Brief requirement -> where it is implemented
| Requirement | Files |
|---|---|
| Registration / login / logout | `Controllers/AccountController.cs`, `Services/AccountService.cs`, `Views/Account/*`, Identity in `Program.cs` |
| All jobs with pagination (+ search/filter) | `Controllers/JobsController.cs` (Index), `Services/JobService.cs`, `Helpers/PaginatedList.cs`, `Views/Jobs/Index.cshtml`, `Views/Shared/_Pager.cshtml` |
| Company add / update / delete own jobs | `JobsController` (Create, Edit, Delete) - role `Employer`, ownership enforced in `JobService` by `CompanyId` |
| Applications on company dashboard | `Controllers/DashboardController.cs`, `Views/Dashboard/*`, `Services/ApplicationService.cs` |
| Job seekers apply and track status | `Controllers/ApplicationsController.cs`, `Views/Applications/*` |

## MVC and OOP summary
- **Model**: `Models/` (entities, `BaseEntity` inheritance), `Data/ApplicationDbContext.cs`
- **View**: `Views/` (Razor) with strongly typed `ViewModels/`
- **Controller**: `Controllers/` (thin, inherit `BaseController`)
- **OOP**: inheritance (`BaseEntity`, `BaseController`, `ApplicationUser : IdentityUser`), interfaces + dependency injection (`I*Service`), encapsulation (`Job.IsExpired`, `Job.SalaryRange`), generics (`PaginatedList<T>`), polymorphism via interfaces, validation via `IValidatableObject`.

## Security notes
Passwords hashed by Identity; anti-forgery tokens on every POST; role + ownership checks on all employer actions; account lockout after 5 failed logins; open-redirect protection on login. Demo passwords and the DB password in `docker-compose.yml` are for local development only.
