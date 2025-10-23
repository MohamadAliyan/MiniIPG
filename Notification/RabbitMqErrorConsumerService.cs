using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Shared;
using System.Text;
using System.Text.Json;

namespace Notification;

public class RabbitMqErrorConsumerService : BackgroundService
{
    private readonly ILogger<RabbitMqErrorConsumerService> _logger;
    private readonly RabbitOptions _options;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqErrorConsumerService(ILogger<RabbitMqErrorConsumerService> logger, IOptions<RabbitOptions> options)
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
        await _channel.ExchangeDeclareAsync(_options.ExchangeErrorName, ExchangeType.Fanout, durable: true);
        await _channel.QueueDeclareAsync(_options.QueueErrorName, durable: true, exclusive: false, autoDelete: false);
        await _channel.QueueBindAsync(_options.QueueErrorName, _options.ExchangeErrorName, string.Empty);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                Log.Warning(" Received error message: {@Error}", json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error writing error message to file");
            }
        
        };

        await _channel.BasicConsumeAsync(_options.QueueErrorName, autoAck: false, consumer);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}