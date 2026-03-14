using FluentValidation;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using YouTubeClone.Shared.Constants;
using YouTubeClone.Shared.DTOs;

namespace YouTubeCloneAPI.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, IHostEnvironment env, ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";

                // Default က 500 Internal Server Error
                var statusCode = (int)HttpStatusCode.InternalServerError;
                var message = AuthMessages.System.InternalServerError;
                object? errors = null; // Validation errors စုဖို့

                // FluentValidation ကလာတဲ့ Exception ဖြစ်နေရင်
                if (ex is ValidationException valEx)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = "Validation failed.";
                    // Error တွေကို Field အလိုက် Group ဖွဲ့လိုက်မယ်
                    errors = valEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.ErrorMessage).ToArray()
                        );
                }
                // Business Logic ကနေ တမင် throw လုပ်လိုက်တဲ့ Exception ဖြစ်နေရင်
                else if (ex is Exception)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = ex.Message;
                }

                context.Response.StatusCode = statusCode;

                var response = env.IsDevelopment()
                    ? new ErrorResponse(statusCode, message, errors, ex.StackTrace?.ToString())
                    : new ErrorResponse(statusCode, message, errors);

                var options = new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
