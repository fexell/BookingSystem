using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookingSystem.Api.Repositories;
using BookingSystem.Api.Services;
using BookingSystem.Api.Filters;
using BookingSystem.Api.Middlewares;
using BookingSystem.Api.Models;

var builder = WebApplication.CreateBuilder( args );

// Database
builder.Services.AddDbContext<AppDbContext>( options =>
    options.UseSqlite( "Data Source=bookingsystem.db" ) );

// Identity — use AddIdentityCore to avoid overriding JWT auth scheme
builder.Services.AddIdentityCore<User>( options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes( 5 );
    options.User.RequireUniqueEmail = true;
} )
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Authentication — JWT stays in control
builder.Services.AddAuthentication( options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
} )
.AddJwtBearer( options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration[ "Jwt:Issuer" ],
        ValidAudience = builder.Configuration[ "Jwt:Audience" ],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes( builder.Configuration[ "Jwt:Key" ]! ) )
    };
    options.Events = new JwtBearerEvents {
        OnMessageReceived = context => {
            context.Token = context.Request.Cookies[ "jwt" ];
            return Task.CompletedTask;
        }
    };
} );

// CORS
builder.Services.AddCors( options => {
    options.AddPolicy( "BlazorClient", policy => {
        policy.WithOrigins( "https://localhost:7024", "http://localhost:5001" )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    } );
} );

// Repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Filters
builder.Services.AddScoped<SameUserFilter>();
builder.Services.AddScoped<NotLoggedInFilter>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed roles
using ( var scope = app.Services.CreateScope() ) {
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    foreach ( var role in new[] { "User", "Admin" } ) {
        if ( !await roleManager.RoleExistsAsync( role ) )
            await roleManager.CreateAsync( new IdentityRole<int>( role ) );
    }
}

if ( app.Environment.IsDevelopment() ) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors( "BlazorClient" );                        // before auth
app.UseMiddleware<RefreshTokenMiddleware>();         // before UseAuthentication
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();