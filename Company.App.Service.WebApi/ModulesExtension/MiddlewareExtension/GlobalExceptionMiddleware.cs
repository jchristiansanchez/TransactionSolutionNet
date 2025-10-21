using Company.App.Cross.Common;
using System.Net;
using System.Text.Json;

namespace Company.App.Service.WebApi.ModulesExtension.MiddlewareExtension
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Details: {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                string message = ex.Message.ToString();

                var response = new ResponseT<object>()
                {
                    Message = message,
                    IsSuccess = false
                };

                await JsonSerializer.SerializeAsync(context.Response.Body, response);
            }
        }
    }
}
