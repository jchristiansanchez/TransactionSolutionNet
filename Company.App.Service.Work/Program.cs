using Company.App.Service.Work;
using Company.App.Application.UseCases;
using Company.App.Infrastructure;
using Company.App.Persistence;
using Serilog;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TransactionConsumer>();

builder.Services.AddHostedService<TransactionConsumer>();

builder.Services.AddApplicationInjectionServices(builder.Configuration);
builder.Services.AddInfrastructureInjectionServices(builder.Configuration, builder.Environment);
builder.Services.AddPersistenceInjectionServices(builder.Configuration);





var host = builder.Build();
host.Run();
