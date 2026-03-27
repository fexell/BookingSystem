# BookingSystem 🎵

Ett bokningssystem för musikstudiouthyrning byggt med ASP.NET Core Web API och Blazor WebAssembly.

---

## Projektbeskrivning

BookingSystem låter användare boka studiolokaler och utrustning per timme. Systemet har rollbaserad åtkomstkontroll där vanliga användare kan se och boka resurser, medan administratörer kan hantera alla bokningar och resurser.

---

## Teknisk stack

| Del | Teknologi |
|-----|-----------|
| Backend | ASP.NET Core 10 Web API |
| Frontend | Blazor WebAssembly |
| Databas | SQLite med Entity Framework Core |
| Autentisering | JWT med cookie-baserad lagring |
| Identitetshantering | ASP.NET Core Identity |
| API-dokumentation | Swagger / OpenAPI |

---

## Projektstruktur

```
BookingSystem/
├── BookingSystem.Api/          ← Backend REST API
│   ├── Controllers/            ← HTTP-endpoints
│   ├── Services/               ← Affärslogik
│   ├── Repositories/           ← Databasåtkomst
│   ├── Models/                 ← Domänmodeller
│   ├── Filters/                ← Actionfilters
│   ├── Helpers/                ← Hjälpklasser
│   ├── Migrations/             ← EF Core migrationer
│   └── Program.cs
├── BookingSystem.Client/       ← Blazor WebAssembly frontend
│   ├── Pages/                  ← Razor-sidor
│   ├── Services/               ← HTTP-klienter
│   ├── Handlers/               ← Cookie-hantering
│   └── Program.cs
├── BookingSystem.Shared/       ← Delade DTOs
│   └── DTOs/
└── BookingSystem.Tests/        ← Enhetstester och integrationstester
    ├── UnitTests/
    └── IntegrationTests/
```

---

## Arkitektur

Projektet följer en lagerarkitektur (Layered Architecture):

```
Controller → Service → Repository → Databas
```

- **Controller** — tar emot HTTP-anrop och delegerar till Service
- **Service** — affärslogik och valideringsregler
- **Repository** — all databaskommunikation via EF Core
- **DTOs** — delade dataöverföringsobjekt mellan frontend och backend

---

## Sidor (Blazor)

| Sida | URL | Åtkomst |
|------|-----|---------|
| Hem | `/` | Alla |
| Logga in | `/login` | Ej inloggad |
| Registrera | `/register` | Ej inloggad |
| Studios | `/studios` | Alla |
| Studio-detalj | `/studios/{id}` | Alla |
| Boka | `/book` | Inloggad |
| Mina bokningar | `/mybookings` | Inloggad |
| Min profil | `/profile` | Inloggad |
| Admin | `/admin` | Admin |
| Admin - Bokningar | `/admin/bookings` | Admin |
| Admin - Studios | `/admin/studios` | Admin |
| Admin - Användare | `/admin/users` | Admin |

---

## API Endpoints

### Auth
| Metod | URL | Beskrivning | Skyddad |
|-------|-----|-------------|---------|
| POST | `/api/auth/register` | Registrera användare | Nej |
| POST | `/api/auth/login` | Logga in | Nej |
| POST | `/api/auth/logout` | Logga ut | Ja |
| POST | `/api/auth/refresh` | Förnya token | Nej |

### Bokningar
| Metod | URL | Beskrivning | Skyddad |
|-------|-----|-------------|---------|
| GET | `/api/bookings` | Hämta alla bokningar | Ja (Admin) |
| GET | `/api/bookings/{id}` | Hämta bokning | Ja |
| GET | `/api/bookings/user/me` | Mina bokningar | Ja |
| GET | `/api/bookings/user/{userId}` | Användarens bokningar | Ja |
| POST | `/api/bookings` | Skapa bokning | Ja |
| PUT | `/api/bookings/{id}` | Uppdatera bokning | Ja (Admin) |
| DELETE | `/api/bookings/{id}` | Ta bort bokning | Ja |

### Resurser
| Metod | URL | Beskrivning | Skyddad |
|-------|-----|-------------|---------|
| GET | `/api/resources` | Hämta alla resurser | Ja |
| GET | `/api/resources/{id}` | Hämta resurs | Ja |
| GET | `/api/resources/available` | Tillgängliga resurser | Nej |
| POST | `/api/resources` | Skapa resurs | Ja (Admin) |
| PUT | `/api/resources/{id}` | Uppdatera resurs | Ja (Admin) |
| DELETE | `/api/resources/{id}` | Ta bort resurs | Ja (Admin) |

### Användare
| Metod | URL | Beskrivning | Skyddad |
|-------|-----|-------------|---------|
| GET | `/api/users` | Hämta alla användare | Ja (Admin) |
| GET | `/api/users/me` | Min profil | Ja |
| GET | `/api/users/{id}` | Hämta användare | Ja |
| PUT | `/api/users/{id}` | Uppdatera användare | Ja |
| DELETE | `/api/users/{id}` | Ta bort användare | Ja (Admin) |

---

## Körinstruktioner

### Förutsättningar

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 eller VS Code

### Klona projektet

```bash
git clone https://github.com/fexell/BookingSystem.git
cd BookingSystem
```

### Installera beroenden

```bash
dotnet restore
```

### Skapa databasen

```bash
cd BookingSystem.Api
dotnet ef database update
```

### Starta projektet

**Via Visual Studio:**
1. Högerklicka på Solution → Properties
2. Välj "Multiple startup projects"
3. Sätt `BookingSystem.Api` och `BookingSystem.Client` till "Start"
4. Tryck `F5`

**Via terminal:**
```bash
# Terminal 1
cd BookingSystem.Api
dotnet run

# Terminal 2
cd BookingSystem.Client
dotnet run
```

### Öppna i webbläsaren

- **Blazor-appen:** `https://localhost:7193`
- **Swagger API:** `https://localhost:7024/swagger`

---

## Testanvändare

| Roll | Email | Lösenord |
|------|-------|----------|
| Admin | admin@studio.se | Admin123! |
| Användare | anna@user.com | User123! |
| Användare | erik@example.com | User123! |

---

## Tillgängliga resurser (seed-data)

| Namn | Typ | Beskrivning |
|------|-----|-------------|
| Studio A | Studio | Big recording studio with drums, guitar amps, and a vocal booth |
| Studio B | Studio | Smaller studio, perfect for vocals and acoustic instruments |
| Guitar Amp | Utrustning | Fender Twin Reverb, great for clean tones and classic rock |
| Vocal Booth | Utrustning | Soundproof booth with a condenser microphone and pop filter |
| Control Room | Studio | Room with mixing console, monitors, and recording software |

---

## Tester

```bash
cd BookingSystem.Tests
dotnet test
```

Projektet innehåller:
- Enhetstester för Services (BookingService, AuthService, ResourceService, UserService)
- Enhetstester för Controllers
- Integrationstester för BookingsController

---

## Designprinciper

- **SOLID** — Single Responsibility, Dependency Inversion via interfaces
- **Clean Code** — tydliga namn, små metoder, separata ansvar
- **DRY** — Generic Repository `IRepository<T>` undviker kodupprepning
- **Lagerarkitektur** — tydlig separation mellan Controller, Service och Repository
