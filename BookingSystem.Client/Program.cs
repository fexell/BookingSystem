using Microsoft.AspNetCore.Components.Authorization;
using BookingSystem.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookingSystem.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient( "API", client => {
    client.BaseAddress = new Uri( "https://localhost:7024" );
} )
#pragma warning disable CA1416
.ConfigurePrimaryHttpMessageHandler( () => new HttpClientHandler {
    UseCookies = true
} );
#pragma warning restore CA1416

await builder.Build().RunAsync();