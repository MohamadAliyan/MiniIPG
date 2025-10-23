using MediatR;
using PaymentService.Application.Payments.Dtos;
using Shared;

namespace PaymentService.Application.Payments.Commands;

public record CreateTransactionCommand(
    string TerminalNo,
    decimal Amount,
    string RedirectUrl,
    string ReservationNumber,
    string PhoneNumber
) : IRequest<Result< TransactionResponse>>;