using System.Globalization;

namespace GameStore.API.Helpers;

public static class DateTimeParser
{
    public static DateTime? ParseDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return null;
        }

        // Extract only the date part before "GMT"
        string cleanedDate = dateString.Split("GMT")[0].Trim();

        // Define possible date formats (without GMT)
        string[] formats =
            ["ddd MMM dd yyyy HH:mm:ss",
            "ddd MMM d yyyy HH:mm:ss"];

        if (DateTime.TryParseExact(cleanedDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            return parsedDate;
        }

        return null; // Return null if parsing fails
    }
}
