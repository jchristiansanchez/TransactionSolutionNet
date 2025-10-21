using System.Threading.RateLimiting;

namespace Company.App.Service.WebApi.ModulesExtension.RateLimiter
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddRatelimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRateLimiter(options =>
            {
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
                };

                // Política: Fixed Window
                options.AddPolicy("GetFixedWindowLimiter", context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "desconocido",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = int.Parse(configuration["RateLimiting:PermitLimit"]!),
                            Window = TimeSpan.FromSeconds(int.Parse(configuration["RateLimiting:Window"]!)),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = int.Parse(configuration["RateLimiting:QueueLimit"]!)
                        });
                });

                // Política: Sliding Window
                options.AddPolicy("GetSlidingWindowLimiter", context =>
                {
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "desconocido",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = int.Parse(configuration["RateLimiting:PermitLimit"]!),
                            Window = TimeSpan.FromSeconds(int.Parse(configuration["RateLimiting:Window"]!)),
                            SegmentsPerWindow = 2,
                            QueueLimit = int.Parse(configuration["RateLimiting:QueueLimit"]!),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });               
            });

            return services;
        }
    }
}
