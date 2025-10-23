using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;

namespace Notification;

public class RabbitMqPaymentConsumerService : BackgroundService
{
    private readonly ILogger<RabbitMqPaymentConsumerService> _logger;
    private readonly RabbitOptions _options;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqPaymentConsumerService(ILogger<RabbitMqPaymentConsumerService> logger, IOptions<RabbitOptions> options)
    {
        _logger = logger;
        _options = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
        };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Fanout, durable: true);
        await _channel.QueueDeclareAsync(_options.QueueName, durable: true, exclusive: false, autoDelete: false);
        await _channel.QueueBindAsync(_options.QueueName, _options.ExchangeName, string.Empty);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(json);

                if (evt != null)
                {
                    _logger.LogInformation(
                        "Received PaymentProcessedEvent: Token={Token}, Status={Status}, Amount={Amount}, RRN={RRN}",
                        evt.Token, evt.Status, evt.Amount, evt.Rrn
                    );
                }
                await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing message");
                await _channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel.BasicConsumeAsync(_options.QueueName, autoAck: false, consumer);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}