
using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Common;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure.RepositoryService;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    private readonly DbSet<Transaction> _entity;

    public TransactionRepository(ApplicationContext context) : base(context)
    {
        _entity = context.Set<Transaction>();
    }


}