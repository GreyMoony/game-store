using GameStore.API.Helpers.RequestContext;

namespace GameStore.API.Middlewares;

public class RequestLocalizationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IRequestLocalizationContext loc)
    {
        var acceptLanguage = context.Request.Headers.AcceptLanguage.FirstOrDefault();
        var languageCode = ParseAcceptLanguage(acceptLanguage);
        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = "en";
        }

        loc.CurrentLanguage = languageCode;
        await _next(context);
    }

    private static string? ParseAcceptLanguage(string? header)
    {
        if (string.IsNullOrWhiteSpace(header))
        {
            return null;
        }

        // ("en-US" -> "en")
        var first = header.Split(',')[0].Trim();
        var primary = first.Split('-')[0];
        return primary.ToLowerInvariant();
    }
}
