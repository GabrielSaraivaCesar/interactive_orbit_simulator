using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectListCache : MonoBehaviour
{
    public static List<GameObject> CelestialBodies = new List<GameObject>();
    public static List<string> colorSchemes = new List<string>();
    
    public static GameObject FindInactiveObjectByName(string parentName, string name)
    {
        GameObject parent = GameObject.Find(parentName);
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public static void addCelestialBodyToCacheList(GameObject celestialBody)
    {
        GameObject mouseTip = FindInactiveObjectByName("CommandTips", "MouseClickTip");
        Debug.Log(mouseTip);
        if (mouseTip != null && mouseTip.activeSelf == false) { 
            mouseTip.SetActive(true);
        }
        CelestialBodies.Add(celestialBody);
    }

    public static void removeCelestialBodyFromCacheList(GameObject celestialBody)
    {
        CelestialBodies.Remove(celestialBody);
        if (CelestialBodies.Count == 0 )
        {
            GameObject mouseTip = FindInactiveObjectByName("CommandTips", "MouseClickTip");
            if (mouseTip != null && mouseTip.activeSelf == true)
            {
                mouseTip.SetActive(false);
            }
        }
    }

    public static Color getRandomCelestialBodyColor()
    {
        float red = 0.5f + Random.Range(0f, 0.5f);
        float green = 0.5f + Random.Range(0f, 0.5f);
        float blue = 0.5f + Random.Range(0f, 0.5f);

        return new Color(red, green, blue);
    }
}
