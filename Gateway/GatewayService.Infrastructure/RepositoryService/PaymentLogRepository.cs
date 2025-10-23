
using Microsoft.EntityFrameworkCore;
using GatewayService.Domain.Entities;
using GatewayService.Infrastructure.Common;
using GatewayService.Infrastructure.Persistence;

namespace GatewayService.Infrastructure.RepositoryService;

public class PaymentLogRepository : BaseRepository<PaymentLog>, IPaymentLogRepository
{
    private readonly DbSet<PaymentLog> _entity;

    public PaymentLogRepository(ApplicationContext context) : base(context)
    {
        _entity = context.Set<PaymentLog>();
    }


}