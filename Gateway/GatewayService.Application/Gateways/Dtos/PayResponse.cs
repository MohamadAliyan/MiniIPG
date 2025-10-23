namespace GatewayService.Application.Gateways.Dtos;

public class PayResponseResult
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; }
    public string Rrn { get; set; }
    public string RedirectUrl { get; set; }
    public decimal Amount { get; set; }
}

public class ErrorPayResponseResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public PayResponseResult? Data { get; set; }
}