using GatewayService.Application.Common.Interfaces;
using GatewayService.Application.Gateways.Dtos;
using MediatR;
using Refit;
using Shared;
using Shared.Dto;
using Shared.Messaging;
using System.Text.Json;
using GatewayService.Domain.Entities;
using static System.Net.WebRequestMethods;


namespace GatewayService.Application.Gateways.Commands.CreatePay;

public class CreatePayCommandHandler : IRequestHandler<CreatePayCommand, Result<PayResponseResult>>
{

    private readonly IPaymentApi _paymentApi;
    private readonly IEventPublisher _eventPublisher;
    private readonly IPaymentLogRepository _paymentLogRepository;

    public CreatePayCommandHandler(IPaymentApi paymentApi, IEventPublisher eventPublisher, IPaymentLogRepository paymentLogRepository)
    {
        _paymentApi = paymentApi;
        _eventPublisher = eventPublisher;
        _paymentLogRepository = paymentLogRepository;
    }

    public async Task<Result<PayResponseResult>> Handle(CreatePayCommand request, CancellationToken cancellationToken)
    {
        var isSuccess = new Random().NextDouble() < 0.8; // 80% موفق
        var res = new PayResponseResult
        {
            Token = request.Token,
            RedirectUrl = "http://siteTest.com/Result"
        };

        if (isSuccess)
        {
            try
            {
                var resVerifiy = await _paymentApi.VerifyPaymentAsync(new VerifyPaymentRequest
                {
                    Token = request.Token,
                    AppCode = "MyAppCode123"
                });
                if (resVerifiy.IsSuccess && resVerifiy.Data is not null)
                {
                    var random = new Random();
                    var random12Digit = Helper.Generate12DigitNumber(random);
                    {
                        res.Amount = resVerifiy.Data.Amount;
                        res.IsSuccess = true;
                        res.Rrn = random12Digit.ToString("D12");


                        var resUpdateStatus = await _paymentApi.UpdateStatusAsync(new UpdateStatusPaymentRequest
                        {
                            Token = request.Token,
                            Rrn = res.Rrn,
                            IsSuccess = res.IsSuccess
                        });

                        _paymentLogRepository.AddAsync(new PaymentLog()
                        {
                            IsSuccess = res.IsSuccess,
                            Token = res.Token,
                            RRN = res.Rrn,
                            Amount = res.Amount,
                            CreatedAt = DateTime.UtcNow,
                            ProcessedAt = DateTime.UtcNow,
                            Id = Guid.NewGuid(),
                            
                        });
                        await _paymentLogRepository.SaveChangesAsync();

                        return Result<PayResponseResult>.Success(res, resUpdateStatus.Data);
                    }
                }
                else
                {
                    res.IsSuccess=false;
                    var resUpdateStatus = await _paymentApi.UpdateStatusAsync(new UpdateStatusPaymentRequest
                    {
                        Token = request.Token,
                        Rrn = res.Rrn,
                        IsSuccess = res.IsSuccess
                    });
                    _paymentLogRepository.AddAsync(new PaymentLog()
                    {
                        IsSuccess = res.IsSuccess,
                        Token = res.Token,
                        RRN = res.Rrn,
                        Amount = res.Amount,
                        CreatedAt = DateTime.UtcNow,
                        ProcessedAt = DateTime.UtcNow,
                        Id = Guid.NewGuid(),

                    });
                    await _paymentLogRepository.SaveChangesAsync();
                    return Result<PayResponseResult>.Failure(res, "پرداخت ناموفق بود");
                }


            }
            catch (ApiException ex)
            {

                // var errorBody = await ex.GetContentAsAsync<Result<PayResponseResult>>(); // اگه بدنه JSON خطا داره

                var json = ex.Content; // بدنه خام
                if (string.IsNullOrWhiteSpace(json))
                    throw new Exception("s"); ;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    
                };

                var result = JsonSerializer.Deserialize<ErrorPayResponseResult>(json, options);

                var paymentEvent = new PaymentProcessedEvent
                {
                    Token = request.Token,
                    Status = PaymentStatus.Failed.ToString(),
                    Amount = 0,
                    Rrn = string.Empty,
                    ReservationNumber = string.Empty,
                    AppCode = "MyAppCode123",
                    CreatedAt = DateTime.UtcNow
                };

                await _eventPublisher.PublishAsync(paymentEvent, cancellationToken);
                return Result<PayResponseResult>.Failure(res, "پرداخت ناموفق بود");



                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }
        else
        {
            res.IsSuccess = false;
            _paymentLogRepository.AddAsync(new PaymentLog()
            {
                IsSuccess = res.IsSuccess,
                Token = res.Token,
                RRN = res.Rrn,
                Amount = res.Amount,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),

            });
            await _paymentLogRepository.SaveChangesAsync();
            return Result<PayResponseResult>.Failure(res, "پرداخت ناموفق بود");

        }


        //transaction.AppCode = request.AppCode;
        //transaction.UpdatedAt = DateTime.UtcNow;

        //if (isSuccess)
        //{
        //    transaction.Status = TransactionStatus.Success;
        //    transaction.Rrn = new Random().NextInt64(100000000000, 999999999999).ToString();
        //    await _repository.UpdateAsync(transaction, cancellationToken);

        //    var dto = new VerifyTransactionResultDto
        //    {
        //        Status = transaction.Status.ToString(),
        //        Amount = transaction.Amount,
        //        Rrn = transaction.Rrn,
        //        ReservationNumber = transaction.ReservationNumber,
        //        Message = "پرداخت با موفقیت تایید شد"
        //    };

        //    return Result<VerifyTransactionResultDto>.Success(dto);
        //}
        //else
        //{
        //    transaction.Status = TransactionStatus.Failed;
        //    await _repository.UpdateAsync(transaction, cancellationToken);

        //    var dto = new VerifyTransactionResultDto
        //    {
        //        Status = transaction.Status.ToString(),
        //        Amount = transaction.Amount,
        //        Rrn = null,
        //        ReservationNumber = transaction.ReservationNumber,
        //        Message = "پرداخت ناموفق بود"
        //    };

        //    return Result<VerifyTransactionResultDto>.Failure(dto, "پرداخت ناموفق بود");
        //}


        //// publish event (GatewayCreated)
        //var evt = new GatewayCreatedEvent(Gateway.Token, Gateway.Amount, Gateway.CreatedAt);
        //await _publisher.PublishAsync("Gateway.events", "GatewayCreated", evt, cancellationToken);

        //return new CreateGatewayResultDto(Gateway.Token, Gateway.Status.ToString(), Gateway.ExpiresAt);
    }
}
