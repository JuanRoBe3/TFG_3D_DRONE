public static class LogMessagesConstants
{
    // UI Labels
    public const string DronePositionPrefix = "Drone Position: ";

    // Error Messages
    public const string ErrorMQTTClientNotFound = "❌ MQTTClient not found in the scene!";
    public const string ErrorPositionTextNotAssigned = "❌ UI Text component (positionText) is not assigned in Inspector!";
    public const string ErrorSceneNotFound = "❌ Scene not found! Check SceneConstants.";
    public const string ErrorCannotSubscribeMQTT = "❌ Cannot subscribe, the client is not connected.";
    public const string ErrorMQTTConnectionFailed = "❌ Error connecting to MQTT: ";
    public const string ErrorMessageNotSent = "⚠️ The message could not be sent, the client is not connected.";

    // Warning Messages
    public const string WarningPositionTextNull = "⚠️ positionText is null, cannot update UI.";
    public const string WarningDuplicateMQTTClient = "⚠️ Duplicate MQTTClient detected and destroyed.";
    public const string WarningNoIPSet = "⚠️ No IP address set for the broker. Using default.";
    public const string WarningTryingToLoadSameScene = "⚠️ Attempted to load the current scene again.";
    public const string WarningTryingSubscribeWithNoClient = "⚠️ Cannot unsubscribe, client is not connected.";

    // Debug Messages
    public const string DebugMQTTConnected = "✅ Connected to the MQTT broker: ";
    public const string DebugMQTTSubscribed = "🔔 Subscribed to topic: ";
    public const string DebugMQTTUnsubscribed = "🔕 Unsubscribed from all topics.";
    public const string DebugMQTTPublished = "📤 Message sent: ";
    public const string DebugBrokerIPUpdated = "🔄 MQTT Broker IP updated: ";
    public const string DebugBrokerIPReset = "🔄 MQTT Broker IP reset to default: ";
    public const string DebugMQTTMessageReceived = "📩 Message received on topic: ";
}