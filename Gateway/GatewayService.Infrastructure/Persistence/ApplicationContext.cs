using Microsoft.EntityFrameworkCore;
using GatewayService.Domain.Entities;
using System.Reflection;

namespace GatewayService.Infrastructure.Persistence;

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
    public DbSet<PaymentLog> PaymentLogs { get; set; }
}