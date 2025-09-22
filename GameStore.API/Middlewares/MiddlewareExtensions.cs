using System.Net;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GameStore.API.Middlewares;

public static class MiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature == null)
                {
                    return;
                }

                var (statusCode, message) = GetExceptionResponse(contextFeature.Error);

                var errorResponse = new
                {
                    StatusCode = statusCode,
                    Message = message,
                    Details = contextFeature.Error.Message, // Detailed message for logging or debugging
                };

                LogException(contextFeature.Error, errorResponse, logger);

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            });
        });
    }

    private static (int StatusCode, string Message) GetExceptionResponse(Exception exception)
    {
        var exceptionMappings = new Dictionary<Type, (HttpStatusCode, string)>
        {
            { typeof(IdsNotValidException), (HttpStatusCode.BadRequest, "Invalid IDs provided.") },
            { typeof(KeyIsNotValidException), (HttpStatusCode.BadRequest, "Invalid key provided.") },
            { typeof(UniquePropertyException), (HttpStatusCode.BadRequest, "Existing unique property provided.") },
            { typeof(EntityCreationException), (HttpStatusCode.BadRequest, "Creation of an entity failed.") },
            { typeof(UserUpdateException), (HttpStatusCode.BadRequest, "Update of user credentials failed.") },
            { typeof(EntityNotFoundException), (HttpStatusCode.NotFound, "The requested entity was not found.") },
            { typeof(DbUpdateException), (HttpStatusCode.BadRequest, "Database update failed.") },
            { typeof(DbUpdateConcurrencyException), (HttpStatusCode.Conflict, "The record you attempted to edit was modified by another user.") },
            { typeof(UnauthorizedAccessException), (HttpStatusCode.Unauthorized, "Invalid credentials.") },
            { typeof(UnauthenticatedException), (HttpStatusCode.Forbidden, "Invalid credentials.") },
            { typeof(ArgumentException), (HttpStatusCode.BadRequest, "Invalid input provided.") },
            { typeof(ArgumentNullException), (HttpStatusCode.BadRequest, "Invalid input provided.") },
        };

        return exceptionMappings.TryGetValue(exception.GetType(), out var response)
            ? ((int)response.Item1, response.Item2)
            : ((int)HttpStatusCode.InternalServerError, "Internal Server Error. Please try again later.");
    }

    private static void LogException(Exception ex, object response, ILogger logger)
    {
        if (ex is IdsNotValidException or KeyIsNotValidException or UniquePropertyException or EntityNotFoundException)
        {
            logger.LogWarning(
                        ex,
                        "Validation exception was caght.\nResponse: {ErrorResponse}",
                        JsonConvert.SerializeObject(response, Formatting.Indented));
        }
        else
        {
            logger.LogError(
                        ex,
                        "An exception was handled by the exception handler.\nError response: {ErrorResponse}",
                        JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}
