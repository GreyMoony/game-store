using System.Globalization;
using System.Text.RegularExpressions;

namespace GameStore.Application.Helpers;
public static partial class KeyStringHelper
{
    public static string CreateKey(string name)
    {
        string key = name.ToLower(CultureInfo.CurrentCulture).Replace(" ", "-");

        key = GameKeyRegex().Replace(key, string.Empty);

        return key;
    }

    // GeneratedRegex method for removing unwanted characters
    [GeneratedRegex(@"[^a-z0-9\-]", RegexOptions.None)]
    private static partial Regex GameKeyRegex();
}
