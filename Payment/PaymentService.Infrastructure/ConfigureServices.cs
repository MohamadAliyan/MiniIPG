using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Common.Models;
using PaymentService.Domain;
using PaymentService.Domain.Common;
using PaymentService.Infrastructure.Common;
using PaymentService.Infrastructure.Persistence;
using Shared.Dto;

namespace PaymentService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        

        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder =>
                    builder.MigrationsAssembly(typeof(ApplicationContext).Assembly
                        .FullName)));
        

        services.Configure<AppSettings>(configuration);
        services.AddMemoryCache();

       Helper. ResolveAllTypes(services, ServiceLifetime.Scoped, typeof(BaseRepository<>),
            "Repository");
        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        return services;
    }

}