
using Shared;

namespace PaymentService.Application.Payments.Dtos;

public class VerifyTransactioResponse
{
    public decimal Amount { get; set; }
    public string ReservationNumber { get; set; }
    public PaymentStatus Status { get; set; }
}