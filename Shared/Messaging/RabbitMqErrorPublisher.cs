using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Shared.Messaging;

public class RabbitMqErrorPublisher
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly RabbitMqSettings _options;


    public RabbitMqErrorPublisher(IOptions<RabbitMqSettings> options)
    {
        _options = options.Value;
        var factory = new ConnectionFactory { HostName = _options.HostName };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeErrorName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false
        ).Wait();
    }

    public async Task PublishErrorAsync(string message, Exception? ex = null)
    {
        
        var payload = new
        {
            Message = message,
            Exception = ex?.Message,
            StackTrace = ex?.StackTrace,
            CreatedAt = DateTime.UtcNow
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

        await _channel.BasicPublishAsync(
            exchange: _options.ExchangeErrorName,
            routingKey: string.Empty,
            mandatory: false,
            body: body
        );
    }
}