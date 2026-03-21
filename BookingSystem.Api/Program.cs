using BookingSystem.Api.Repositories;
using BookingSystem.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.Design;
using System.Text;


var builder = WebApplication.CreateBuilder( args );

builder.Services.AddDbContext<AppDbContext>( options =>
options.UseSqlite( "Data Source=bookingsystem.db" ) );

// Repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();