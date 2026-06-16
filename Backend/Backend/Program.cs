using System.Text;
using Backend.Domain.Entities;
using Backend.Features.Clients;
using Backend.Features.Contracts;
using Backend.Features.Contracts.Pricing;
using Backend.Features.Payments;
using Backend.Features.Revenue;
using Backend.Features.Software;
using Backend.Infrastructure.Auth;
using Backend.Infrastructure.Currency;
using Backend.Infrastructure.Database;
using Backend.Infrastructure.Time;
using Backend.Shared.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ISoftwareService, SoftwareService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IContractPaymentService, ContractPaymentService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IContractPriceCalculator, ContractPriceCalculator>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<PasswordHasher<Employee>>();
builder.Services.AddHttpClient<ICurrencyService, NbpCurrencyService>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// OpenAPI for .NET 10
builder.Services.AddOpenApi();

var app = builder.Build();

if (args.Contains("--seed"))
{
    await DataSeeder.SeedAsync(app.Services);
    return;
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Revora API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();