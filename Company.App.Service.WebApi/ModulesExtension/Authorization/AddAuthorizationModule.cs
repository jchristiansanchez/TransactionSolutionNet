namespace Company.App.Service.WebApi.ModulesExtension.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("Policy-Transaction", politica => politica.RequireRole("transaction"));                
            });

            return services;
        }
    }
}
