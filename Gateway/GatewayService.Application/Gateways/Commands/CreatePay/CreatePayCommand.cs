using GatewayService.Application.Gateways.Dtos;
using MediatR;
using Shared;


namespace GatewayService.Application.Gateways.Commands.CreatePay;

public record CreatePayCommand(
    string Token


) : IRequest<Result<PayResponseResult>>;