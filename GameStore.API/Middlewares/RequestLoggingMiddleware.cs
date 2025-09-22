using System.Diagnostics;
using Newtonsoft.Json;

namespace GameStore.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Capture and log request details
        var requestDetails = await FormatRequest(context.Request);

        // Swap the response body stream to capture the response
        var originalResponseBody = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var isImageRequest = context.Request.Path.StartsWithSegments("/api/games")
            && context.Request.Path.Value?.Contains("/image", StringComparison.OrdinalIgnoreCase) == true;
        bool shouldLogResponseBody = context.Request.Path.StartsWithSegments("/api");

        logger.LogInformation("Request Log:\nRequestDetails:\n{RequestDetails}", requestDetails);

        try
        {
            // Proceed with the request
            await next(context);

            stopwatch.Stop();

            // Capture and log response details
            var responseDetails = await FormatResponse(context.Response, shouldLogResponseBody, isImageRequest);

            logger.LogInformation(
                "ResponseDetails:\n{ResponseDetails}\nElapsed Time: {ElapsedTime} ms",
                responseDetails,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception)
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            throw;
        }
        finally
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;
        }
    }

    private static async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();
        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;

        return $"Method: {request.Method}, Path: {request.Path}, IP: {request.HttpContext.Connection.RemoteIpAddress}\n" +
               $"Headers: {string.Join(", ", request.Headers)}\nBody: {requestBody}";
    }

    private static async Task<string> FormatResponse(HttpResponse response, bool logResponseBody, bool isImageRequest)
    {
        if (isImageRequest)
        {
            return $"Status Code: {response.StatusCode}\nBody: <Binary response skipped>";
        }

        if (response.Body.Length > 10_000)
        {
            return $"Status Code: {response.StatusCode}\nBody: <Large response skipped>";
        }

        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = logResponseBody
            ? await new StreamReader(response.Body).ReadToEndAsync()
            : "<Skipped>";
        response.Body.Seek(0, SeekOrigin.Begin); // Reset the stream position for the client

        return $"Status Code: {response.StatusCode}\nBody: {FormatJson(responseBody)}";
    }

    private static string FormatJson(string json)
    {
        try
        {
            var parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
        catch
        {
            // Return as-is if it’s not a valid JSON format
            return json;
        }
    }
}
