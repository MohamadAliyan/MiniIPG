using MediatR;
using Shared;

namespace PaymentService.Application.Payments.Commands;

public record UpdateStatusTransactionCommand(
    string Token,
    bool IsSuccess,
    string? Rrn
  
) : IRequest<Result<string>>;