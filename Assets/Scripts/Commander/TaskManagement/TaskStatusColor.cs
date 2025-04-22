using UnityEngine;

public static class TaskStatusColor
{
    public static Color GetColorForStatus(string status)
    {
        switch (status)
        {
            case "To be executed":
                return HexToColor("#FDBA74");
            case "Executing...":
                return HexToColor("#7DD3FC");
            case "Finished":
                return HexToColor("#84CC16");
            default:
                return Color.gray;
        }
    }

    private static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;

        Debug.LogWarning($"⚠️ No se pudo convertir el color HEX: {hex}");
        return Color.magenta; // color de fallo visual
    }
}
