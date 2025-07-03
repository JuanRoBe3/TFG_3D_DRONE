using System.Collections.Generic;

public static class MQTTTopicSubscriptions
{
    public static readonly Dictionary<string, List<string>> RoleTopics = new()
    {
        { "Commander", new List<string>
            {
                MQTTConstants.DroneStatusTopic,
                //MQTTConstants.DronePositionTopic,
                MQTTConstants.DroneCameraTopic,
                MQTTConstants.SelectedDroneTopic,
                MQTTConstants.PendingTasksRequestTopic,
                MQTTConstants.SelectedTaskTopic
            }
        },
        { "Pilot", new List<string>
            {
                MQTTConstants.CommandTopic,
                MQTTConstants.PendingTasksTopic,
                MQTTConstants.SearchingZone
            }
        }
    };
}
