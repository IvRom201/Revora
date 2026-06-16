using System.ComponentModel.DataAnnotations;

namespace Backend.Features.Contracts.DTOs;

public record CreateContractRequest
{
    public int ClientId { get; set; }

    public int SoftwareProductId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Range(0, 3)]
    public int AdditionalSupportYears { get; set; }
}