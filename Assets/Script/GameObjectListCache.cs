using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectListCache : MonoBehaviour
{
    public static List<GameObject> CelestialBodies = new List<GameObject>();
    public static List<string> colorSchemes = new List<string>();
    
    public static void addCelestialBodyToCacheList(GameObject celestialBody)
    {
        CelestialBodies.Add(celestialBody);
    }

    public static void removeCelestialBodyFromCacheList(GameObject celestialBody)
    {
        CelestialBodies.Remove(celestialBody);
    }

    public static Color getRandomCelestialBodyColor()
    {
        float red = 0.5f + Random.Range(0f, 0.5f);
        float green = 0.5f + Random.Range(0f, 0.5f);
        float blue = 0.5f + Random.Range(0f, 0.5f);

        return new Color(red, green, blue);
    }
}
