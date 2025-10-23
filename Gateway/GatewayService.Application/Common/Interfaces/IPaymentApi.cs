using GatewayService.Application.Gateways.Dtos;
using Refit;
using Shared;

namespace GatewayService.Application.Common.Interfaces;

public interface IPaymentApi
{
    [Post("/api/payment/verify")]
    Task<Result<VerifyPaymentResponse>> VerifyPaymentAsync([Body] VerifyPaymentRequest request);

    [Post("/api/payment/update-status")]
    Task<Result<string>> UpdateStatusAsync([Body] UpdateStatusPaymentRequest request);
}