using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Database;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<Employee>>();

        await dbContext.Database.MigrateAsync();

        if (!await dbContext.Employees.AnyAsync())
        {
            var admin = new Employee
            {
                Login = "admin",
                Role = EmployeeRole.Admin,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");

            var user = new Employee
            {
                Login = "user",
                Role = EmployeeRole.User,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHash = passwordHasher.HashPassword(user, "User123!");

            dbContext.Employees.AddRange(admin, user);
        }

        if (!await dbContext.SoftwareProducts.AnyAsync())
        {
            var finance = new SoftwareProduct
            {
                Name = "Revora Finance",
                Description = "Financial management software for enterprise clients.",
                CurrentVersion = "1.0.0",
                Category = "Finance",
                YearlyLicensePrice = 12000m
            };

            var education = new SoftwareProduct
            {
                Name = "Revora Edu",
                Description = "Education management platform.",
                CurrentVersion = "2.1.0",
                Category = "Education",
                YearlyLicensePrice = 8000m
            };

            dbContext.SoftwareProducts.AddRange(finance, education);
            await dbContext.SaveChangesAsync();

            dbContext.Discounts.AddRange(
                new Discount
                {
                    Name = "Black Friday Contract Discount",
                    DiscountType = DiscountType.Contract,
                    Percentage = 10m,
                    StartDate = new DateOnly(2026, 1, 1),
                    EndDate = new DateOnly(2026, 12, 31),
                    SoftwareProductId = finance.Id
                },
                new Discount
                {
                    Name = "Education Promo",
                    DiscountType = DiscountType.Contract,
                    Percentage = 15m,
                    StartDate = new DateOnly(2026, 1, 1),
                    EndDate = new DateOnly(2026, 12, 31),
                    SoftwareProductId = education.Id
                }
            );
        }

        await dbContext.SaveChangesAsync();
    }
}