using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using System.Reflection;

namespace PaymentService.Infrastructure.Persistence;

public class ApplicationContext:DbContext
{
    public ApplicationContext(DbContextOptions options         ) :

        base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
    public DbSet<Transaction> Transactions { get; set; }
}