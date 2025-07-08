using System.Collections.Generic;
using UnityEngine;

public static class TargetVisibilityHelper
{
    private static List<Vector3> GetFaceSamplePoints(Vector3 center, Vector3 right, Vector3 up)
    {
        List<Vector3> points = new();

        Vector3 topLeft = center - right + up;
        Vector3 topRight = center + right + up;
        Vector3 bottomLeft = center - right - up;
        Vector3 bottomRight = center + right - up;

        points.Add(topLeft);
        points.Add(topRight);
        points.Add(bottomLeft);
        points.Add(bottomRight);

        points.Add((topLeft + topRight) * 0.5f);     // Top edge center
        points.Add((bottomLeft + bottomRight) * 0.5f); // Bottom edge center
        points.Add((topLeft + bottomLeft) * 0.5f);     // Left edge center
        points.Add((topRight + bottomRight) * 0.5f);   // Right edge center

        points.Add(center); // Face center

        points.Add((topLeft + center) * 0.5f);
        points.Add((topRight + center) * 0.5f);
        points.Add((bottomLeft + center) * 0.5f);
        points.Add((bottomRight + center) * 0.5f);

        return points;
    }

    public static List<Vector3> GetAllSamplePoints(Bounds bounds)
    {
        List<Vector3> allPoints = new();

        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3 right = Vector3.right * extents.x;
        Vector3 up = Vector3.up * extents.y;
        Vector3 forward = Vector3.forward * extents.z;

        allPoints.AddRange(GetFaceSamplePoints(center + forward, right, up));   // Front
        allPoints.AddRange(GetFaceSamplePoints(center - forward, right, up));   // Back
        allPoints.AddRange(GetFaceSamplePoints(center + right, forward, up));   // Right
        allPoints.AddRange(GetFaceSamplePoints(center - right, forward, up));   // Left
        allPoints.AddRange(GetFaceSamplePoints(center + up, right, forward));   // Top
        allPoints.AddRange(GetFaceSamplePoints(center - up, right, forward));   // Bottom

        return allPoints;
    }
}
