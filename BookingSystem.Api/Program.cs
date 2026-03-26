using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Text;

using BookingSystem.Api.Filters;
using BookingSystem.Api.Helpers;
using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using BookingSystem.Api.Services;

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

    // If access token missing or expired, try refresh
    options.Events = new JwtBearerEvents {
        OnMessageReceived = async context =>
        {
            var path = context.HttpContext.Request.Path.Value;

            // 🚫 Do NOT refresh tokens on logout
            if ( path != null && path.Contains( "/logout", StringComparison.OrdinalIgnoreCase ) ) {
                context.Token = context.Request.Cookies[ "jwt" ];
                return;
            }

            var jwt = context.Request.Cookies[ "jwt" ];
            var refresh = context.Request.Cookies[ "refreshToken" ];

            if ( !string.IsNullOrEmpty( refresh ) &&
                ( string.IsNullOrEmpty( jwt ) || TokenHelper.IsTokenExpired( jwt ) ) ) {
                var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                var user = await authService.RefreshAsync( refresh );

                if ( user != null ) {
                    var newAccess = await authService.GenerateTokenAsync( user );
                    var newRefresh = await authService.GenerateRefreshTokenAsync( user );

                    context.Response.Cookies.Append( "jwt", newAccess, CookieHelper.GetCookieOptions() );
                    context.Response.Cookies.Append( "refreshToken", newRefresh, CookieHelper.GetCookieOptions() );

                    context.Token = newAccess;
                    return;
                }
            }

            context.Token = jwt;
        },

        OnChallenge = context => {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            // Return a 401 with a message, if the user is trying to access a protected route
            return context.Response.WriteAsync(
                """{ "message": "You must be logged in to access this resource" }"""
            );
        }
    };
} );

// CORS
builder.Services.AddCors( options => {
    options.AddPolicy( "BlazorClient", policy => {
        policy.WithOrigins( "https://localhost:7193", "http://localhost:5173" )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    } );
} );

builder.Services.Configure<ApiBehaviorOptions>( options => {
    options.SuppressModelStateInvalidFilter = true;
} );

// Repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Filters
builder.Services.AddScoped<SameUserFilter>();
builder.Services.AddScoped<NotLoggedInFilter>();

// Controllers & Swagger
builder.Services.AddControllers()
    .AddNewtonsoftJson();
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
app.UseCors( "BlazorClient" );
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }