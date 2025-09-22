namespace GameStore.Application.Helpers;
#pragma warning disable IDE0305 // Simplify collection initialization
public static class IdParser
{
    public static List<Guid> GetGuids(List<string> ids)
    {
        return ids
            .Where(s => Guid.TryParse(s, out _)) // Filter valid GUIDs
            .Select(Guid.Parse)
            .ToList();
    }

    public static List<int> GetInts(List<string> ids)
    {
        return ids
            .Where(s => int.TryParse(s, out _)) // Filter valid int Ids
            .Select(int.Parse)
            .ToList();
    }
}
#pragma warning restore IDE0305 // Simplify collection initialization
