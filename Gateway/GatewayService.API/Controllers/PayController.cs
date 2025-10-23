using GatewayService.Application.Gateways.Commands.CreatePay;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.API.Controllers;

public class PayController : ApiControllerBase
{

    public PayController(IMediator mediator)
        : base(mediator) { }

    [HttpGet("pay")]
    public async Task<IActionResult> Pay([FromQuery] CreatePayCommand request)
    {
        return await SendRequest(request);
    }


}