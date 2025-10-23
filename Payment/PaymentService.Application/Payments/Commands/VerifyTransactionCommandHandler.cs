using MediatR;
using Microsoft.Extensions.Options;
using PaymentService.Application.Payments.Dtos;
using PaymentService.Domain;
using PaymentService.Domain.Entities;

using Shared;
using System.Transactions;
using PaymentService.Application.Common.Models;

namespace PaymentService.Application.Payments.Commands;

public class VerifyTransactionCommandHandler : IRequestHandler<VerifyTransactionCommand, Result<VerifyTransactioResponse>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IOptions<AppSettings> _configuration;

    public VerifyTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IOptions<AppSettings> configuration)
    {
        _transactionRepository = transactionRepository;
        _configuration = configuration;
    }

    public async Task<Result<VerifyTransactioResponse>> Handle(VerifyTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = _transactionRepository.GetBy(p => p.Token == request.Token).SingleOrDefault();

            var res = new VerifyTransactioResponse();
           

            if (transaction is null)
                return Result<VerifyTransactioResponse>.Failure($"توکن نامعتبر است");

            if (transaction.Status == PaymentStatus.Success)
                return Result<VerifyTransactioResponse>.Failure("این تراکنش قبلاً با موفقیت تایید شده است.");

            var timeSinceCreation = DateTime.UtcNow - transaction.CreatedAt;
            var expirationTime = TimeSpan.FromMinutes(2);

            if (timeSinceCreation > expirationTime)
            {
                transaction.Status = PaymentStatus.Expired;
                transaction.UpdatedAt = DateTime.UtcNow;
                transaction.AppCode = request.AppCode;

                _transactionRepository.Update(transaction);
                await _transactionRepository.SaveChangesAsync();
                res.Status = PaymentStatus.Expired;
                res.Amount=transaction.Amount;
                res.ReservationNumber=transaction.ReservationNumber;
                return Result<VerifyTransactioResponse>.Success(res,$"زمان پرداخت منقضی شده است.");
            }
            else
            {
                transaction.Status = PaymentStatus.Success;
                transaction.UpdatedAt = DateTime.UtcNow;
                transaction.AppCode = request.AppCode;
                _transactionRepository.Update(transaction);
                await _transactionRepository.SaveChangesAsync();
                res.Status = PaymentStatus.Success;
                res.Amount = transaction.Amount;
                res.ReservationNumber = transaction.ReservationNumber;

                return Result<VerifyTransactioResponse>.Success(res, "پرداخت با موفقیت تایید شد");
            }
        }
        catch (Exception ex)
        {
            return Result<VerifyTransactioResponse>.Failure($"خطا در پیداکردن  تراکنش: {ex.Message}");
        }



    }
}