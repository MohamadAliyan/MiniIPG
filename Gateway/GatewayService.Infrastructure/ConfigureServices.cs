using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GatewayService.Application;
using GatewayService.Application.Common.Interfaces;
using GatewayService.Application.Common.Models;
using GatewayService.Domain;
using GatewayService.Domain.Common;
using GatewayService.Infrastructure.Common;
using GatewayService.Infrastructure.Persistence;
using Refit;
using Shared.Dto;

namespace GatewayService.Infrastructure;

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

        Helper.ResolveAllTypes(services, ServiceLifetime.Scoped, typeof(BaseRepository<>),
            "Repository");
        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        services
            .AddRefitClient<IPaymentApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("http://localhost:5001"); 
                c.Timeout = TimeSpan.FromSeconds(120);
            });

        return services;
    }


}