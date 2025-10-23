using MediatR;
using PaymentService.Application.Payments.Dtos;
using Shared;

namespace PaymentService.Application.Payments.Commands;

public record VerifyTransactionCommand(
    string Token,
    string AppCode
 
) : IRequest<Result<VerifyTransactioResponse>>;