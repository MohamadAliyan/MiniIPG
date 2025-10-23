namespace GatewayService.Application.Gateways.Dtos;

public class VerifyPaymentRequest
{
    public string Token { get; set; } 
    public string AppCode { get; set; }
}

public class VerifyPaymentResponse
{
   public decimal Amount { get; set; }
    public string ReservationNumber { get; set; }
    public int Status { get; set; }


}
public class UpdateStatusPaymentRequest
{
    public string Token { get; set; } 
    public string Rrn { get; set; } 
    public bool IsSuccess { get; set; }
}

