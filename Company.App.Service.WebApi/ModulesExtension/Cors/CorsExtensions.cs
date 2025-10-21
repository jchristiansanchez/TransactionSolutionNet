namespace Company.App.Service.WebApi.ModulesExtension.Cors
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:7202", "wss://localhost:7036")
                   .AllowCredentials()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
                });


                options.AddPolicy("PermitirFrontendLocal", policy =>
                {
                    policy
                        .AllowAnyOrigin()  // Solo para pruebas locales, ¡NO usar en producción!
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
