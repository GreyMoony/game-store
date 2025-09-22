namespace GameStore.API.Helpers.RequestContext;

public class RequestLocalizationContext : IRequestLocalizationContext
{
    public string CurrentLanguage { get; set; } = "en";
}
