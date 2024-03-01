using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtils : MonoBehaviour
{
    public static float BIG_G = 6.6743e-11f;
    public static int timeWarp = 1000;
    public static bool isPaused = true;

    public static Vector2 getGravityAcceleration(CelestialBodyScript fromBody, CelestialBodyScript toBody)
    {
        float distance = getDistanceBetweenCoords(fromBody.metricPosition, toBody.metricPosition);
        float minDistance = (fromBody.diameterInMeters + toBody.diameterInMeters) / 2; // Min cap distance based on the radius

        if (distance < minDistance) 
        {
            distance = minDistance;
        }

        float acceleration = (BIG_G * toBody.mass) / (float)Math.Pow(distance, 2);
        Vector2 speedDistribution = getSpeedDistribution(fromBody.metricPosition, toBody.metricPosition, distance);
        speedDistribution.x = speedDistribution.x * acceleration;
        speedDistribution.y = speedDistribution.y * acceleration;

        return speedDistribution;
    }

    public static Vector2 getSpeedDistribution(Vector2 pos1, Vector2 pos2)
    {
        float distance = getDistanceBetweenCoords(pos1, pos2);
        return getSpeedDistribution(pos1, pos2, distance);
    }
    public static Vector2 getSpeedDistribution(Vector2 pos1, Vector2 pos2, float distance)
    {
        Vector2 speedDistribution = new Vector2();
        speedDistribution.x = (pos1.x - pos2.x) / distance;
        speedDistribution.y = (pos1.y - pos2.y) / distance;
        return speedDistribution;
    }

    public static float getDistanceBetweenCoords(Vector2 coord1, Vector2 coord2)
    {
        return (float)Math.Sqrt(
            Math.Pow(
                Math.Abs(coord1.x - coord2.x)
                , 2) + 
            Math.Pow(
                Math.Abs(coord1.y - coord2.y)
                , 2)
         );
    }
}
