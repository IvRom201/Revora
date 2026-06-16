using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Features.Payments.DTOs;
using Backend.Infrastructure.Database;
using Backend.Infrastructure.Time;
using Backend.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Payments;

public class ContractPaymentService : IContractPaymentService
{
    private readonly AppDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ContractPaymentService(
        AppDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ContractPaymentResponse> PayForContractAsync(
        int contractId,
        CreateContractPaymentRequest request)
    {
        var contract = await _dbContext.Contracts
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == contractId);

        if (contract is null)
        {
            throw new NotFoundException($"Contract with id {contractId} was not found.");
        }

        if (request.ClientId != contract.ClientId)
        {
            throw new BadRequestException("Payment client does not match contract client.");
        }

        if (request.Amount <= 0)
        {
            throw new BadRequestException("Payment amount must be greater than zero.");
        }

        if (contract.Status == ContractStatus.Signed)
        {
            throw new BadRequestException("Contract is already fully paid and signed.");
        }

        if (contract.Status is ContractStatus.Cancelled or ContractStatus.Expired)
        {
            throw new BadRequestException("Cannot pay for cancelled or expired contract.");
        }

        if (_dateTimeProvider.Today > contract.EndDate)
        {
            contract.Status = ContractStatus.Expired;

            foreach (var oldPayment in contract.Payments)
            {
                oldPayment.IsRefunded = true;
            }

            await _dbContext.SaveChangesAsync();

            throw new BadRequestException("Payment deadline has passed. Contract expired and previous payments were refunded.");
        }

        var alreadyPaid = contract.Payments
            .Where(p => !p.IsRefunded)
            .Sum(p => p.Amount);

        var newTotal = alreadyPaid + request.Amount;

        if (newTotal > contract.FinalPrice)
        {
            throw new BadRequestException("Payment would exceed contract final price.");
        }

        var payment = new ContractPayment
        {
            ContractId = contract.Id,
            ClientId = request.ClientId,
            Amount = request.Amount,
            PaymentDate = _dateTimeProvider.UtcNow,
            IsRefunded = false
        };

        _dbContext.ContractPayments.Add(payment);

        var contractSigned = false;

        if (newTotal == contract.FinalPrice)
        {
            contract.Status = ContractStatus.Signed;
            contract.SignedAt = _dateTimeProvider.UtcNow;
            contractSigned = true;
        }

        await _dbContext.SaveChangesAsync();

        return new ContractPaymentResponse
        {
            Id = payment.Id,
            ContractId = contract.Id,
            ClientId = payment.ClientId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            IsRefunded = payment.IsRefunded,
            ContractSigned = contractSigned,
            TotalPaid = newTotal,
            RequiredAmount = contract.FinalPrice
        };
    }
}