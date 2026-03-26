using BookingSystem.Client;
using BookingSystem.Client.Handlers;
using BookingSystem.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>( sp =>
    sp.GetRequiredService<CustomAuthStateProvider>() );
builder.Services.AddScoped<CookieHandler>();
builder.Services.AddScoped<IResourceService, ResourceService>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient( "API", client => {
    client.BaseAddress = new Uri( "https://localhost:7024" );
} ).AddHttpMessageHandler<CookieHandler>();

await builder.Build().RunAsync();