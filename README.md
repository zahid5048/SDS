# Chemical SDS System

Chemical Safety Data Sheet (SDS) management for **C.I.W.C&E — DGLW Govt of Punjab**.

ASP.NET Core MVC app with 16-section SDS wizard, dashboard, PDF export, and user management.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB / Express / full)
- Laragon or IIS Express (optional)

## Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/zahid5048/SDS.git
   cd SDS
   ```

2. Copy configuration (not in Git — contains your local DB connection):
   ```bash
   copy appsettings.Example.json appsettings.json
   ```
   Edit `appsettings.json` and set your `DefaultConnection` string.

3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

4. Run the app:
   ```bash
   dotnet run
   ```
   Default URL: `http://localhost:5020`

5. First login (seeded on first run):
   - **Username:** `admin`
   - **Password:** `admin123`
   - Complete the math CAPTCHA on the login page.

## Features

- Login with CAPTCHA
- Dashboard with stats and Chart.js charts
- All chemicals list with live search, sort, pagination
- 16-section SDS wizard
- SDS details view and PDF download
- User management (Admin)

## Project structure

| Folder | Purpose |
|--------|---------|
| `Controllers/` | MVC controllers |
| `Views/` | Razor views |
| `Services/` | Business logic |
| `Data/` | EF Core `DbContext` |
| `Migrations/` | Database migrations |
| `wwwroot/` | CSS, JS, static files |

## Git — ignored files

See `.gitignore`. These are **not** pushed to GitHub:

- `bin/`, `obj/` — build output
- `appsettings.json` — your local database connection
- `appsettings.Development.json` — local overrides
- IDE folders (`.vs/`, etc.)

Use `appsettings.Example.json` as a template.

## License

Government / internal use — DGJW Punjab.
