using Company.App.Application.Dto;
using Company.App.Application.Interface.UseCases;
using Company.App.Application.UseCases.Transaction;
using Company.App.Application.Validator;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.App.Application.UseCases
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationInjectionServices(this IServiceCollection services, IConfiguration configuration)    
        {
            services.AddScoped<ITransactionApplication, TransactionApplication>();
            services.AddScoped<IValidator<TransactionDto>, TransactionDtoValidator>();
            services.AddScoped<IValidator<TransactionRequestDto>, TransactionRequestDtoValidator>();

            return services;
        }
    }
}
