# BookReviewApp

A layered .NET 9 solution for managing books and user reviews with Web (MVC) and REST API projects, Entity Framework Core, ASP.NET Core Identity, and JWT authentication.

## Projects
- **BookReviewApp.Domain**: Entities (`Book`, `Review`, `ReviewVote`, `AppUser`).
- **BookReviewApp.DataAccess**: `Context` (EF Core + Identity), entity mappings, migrations.
- **BookReviewApp.Core**: Services, validators, result models, interfaces.
- **BookReviewApp.Api**: JSON REST API with JWT auth and Swagger.
- **BookReviewApp.Web**: MVC app for browsing/creating books and reviews.
- **BookReviewApp.Tests**: Unit and controller tests.

## Configuration
Both `Api` and `Web` read connection strings and JWT settings from `appsettings.*.json`.

- Set `ConnectionStrings:DefaultConnection` to a reachable SQL Server.
- Configure `JwtConfig` in `BookReviewApp.Api`:
  - `Key` (strong secret), `Issuer`, `Audience`.

Example (Api `appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BookReviewApp;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "JwtConfig": {
    "Key": "dev-secret-change-me",
    "Issuer": "BookReviewApp",
    "Audience": "BookReviewAppUsers"
  }
}
```

## Automatic migrations and seeding (Web)
The Web app applies pending migrations and seeds initial data at startup.
- Hooked in `Program.cs` via:
  - `await app.ApplyMigrationsAndSeedAsync();`
- Implementation lives in `BookReviewApp.Web/Extensions/ApplicationBuilderExtensions.cs` and uses `DbSeeder.SeedAsync`.

## Authentication (API)
- **API**: Login endpoints are exposed in `AuthController`. After login, use the returned JWT in Swagger authorize modal for protected endpoints (books/reviews).
- **Web**: Uses ASP.NET Core Identity for user registration, login, and authentication.

## Solution structure (high-level)
```
BookReviewApp.sln
├─ BookReviewApp.Domain
├─ BookReviewApp.DataAccess
├─ BookReviewApp.Core
├─ BookReviewApp.Api
├─ BookReviewApp.Web
└─ BookReviewApp.Tests
```

## API quick reference
- `POST /api/auth/login`
- `GET /api/books`
- `GET /api/books/{id}`
- `POST /api/books`
- `GET /api/books/{bookId}/reviews`
- `POST /api/reviews`
- `GET /api/reviews/{id}`
- `POST /api/reviews/{id}/vote`

Swagger UI is available when running the API in Development.