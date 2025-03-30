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
    public const string DronePositionTopic = "drone/position";
    public const string DroneStatusTopic = "drone/status";
    public const string CommandTopic = "drone/commands";
    public const string SelectedDroneTopic = "droneselection/selected";

}
