using System;
using UnityEngine;

public static class WaterCurrent
{
    public static float strength = 5f;
    public static float speed = 5f;

    public static Vector2 GetWaterCurrentForce(Vector2 objVelocity)
    {
        float speedDiff = speed - objVelocity.y;
        float force = speedDiff * strength;
        return Vector2.up * force;
    }
    
}
