namespace Shared;

public class PaymentProcessedEvent
{
    public string Token { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
    public string Rrn { get; set; }
    public string ReservationNumber { get; set; }
    public string AppCode { get; set; }
    public DateTime CreatedAt { get; set; }
}