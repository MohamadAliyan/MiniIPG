namespace Shared.Messaging;

public class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "payment.events";
    public string ExchangeErrorName { get; set; } = "paymentError.events";
    public int Port { get; set; } = 5672;
}