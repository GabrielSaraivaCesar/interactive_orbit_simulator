using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtils : MonoBehaviour
{
    public float BIG_G = 6.6743e-11f;

    public Vector2 getGravityAcceleration(CelestialBodyScript fromBody, CelestialBodyScript toBody)
    {
        float distance = getDistanceBetweenCoords(fromBody.metricPosition, toBody.metricPosition);
        float acceleration = (BIG_G * toBody.mass) / (float)Math.Pow(distance, 2);
        Vector2 speedDistribution = getSpeedDistribution(fromBody.metricPosition, toBody.metricPosition, distance);
        speedDistribution.x = speedDistribution.x * acceleration;
        speedDistribution.y = speedDistribution.y * acceleration;

        return speedDistribution;
    }

    public Vector2 getSpeedDistribution(Vector2 pos1, Vector2 pos2)
    {
        float distance = getDistanceBetweenCoords(pos1, pos2);
        return getSpeedDistribution(pos1, pos2, distance);
    }
    public Vector2 getSpeedDistribution(Vector2 pos1, Vector2 pos2, float distance)
    {
        Vector2 speedDistribution = new Vector2();
        speedDistribution.x = pos1.x / distance;
        speedDistribution.y = pos1.y / distance;
        return speedDistribution;
    }

    public float getDistanceBetweenCoords(Vector2 coord1, Vector2 coord2)
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
