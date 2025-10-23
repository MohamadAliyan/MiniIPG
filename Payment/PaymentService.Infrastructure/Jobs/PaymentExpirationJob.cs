using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Transactions;
using Dapper;
using Shared;
using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Infrastructure.Jobs;

public class PaymentExpirationJob : IPaymentExpirationJob
{

    private readonly IConfiguration _config;
    private readonly ILogger<PaymentExpirationJob> _logger;

    public PaymentExpirationJob(IConfiguration config, ILogger<PaymentExpirationJob> logger)
    {
        _config = config;
        _logger = logger;
    }
    public async Task ExpirePendingTransactionsAsync()
    {
        try
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string sql = @"
                    UPDATE Transactions
                    SET Status = @Expired,
                        UpdatedAt = GETUTCDATE()
                    WHERE Status = @Pending
                    AND DATEDIFF(SECOND, CreatedAt, GETUTCDATE()) >= 120;";

            int affectedRows = await connection.ExecuteAsync(sql, new
            {
                PaymentStatus.Pending,
                PaymentStatus.Expired
            });

            if (affectedRows > 0)
                _logger.LogInformation("⏰ {Count} transaction(s) expired at {Time}", affectedRows, DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in ExpirePendingTransactionsAsync job");
        }
    }
}