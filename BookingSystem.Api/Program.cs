using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using BookingSystem.Api.Repositories;
using BookingSystem.Api.Services;
using BookingSystem.Api.Filters;
using BookingSystem.Api.Middlewares;


var builder = WebApplication.CreateBuilder( args );

builder.Services.AddDbContext<AppDbContext>( options =>
options.UseSqlite( "Data Source=bookingsystem.db" ) );

// Repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

//Services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Filters
builder.Services.AddScoped<SameUserFilter>();
builder.Services.AddScoped<NotLoggedInFilter>();

// CORS
builder.Services.AddCors( options => {
    options.AddPolicy( "BlazorClient", policy => {
        policy.WithOrigins( "https://localhost:7024", "http://localhost:5001" )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    } );
} );

builder.Services.AddAuthentication( JwtBearerDefaults.AuthenticationScheme )
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

//Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() ) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RefreshTokenMiddleware>();
app.UseCors( "BlazorClient" );
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();