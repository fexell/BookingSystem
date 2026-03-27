BookingSystem 🎵
Ett bokningssystem för musikstudiouthyrning byggt med ASP.NET Core Web API och Blazor WebAssembly.

Projektbeskrivning
BookingSystem låter användare boka studiolokaler och utrustning per timme. Systemet har rollbaserad åtkomstkontroll där vanliga användare kan se och boka resurser, medan administratörer kan hantera alla bokningar och resurser.

Teknisk stack
DelTeknologiBackendASP.NET Core 10 Web APIFrontendBlazor WebAssemblyDatabasSQLite med Entity Framework CoreAutentiseringJWT med cookie-baserad lagringIdentitetshanteringASP.NET Core IdentityAPI-dokumentationSwagger / OpenAPI

Projektstruktur
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

Arkitektur
Projektet följer en lagerarkitektur (Layered Architecture):
Controller → Service → Repository → Databas

Controller — tar emot HTTP-anrop och delegerar till Service
Service — affärslogik och valideringsregler
Repository — all databaskommunikation via EF Core
DTOs — delade dataöverföringsobjekt mellan frontend och backend


Sidor (Blazor)
SidaURLÅtkomstHem/AllaLogga in/loginEj inloggadRegistrera/registerEj inloggadStudios/studiosAllaStudio-detalj/studios/{id}AllaBoka/bookInloggadMina bokningar/mybookingsInloggadMin profil/profileInloggadAdmin/adminAdminAdmin - Bokningar/admin/bookingsAdminAdmin - Studios/admin/studiosAdminAdmin - Användare/admin/usersAdmin

API Endpoints
Auth
MetodURLBeskrivningSkyddadPOST/api/auth/registerRegistrera användareNejPOST/api/auth/loginLogga inNejPOST/api/auth/logoutLogga utJaPOST/api/auth/refreshFörnya tokenNej
Bokningar
MetodURLBeskrivningSkyddadGET/api/bookingsHämta alla bokningarJa (Admin)GET/api/bookings/{id}Hämta bokningJaGET/api/bookings/user/meMina bokningarJaGET/api/bookings/user/{userId}Användarens bokningarJaPOST/api/bookingsSkapa bokningJaPUT/api/bookings/{id}Uppdatera bokningJa (Admin)DELETE/api/bookings/{id}Ta bort bokningJa
Resurser
MetodURLBeskrivningSkyddadGET/api/resourcesHämta alla resurserJaGET/api/resources/{id}Hämta resursJaGET/api/resources/availableTillgängliga resurserNejPOST/api/resourcesSkapa resursJa (Admin)PUT/api/resources/{id}Uppdatera resursJa (Admin)DELETE/api/resources/{id}Ta bort resursJa (Admin)
Användare
MetodURLBeskrivningSkyddadGET/api/usersHämta alla användareJa (Admin)GET/api/users/meMin profilJaGET/api/users/{id}Hämta användareJaPUT/api/users/{id}Uppdatera användareJaDELETE/api/users/{id}Ta bort användareJa (Admin)

Körinstruktioner
Förutsättningar

.NET 10 SDK
Visual Studio 2022 eller VS Code

Klona projektet
bashgit clone https://github.com/fexell/BookingSystem.git
cd BookingSystem
Installera beroenden
bashdotnet restore
Skapa databasen
bashcd BookingSystem.Api
dotnet ef database update
Starta projektet
Via Visual Studio:

Högerklicka på Solution → Properties
Välj "Multiple startup projects"
Sätt BookingSystem.Api och BookingSystem.Client till "Start"
Tryck F5

Via terminal:
bash# Terminal 1
cd BookingSystem.Api
dotnet run

# Terminal 2
cd BookingSystem.Client
dotnet run
Öppna i webbläsaren

Blazor-appen: https://localhost:7193
Swagger API: https://localhost:7024/swagger


Testanvändare
RollEmailLösenordAdminadmin@studio.seAdmin123!Användareanna@user.comUser123!Användareerik@example.comUser123!

Tillgängliga resurser (seed-data)
NamnTypBeskrivningStudio AStudioBig recording studio with drums, guitar amps, and a vocal boothStudio BStudioSmaller studio, perfect for vocals and acoustic instrumentsGuitar AmpUtrustningFender Twin Reverb, great for clean tones and classic rockVocal BoothUtrustningSoundproof booth with a condenser microphone and pop filterControl RoomStudioRoom with mixing console, monitors, and recording software

Tester
bashcd BookingSystem.Tests
dotnet test
Projektet innehåller:

Enhetstester för Services (BookingService, AuthService, ResourceService, UserService)
Enhetstester för Controllers
Integrationstester för BookingsController


Designprinciper

SOLID — Single Responsibility, Dependency Inversion via interfaces
Clean Code — tydliga namn, små metoder, separata ansvar
DRY — Generic Repository IRepository<T> undviker kodupprepning
Lagerarkitektur — tydlig separation mellan Controller, Service och Repository
