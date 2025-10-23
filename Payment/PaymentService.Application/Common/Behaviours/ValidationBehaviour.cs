using System.Text;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace PaymentService.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<ValidationFailure> errors = _validators.Select(v => v.Validate(request))
            .SelectMany(result => result.Errors).Where(error => error != null).ToList();

        if (errors.Any())
        {
            StringBuilder errorBuilder = new();

            errorBuilder.AppendLine("Invalid command, reason: ");

            foreach (ValidationFailure? error in errors)
            {
                errorBuilder.AppendLine(error.ErrorMessage);
            }

            throw new Exception(errorBuilder.ToString());
        }

        return await next();
    }
}
