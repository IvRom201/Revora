namespace Backend.Features.Software.DTOs;

public record SoftwareResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string CurrentVersion { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal YearlyLicensePrice { get; set; }
}