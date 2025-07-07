public static class MQTTConstants
{
    // MQTT Broker Default Settings
    public const int BrokerPort = 1883;
    public const string DefaultBrokerIP = "127.0.0.1";
    public const string Username = "admin";
    public const string Password = "admin";

    // MQTT Client ID
    public const string ClientId = "UnityClient"; // Added this constant

    // MQTT Topics (centralized for easy updates)
    //public const string DronePositionTopic = "drone/position"; //CREO QUE YA NO SE USA
    public const string DroneStatusTopic = "drone/status";
    public const string CommandTopic = "drone/commands";
    public const string SelectedDroneTopic = "droneselection/selected";
    public const string DroneCameraTopic= "drone/droneCamera";

    // TOPICS relacionados con tareas
    public const string PendingTasksTopic = "drone/tasks/pending";       // ← Para publicar tareas pendientes
    public const string PendingTasksRequestTopic = "drone/tasks/request"; // ← (Opcional) Si quieres que el piloto pida tareas

    // Commander add layers to map Topics
    public const string SearchingZone = "zone/area";

    public const string SelectedTaskTopic = "task/selected";   // → mensaje piloto → comandante
    public const string TaskStatusChangedTopic = "task/status"; // (opcional) notificar cualquier cambio de estado  
    public const string PilotReadyForSearchingZone = "pilot/ready/searchzones";

    public const string PendingSearchZonesTopic = "searchzones/pending";
    public const string PendingSearchZonesRequestTopic = "searchzones/request";
}
