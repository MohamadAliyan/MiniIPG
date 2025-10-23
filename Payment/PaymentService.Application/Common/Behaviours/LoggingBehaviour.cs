using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace PaymentService.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger;


    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        string? userName = string.Empty;


        _logger.LogInformation("clean Request: {Name}  {@UserName} {@Request}", requestName,
            userName, request);
    }
}
