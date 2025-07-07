[System.Serializable]
public class SearchZoneSummary
{
    public string id;
    public SerializableVector3 center;
    public SerializableVector3 size;

    public SearchZoneSummary(SearchZoneData data)
    {
        id = data.id;
        center = data.center;
        size = data.size;
    }
}