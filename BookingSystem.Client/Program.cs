using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookingSystem.Client;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddHttpClient( "API", client => {
    client.BaseAddress = new Uri( "https://localhost:7024" );
} )
.ConfigurePrimaryHttpMessageHandler( () => new HttpClientHandler {
    UseCookies = true
} );

await builder.Build().RunAsync();