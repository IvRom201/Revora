using Backend.Features.Software.DTOs;
using Backend.Infrastructure.Database;
using Backend.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Software;

public class SoftwareService : ISoftwareService
{
    private readonly AppDbContext _dbContext;

    public SoftwareService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SoftwareResponse>> GetAllAsync()
    {
        return await _dbContext.SoftwareProducts
            .AsNoTracking()
            .Select(s => new SoftwareResponse
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                CurrentVersion = s.CurrentVersion,
                Category = s.Category,
                YearlyLicensePrice = s.YearlyLicensePrice
            })
            .ToListAsync();
    }

    public async Task<SoftwareResponse> GetByIdAsync(int id)
    {
        var software = await _dbContext.SoftwareProducts
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (software is null)
        {
            throw new NotFoundException($"Software product with id {id} was not found.");
        }

        return new SoftwareResponse
        {
            Id = software.Id,
            Name = software.Name,
            Description = software.Description,
            CurrentVersion = software.CurrentVersion,
            Category = software.Category,
            YearlyLicensePrice = software.YearlyLicensePrice
        };
    }
}