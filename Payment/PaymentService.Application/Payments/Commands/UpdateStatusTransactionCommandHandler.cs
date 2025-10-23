using MediatR;
using Microsoft.Extensions.Options;
using PaymentService.Application.Common.Models;
using PaymentService.Application.Payments.Dtos;
using PaymentService.Domain;
using PaymentService.Domain.Entities;

using Shared;
using Shared.Messaging;


namespace PaymentService.Application.Payments.Commands;

public class UpdateStatusTransactionCommandHandler : IRequestHandler<UpdateStatusTransactionCommand, Result<string>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IOptions<AppSettings> _configuration;
    private readonly IEventPublisher _eventPublisher;

    public UpdateStatusTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IOptions<AppSettings> configuration,
        IEventPublisher eventPublisher)
    {
        _transactionRepository = transactionRepository;
        _configuration = configuration;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<string>> Handle(UpdateStatusTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = _transactionRepository.GetBy(p => p.Token == request.Token).SingleOrDefault();

            if (transaction is null)
                return Result<string>.Failure($"توکن نامعتبر است");



            transaction.Status = request.IsSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
            transaction.UpdatedAt = DateTime.UtcNow;
            if (request.IsSuccess)
            {
                transaction.RRN = request.Rrn;
            }

            _transactionRepository.Update(transaction);
            await _transactionRepository.SaveChangesAsync();


            var paymentEvent = new PaymentProcessedEvent
            {
                Token = transaction.Token,
                Status = transaction.Status.ToString(),
                Amount = transaction.Amount,
                Rrn = transaction.RRN,
                ReservationNumber = transaction.ReservationNumber,
                AppCode = transaction.AppCode,
                CreatedAt = transaction.CreatedAt
            };

            await _eventPublisher.PublishAsync(paymentEvent,cancellationToken);

            return Result<string>.Success("وضعیت با موفقیت به روزرسانی شد");

        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"خطا در پیداکردن  تراکنش: {ex.Message}");


        }
    }
}