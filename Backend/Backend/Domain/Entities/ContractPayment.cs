using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Domain.Entities;

public class ContractPayment
{
    [Key]
    public int Id { get; set; }

    public int ContractId { get; set; }

    [ForeignKey(nameof(ContractId))]
    public Contract Contract { get; set; } = null!;
    
    public int ClientId { get; set; }

    [Precision(18, 2)]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public bool IsRefunded { get; set; } = false;
}