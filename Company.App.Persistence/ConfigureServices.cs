using Company.App.Application.Interface.Persistence.Context;
using Company.App.Application.Interface.Persistence.Repositories;
using Company.App.Persistence.Context.Dapper;
using Company.App.Persistence.Context.EFCore;
using Company.App.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.App.Persistence
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddPersistenceInjectionServices(this IServiceCollection services, IConfiguration configuration)
        {
            // EF
            services.AddDbContext<EFCoreContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TransactionConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()));

            // Dapper
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}
