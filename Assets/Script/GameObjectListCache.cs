using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectListCache : MonoBehaviour
{
    public static List<GameObject> CelestialBodies = new List<GameObject>();
    
    public static void addCelestialBodyToCacheList(GameObject celestialBody)
    {
        CelestialBodies.Add(celestialBody);
    }
}
