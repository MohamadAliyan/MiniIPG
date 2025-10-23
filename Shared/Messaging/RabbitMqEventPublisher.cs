

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Shared.Messaging;



public class RabbitMqEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly RabbitMqSettings _options;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqEventPublisher(ILogger<RabbitMqEventPublisher> logger, IOptions<RabbitMqSettings> options)
    {
        _logger = logger;
        _options = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false
        ).Wait();
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: string.Empty,
                mandatory: false,
                body: body
            );

            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}