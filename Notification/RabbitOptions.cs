namespace Notification;

public class RabbitOptions
{
    public string HostName { get; set; } = "localhost";
    public string ExchangeName { get; set; } = "payment.events";
    public string ExchangeErrorName { get; set; } = "paymentError.events";
    public string QueueName { get; set; } = "notification_queue";
    public string QueueErrorName { get; set; } = "notification_queueError";
}

