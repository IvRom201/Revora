using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Features.Clients.DTOs;
using Backend.Infrastructure.Database;
using Backend.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Clients;

public class ClientService : IClientService
{
    private readonly AppDbContext _dbContext;

    public ClientService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClientResponse> CreateIndividualAsync(CreateIndividualClientRequest request)
    {
        var peselExists = await _dbContext.Clients
            .AnyAsync(c => c.Pesel == request.Pesel);

        if (peselExists)
        {
            throw new ConflictException("Client with this PESEL already exists.");
        }

        var client = new Client
        {
            ClientType = ClientType.Individual,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Pesel = request.Pesel
        };

        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(client);
    }

    public async Task<ClientResponse> CreateCompanyAsync(CreateCompanyClientRequest request)
    {
        var krsExists = await _dbContext.Clients
            .AnyAsync(c => c.Krs == request.Krs);

        if (krsExists)
        {
            throw new ConflictException("Client with this KRS already exists.");
        }

        var client = new Client
        {
            ClientType = ClientType.Company,
            CompanyName = request.CompanyName,
            Address = request.Address,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Krs = request.Krs
        };

        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(client);
    }

    public async Task<List<ClientResponse>> GetAllAsync()
    {
        return await _dbContext.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => MapToResponse(c))
            .ToListAsync();
    }

    public async Task<ClientResponse> GetByIdAsync(int id)
    {
        var client = await _dbContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (client is null)
        {
            throw new NotFoundException($"Client with id {id} was not found.");
        }

        return MapToResponse(client);
    }

    public async Task<ClientResponse> UpdateAsync(int id, UpdateClientRequest request)
    {
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (client is null)
        {
            throw new NotFoundException($"Client with id {id} was not found.");
        }

        if (client.ClientType == ClientType.Individual)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                throw new BadRequestException("First name and last name are required for individual client.");
            }

            client.FirstName = request.FirstName;
            client.LastName = request.LastName;
        }

        if (client.ClientType == ClientType.Company)
        {
            if (string.IsNullOrWhiteSpace(request.CompanyName))
            {
                throw new BadRequestException("Company name is required for company client.");
            }

            client.CompanyName = request.CompanyName;
        }

        client.Address = request.Address;
        client.Email = request.Email;
        client.PhoneNumber = request.PhoneNumber;

        await _dbContext.SaveChangesAsync();

        return MapToResponse(client);
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (client is null)
        {
            throw new NotFoundException($"Client with id {id} was not found.");
        }

        if (client.ClientType == ClientType.Company)
        {
            throw new ForbiddenException("Company clients cannot be deleted.");
        }

        client.FirstName = "DELETED";
        client.LastName = "DELETED";
        client.Address = "DELETED";
        client.Email = $"deleted_client_{client.Id}@deleted.local";
        client.PhoneNumber = "000000000";
        client.IsDeleted = true;

        await _dbContext.SaveChangesAsync();
    }

    private static ClientResponse MapToResponse(Client client)
    {
        return new ClientResponse
        {
            Id = client.Id,
            ClientType = client.ClientType,
            FirstName = client.FirstName,
            LastName = client.LastName,
            CompanyName = client.CompanyName,
            Address = client.Address,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            Pesel = client.Pesel,
            Krs = client.Krs,
            IsDeleted = client.IsDeleted
        };
    }
}