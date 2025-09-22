namespace GameStore.API.Helpers.RequestContext;

public interface IRequestLocalizationContext
{
    string CurrentLanguage { get; set; }
}
