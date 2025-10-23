using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private readonly IMediator _mediator;
    protected ApiControllerBase(IMediator mediator)
    {
        _mediator = mediator;

    }
    protected async Task<IActionResult> SendRequest<T>(IRequest<Result<T>> request)
    {
        try
        {
            var result = await _mediator.Send(request);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
        catch (ValidationException ex)
        {
            var errorMessage = string.Join(" | ", ex.Errors.Select(e => e.ErrorMessage));
            return BadRequest(Result<T>.Failure($"خطای اعتبارسنجی: {errorMessage}"));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            return StatusCode(400, Result<T>.Failure($"خطای داخلی سرور: {ex.Message}"));
        }
    }

    protected async Task<IActionResult> SendRequest(IRequest<Result> request)
    {
        try
        {
            var result = await _mediator.Send(request);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
        catch (ValidationException ex)
        {
            var errorMessage = string.Join(" | ", ex.Errors.Select(e => e.ErrorMessage));
            return BadRequest(Result.Failure($"خطای اعتبارسنجی: {errorMessage}"));
        }
        catch (Exception ex)
        {
            return StatusCode(400, Result.Failure($"خطای داخلی سرور: {ex.Message}"));
        }
    }
}




