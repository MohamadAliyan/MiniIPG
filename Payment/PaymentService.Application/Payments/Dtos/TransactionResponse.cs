namespace PaymentService.Application.Payments.Dtos;

public class TransactionResponse
{
    public string GatewayUrl { get; set; }
    public string Token { get; set; }

}