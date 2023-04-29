using UnityEngine;

public static class HexMetrics {

    public const float outerRadius = 1f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public static Vector3[] corners = {
        new Vector3(0f, outerRadius, 0f),
        new Vector3(innerRadius , 0.5f * outerRadius, 0f),
        new Vector3(innerRadius,  -0.5f * outerRadius, 0f),
        new Vector3(0f, -outerRadius, 0f),
        new Vector3(-innerRadius, -0.5f * outerRadius, 0f),
        new Vector3(-innerRadius, 0.5f * outerRadius, 0f),
        new Vector3(0f, outerRadius, 0f)
    };

    public static HexCoordinates[] neighborCoordinates = {
        new HexCoordinates(0, 1),
        new HexCoordinates(1, 0),
        new HexCoordinates(1, -1),
        new HexCoordinates(0, -1),
        new HexCoordinates(-1, 0),
        new HexCoordinates(-1, 1),
    };
    
    public static Vector3 qVector = new Vector3(1.73205080757f, 0f, 0f) * outerRadius;
    
    public static Vector3 rVector = new Vector3(0.866025404f, 1.5f, 0f) * outerRadius;
    
    public static Vector3 GetFirstCorner (HexDirection direction) {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner (HexDirection direction) {
        return corners[(int)direction + 1];
    }
    
    public const float solidFactor = 0.85f;
	
    public const float blendFactor = 1f - solidFactor;
    
    public static Vector3 GetFirstSolidCorner (HexDirection direction) {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner (HexDirection direction) {
        return corners[(int)direction + 1] * solidFactor;
    }
    
    public static Vector3 GetBridge (HexDirection direction) {
        return (corners[(int)direction] + corners[(int)direction + 1]) *
               0.5f * blendFactor;
    }
}