
using Company.App.Application.UseCases;
using Company.App.Infrastructure;
using Company.App.Persistence;
using Company.App.Service.WebApi.ModulesExtension.Authentication;
using Company.App.Service.WebApi.ModulesExtension.Authorization;
using Company.App.Service.WebApi.ModulesExtension.Cors;
using Company.App.Service.WebApi.ModulesExtension.MiddlewareExtension;
using Company.App.Service.WebApi.ModulesExtension.RateLimiter;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationInjectionServices(builder.Configuration);
builder.Services.AddInfrastructureInjectionServices(builder.Configuration, builder.Environment);
builder.Services.AddPersistenceInjectionServices(builder.Configuration);

builder.Services.AddAuthenticationModule(builder.Configuration);
builder.Services.AddAuthorizationModule(builder.Configuration);

builder.Services.AddTransient<GlobalExceptionMiddleware>();

builder.Services.AddRatelimiting(builder.Configuration);

builder.Services.AddCorsModule(builder.Configuration);

// Logger
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithClientIp()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    /*
     el id se genera automaticamente: la configuracion en el appsetting: CorrelationId: {CorrelationId}
    .Enrich.WithCorrelationId()
     */
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

// Enable Cors
app.UseCors("CorsPolicy");

// Validar si el token es Válido
app.UseAuthentication();

app.UseAuthorization();

// MiddlewareExtension
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
