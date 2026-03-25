using System.Linq;
using BookingSystem.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookingSystem.Tests.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Ta bort den befintliga Sqlite-databasen och Option-typer
                var descriptors = services
                    .Where(d => d.ServiceType.Name.Contains("DbContextOptions"))
                    .ToList();

                foreach (var d in descriptors)
                {
                    services.Remove(d);
                }

                // Lägg till In-Memory databas för testning
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Bygg ServiceProvider för att seeda databasen direkt om vi vill
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    
                    // Skapa databasen för test
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}
