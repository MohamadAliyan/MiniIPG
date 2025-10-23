using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Payments.Commands;

namespace PaymentService.API.Controllers;

public class PaymentController : ApiControllerBase
{

    public PaymentController(IMediator mediator)
        : base(mediator) { }

    [HttpPost("get-token")]
    public async Task<IActionResult> GetToken([FromBody] CreateTransactionCommand request)
    {
        return await SendRequest(request);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyToken([FromBody] VerifyTransactionCommand request)
    {
        return await SendRequest(request);
    }

    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusTransactionCommand request)
    {
        return await SendRequest(request);
    }
}