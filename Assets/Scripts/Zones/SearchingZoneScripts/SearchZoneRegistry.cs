using System.Collections.Generic;

public static class SearchZoneRegistry
{
    private static List<SearchZoneSummary> allZones = new();

    public static void SetZones(List<SearchZoneSummary> zones) =>
        allZones = zones;

    public static List<SearchZoneSummary> GetAllZones() => allZones;

    public static SearchZoneSummary GetZoneById(string id) =>
        allZones.Find(z => z.id == id);
    public static void Register(SearchZoneData data)
    {
        var summary = new SearchZoneSummary(data);
        allZones.Add(summary);
    }

}
