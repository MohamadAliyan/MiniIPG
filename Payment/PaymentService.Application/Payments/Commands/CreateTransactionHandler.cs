using MediatR;
using Microsoft.Extensions.Options;
using PaymentService.Application.Common.Models;
using PaymentService.Application.Payments.Dtos;
using PaymentService.Domain.Entities;

using Shared;

namespace PaymentService.Application.Payments.Commands;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand,Result< TransactionResponse>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IOptions<AppSettings> _configuration;

    public CreateTransactionHandler(
        ITransactionRepository transactionRepository,
        IOptions<AppSettings> configuration)
    {
        _transactionRepository = transactionRepository;
        _configuration = configuration;
    }

    public async Task<Result< TransactionResponse>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = new Transaction
            {
                Id =Guid.NewGuid(),
                TerminalNo = request.TerminalNo,
                Amount = request.Amount,
                RedirectUrl = request.RedirectUrl,
                ReservationNumber = request.ReservationNumber,
                PhoneNumber = request.PhoneNumber,
                Token = Guid.NewGuid().ToString(),
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                
            };

            _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            var res= new TransactionResponse
            {
                Token = transaction.Token,
                GatewayUrl = $"{_configuration.Value.GatewayUrl}/api/gateway/pay/{ transaction.Token }",
                   
            };

            return Result<TransactionResponse>.Success(res, "توکن با موفقیت ایجاد شد.");
        }
        catch (Exception ex)
        {
            return Result<TransactionResponse>.Failure($"خطا در ایجاد تراکنش: {ex.Message}");
        }


      
    }
}