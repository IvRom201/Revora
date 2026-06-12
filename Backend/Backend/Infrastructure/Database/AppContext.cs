using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options){}

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<SoftwareProduct> SoftwareProducts { get; set; } = null!;
    public DbSet<Discount> Discounts { get; set; } = null!;
    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<ContractPayment> ContractPayments { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
}