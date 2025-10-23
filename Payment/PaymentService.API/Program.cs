using PaymentService.Infrastructure;
using PaymentService.Application;
using System.Reflection;
using Hangfire;
using Hangfire.SqlServer;
using Shared.Messaging;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Infrastructure.Jobs;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.Load("PaymentService.Application")));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
builder.Services.AddSingleton<RabbitMqErrorPublisher>(); ;

builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
        {
            SchemaName = "Hangfire"
        });
});
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IPaymentExpirationJob, PaymentExpirationJob>();
var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<IPaymentExpirationJob>(
    "expire-pending-payments",
    job => job.ExpirePendingTransactionsAsync(),
    "*/30 * * * * *" 
);

app.MapControllers();

app.Run();


