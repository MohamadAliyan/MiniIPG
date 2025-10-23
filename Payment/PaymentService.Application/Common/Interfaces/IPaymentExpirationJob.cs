namespace PaymentService.Application.Common.Interfaces;

public interface IPaymentExpirationJob
{
    Task ExpirePendingTransactionsAsync();
}

