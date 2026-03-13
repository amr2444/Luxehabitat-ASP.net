<div align="center">

<br/>

```
██╗     ██╗   ██╗██╗  ██╗███████╗██╗  ██╗ █████╗ ██████╗ ██╗████████╗ █████╗ ████████╗
██║     ██║   ██║╚██╗██╔╝██╔════╝██║  ██║██╔══██╗██╔══██╗██║╚══██╔══╝██╔══██╗╚══██╔══╝
██║     ██║   ██║ ╚███╔╝ █████╗  ███████║███████║██████╔╝██║   ██║   ███████║   ██║   
██║     ██║   ██║ ██╔██╗ ██╔══╝  ██╔══██║██╔══██║██╔══██╗██║   ██║   ██╔══██║   ██║   
███████╗╚██████╔╝██╔╝ ██╗███████╗██║  ██║██║  ██║██████╔╝██║   ██║   ██║  ██║   ██║   
╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚═╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   
```

### *A luxury real estate platform built with editorial precision*

<br/>

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/aspnet/core)
[![Entity Framework](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/ef/core)
[![SQL Server](https://img.shields.io/badge/SQL_Server-LocalDB-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-gold?style=for-the-badge)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![xUnit](https://img.shields.io/badge/Tests-xUnit-5E4EB5?style=for-the-badge&logo=xunit&logoColor=white)](https://xunit.net/)

<br/>

> **LuxeHabitat** is not just another property listing app.  
> It is a full-stack, production-grade real estate platform crafted with  
> Clean Architecture, role-based access control, and an immersive dark luxury UI  
> that rivals the world's finest property portals.

<br/>

---

</div>

<br/>

## ✦ What is LuxeHabitat?

LuxeHabitat is a **complete real estate management platform** built on ASP.NET Core 9, designed to handle the full lifecycle of a property — from listing creation to lease contract signing.

It supports three distinct user roles, each with their own dashboard, workflows, and permissions:

| Role | What they do |
|------|-------------|
| 🏢 **Agent** | Create and manage property listings, handle inquiries, approve visits, review rental applications |
| 🏠 **Tenant** | Search properties, save favorites, request visits, submit rental applications, leave reviews |
| 🔑 **Admin** | Oversee the entire platform, moderate listings, manage users, approve reviews |

Every interaction flows through a carefully designed service layer, enforced by ownership checks and policy-based authorization — so no user can ever touch data that isn't theirs.

<br/>

---

## ✦ Architecture Overview

LuxeHabitat follows a strict **Clean Architecture** pattern, organized into four independent layers:

```
📦 RealEstate Solution
│
├── 🌐 RealEstate.Web              → ASP.NET Core MVC (Controllers, Views, ViewModels)
│   ├── Controllers/               → 7 controllers + Admin area
│   ├── Views/                     → Razor pages with dark luxury UI
│   ├── Policies/                  → RBAC + ResourceOwnership authorization
│   └── ViewComponents/            → NotificationPanel component
│
├── ⚙️  RealEstate.Application      → Business Logic (no framework dependencies)
│   ├── Services/                  → 12 service classes
│   ├── Interfaces/                → IPropertyService, ITenantService, etc.
│   ├── DTOs/                      → Clean data contracts per feature
│   └── Exceptions/                → AppException hierarchy
│
├── 🗄️  RealEstate.Domain           → Core Entities & Enums (zero dependencies)
│   ├── Entities/                  → 18 domain entities
│   ├── Enums/                     → PropertyType, Status, ListingType...
│   └── Common/                    → AuditableEntity, BaseEntity
│
├── 🔧 RealEstate.Infrastructure    → EF Core, Identity, File Storage, Migrations
│   ├── Persistence/               → ApplicationDbContext + 18 Fluent configurations
│   ├── Migrations/                → EF Core migrations
│   ├── Seed/                      → Identity + Domain seeders
│   └── Services/                  → LocalFileStorageService
│
└── 🧪 RealEstate.Tests             → xUnit test suite
    ├── Architecture/              → Solution structure tests
    ├── Application/               → Service workflow tests
    └── Helpers/                   → TestDataBuilder
```

<br/>

**Dependency flow** — outer layers depend on inner layers, never the reverse:

```
Web  →  Application  →  Domain
         ↑
   Infrastructure
```

Infrastructure implements the interfaces defined in Application. The Domain knows nothing about databases, HTTP, or UI. This is Clean Architecture working exactly as intended.

<br/>

---

## ✦ Feature Showcase

### 🏘️ Property Management
- Create, edit, publish, archive, and soft-delete property listings
- Property lifecycle: `Draft → Published → Rented / Archived / Suspended`
- Multi-image upload with cover image selection
- Geolocation support (latitude/longitude)
- Amenities and features tagging
- Full-text search with filters: city, district, price range, bedrooms, property type, listing type
- Paginated public listing with `PagedResult<T>`

### 👤 Role-Based Access Control
- Three roles: **Admin**, **Agent**, **Tenant** — enforced at every layer
- Custom `ResourceOwnershipHandler` — policy-based authorization in ASP.NET Core
- `OwnershipAccessService` — centralized cross-entity ownership verification
- `IAuthorizationService` used directly in controllers for resource-level checks
- Anti-forgery tokens on all POST actions

### 📬 Tenant Workflow
- Save properties to **Favorites**
- Send **Inquiries** to agents (with subject and message)
- Request **Visit Appointments** with preferred schedule
- Submit **Rental Applications** with cover letter and move-in date
- Write **Reviews** on properties they've visited
- Manage all activity from a personal dashboard

### 🧑‍💼 Agent Workflow
- Full property management dashboard
- View and respond to inquiries
- Approve or decline visit appointments
- Review and process rental applications
- Real-time notification panel for all activity

### 🔑 Admin Panel
- Dedicated `/Admin` area with its own controllers and views
- User management: suspend, activate, delete accounts
- Property moderation: suspend and restore listings
- Review moderation: approve or reject tenant reviews
- Global dashboard with platform statistics

### 🔔 Notification System
- Notifications created on key events (new inquiry, visit approved, application accepted...)
- Real-time notification count in navbar
- `NotificationPanelViewComponent` — Razor ViewComponent pattern
- Mark as read, bulk read

### 📄 Document & Contract Management
- `LeaseContract` entity with full status lifecycle
- `Document` entity with typed document support (ID, contract, etc.)
- File upload via `IFileStorageService` (local implementation, cloud-ready interface)

### 🛡️ Security
- ASP.NET Core Identity with custom `ApplicationUser`
- Password policy: min 8 chars, digit, upper, lowercase required
- Unique email enforcement
- Soft delete with global EF Core query filters — deleted records are invisible to all queries
- `AuditLog` entity for tracking sensitive operations
- `AuditableEntity` base class — `CreatedAt`, `UpdatedAt`, `CreatedByUserId`, `UpdatedByUserId` on every entity

<br/>

---

## ✦ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core 9 MVC |
| **ORM** | Entity Framework Core 9 |
| **Database** | SQL Server (LocalDB for dev) |
| **Authentication** | ASP.NET Core Identity |
| **Authorization** | Policy-based + Resource-based |
| **Frontend** | Razor Views, Bootstrap 5, Vanilla JS |
| **UI Theme** | Dark luxury editorial (Playfair Display + Inter) |
| **File Storage** | Local filesystem (IFileStorageService — swap to Azure Blob or S3) |
| **Testing** | xUnit + EF Core InMemory |
| **Migrations** | EF Core Code First |
| **Target Framework** | .NET 9.0 |

<br/>

---

## ✦ Domain Model

18 entities covering the complete real estate lifecycle:

```
ApplicationUser (Identity)
│
├── AgentProfile          → owns Properties
│   └── Property
│       ├── PropertyImage
│       ├── PropertyAmenity
│       ├── PropertyFeature
│       ├── Inquiry           ← sent by Tenant
│       ├── VisitAppointment  ← requested by Tenant
│       ├── RentalApplication ← submitted by Tenant
│       ├── Review            ← written by Tenant
│       ├── Document
│       └── LeaseContract
│
├── TenantProfile         → interacts with Properties
│   ├── Favorite
│   ├── Inquiry
│   ├── VisitAppointment
│   ├── RentalApplication
│   └── Review
│
├── City
│   └── District
│
├── Notification
└── AuditLog
```

<br/>

---

## ✦ Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server LocalDB](https://docs.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (ships with Visual Studio)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# Dev Kit

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/LuxeHabitat.git
cd LuxeHabitat
```

### 2. Configure the connection string

Open `src/RealEstate.Web/appsettings.json` and verify:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RealEstateDb_App;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

> **Using a different SQL Server?** Just replace the connection string. The rest is handled automatically.

### 3. Run the application

```bash
cd src/RealEstate.Web
dotnet run
```

On first launch, the app will:
- **Apply all EF Core migrations** automatically
- **Seed the database** with cities (Paris, Lyon, Marseille), districts, and a default admin account

### 4. Default credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@realestate.local` | `Admin1234!` |

Register as **Agent** or **Tenant** directly from the `/Account/Register` page.

### 5. Run the tests

```bash
cd tests/RealEstate.Tests
dotnet test
```

<br/>

---

## ✦ Project Structure (source files only)

```
src/
├── RealEstate.Web/
│   ├── Areas/Admin/Controllers/AdminController.cs
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── AgentController.cs
│   │   ├── DashboardController.cs
│   │   ├── HomeController.cs
│   │   ├── NotificationController.cs
│   │   ├── PropertyController.cs
│   │   └── TenantController.cs
│   ├── Policies/
│   │   ├── AuthorizationPolicies.cs
│   │   ├── ResourceOwnershipHandler.cs
│   │   └── ResourceOwnershipRequirement.cs
│   ├── ViewComponents/NotificationPanelViewComponent.cs
│   ├── Extensions/
│   └── Mappings/ViewModelMappingExtensions.cs
│
├── RealEstate.Application/
│   ├── Services/               (12 services)
│   ├── Interfaces/Services/    (12 interfaces)
│   ├── DTOs/                   (per-feature DTOs)
│   └── Exceptions/             (AppException hierarchy)
│
├── RealEstate.Domain/
│   ├── Entities/               (18 entities)
│   ├── Enums/                  (11 enums)
│   └── Common/                 (BaseEntity, AuditableEntity)
│
└── RealEstate.Infrastructure/
    ├── Persistence/
    │   ├── ApplicationDbContext.cs
    │   ├── Configurations/     (18 Fluent API configs)
    │   ├── Migrations/
    │   └── Seed/
    └── Services/LocalFileStorageService.cs
```

<br/>

---

## ✦ Key Design Decisions

**Why no Repository pattern?**  
Services inject `IApplicationDbContext` directly. This keeps the codebase simple without the ceremony of repositories over EF Core, which already implements the Unit of Work pattern. The interface boundary is sufficient for testability with EF InMemory.

**Why resource-based authorization?**  
Standard role-based authorization (`[Authorize(Roles = "Agent")]`) can't answer "does this agent own this property?". The `ResourceOwnershipHandler` + `OwnershipAccessService` solve this at the authorization layer, not scattered across service methods.

**Why soft delete with global query filters?**  
Deleted entities are set `IsDeleted = true` and filtered at the EF Core level via `HasQueryFilter`. This means no deleted record ever appears in any query, without any developer having to remember to add `.Where(x => !x.IsDeleted)` everywhere. One config, zero leaks.

**Why a custom exception hierarchy?**  
`NotFoundAppException`, `ForbiddenAppException`, `ConflictAppException`, `ValidationAppException` map cleanly to HTTP semantics. Controllers catch `AppException` and convert to the appropriate `IActionResult`. No scattered `try/catch` logic with magic strings.

<br/>

---

## ✦ Roadmap

- [ ] Azure Blob Storage implementation for `IFileStorageService`
- [ ] Email notifications via SendGrid / SMTP
- [ ] Real-time notifications via SignalR
- [ ] Google Maps / Mapbox integration for property geolocation
- [ ] PDF generation for lease contracts
- [ ] Advanced analytics dashboard for admins
- [ ] REST API layer (for mobile clients)
- [ ] Docker support with docker-compose

<br/>

---

## ✦ Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you'd like to change.

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'feat: add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

Please follow the existing code style — Clean Architecture layers, service interfaces, and DTO patterns.

<br/>

---

## ✦ License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

<br/>

---

<div align="center">

**Built with precision. Designed with intent.**

*LuxeHabitat — where Clean Architecture meets luxury real estate.*

<br/>

[![GitHub stars](https://img.shields.io/github/stars/YOUR_USERNAME/LuxeHabitat?style=social)](https://github.com/YOUR_USERNAME/LuxeHabitat)
[![GitHub forks](https://img.shields.io/github/forks/YOUR_USERNAME/LuxeHabitat?style=social)](https://github.com/YOUR_USERNAME/LuxeHabitat/fork)

</div>
