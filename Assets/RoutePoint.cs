using UnityEngine;

public class RoutePoint : MonoBehaviour
{
    [Tooltip("Identificador opcional de la zona a la que pertenece este punto")]
    public string zoneID = "default";

    [Tooltip("Se marca en true cuando el dron pasa cerca")]
    public bool visited = false;
}
