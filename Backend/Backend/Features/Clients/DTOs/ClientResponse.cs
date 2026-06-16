using Backend.Domain.Enums;

namespace Backend.Features.Clients.DTOs;

public record ClientResponse
{
    public int Id { get; set; }

    public ClientType ClientType { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? CompanyName { get; set; }

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Pesel { get; set; }

    public string? Krs { get; set; }

    public bool IsDeleted { get; set; }
}