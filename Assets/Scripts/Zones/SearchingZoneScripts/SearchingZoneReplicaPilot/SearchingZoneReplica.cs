using UnityEngine;

public class SearchingZoneReplica : MonoBehaviour
{
    public void Init(SearchZoneData data)
    {
        transform.position = data.center.ToUnityVector3();
        transform.localScale = data.size.ToUnityVector3();
    }
}
