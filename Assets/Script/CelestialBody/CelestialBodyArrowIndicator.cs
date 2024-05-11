using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CelestialBodyArrowIndicator : MonoBehaviour
{
    public GameObject celestialBodyElement;
    public GameObject axisElement;

    private static CelestialBodyScript celestialBodyScript;
    private static GameObject draggingTarget;
    private static Vector2 lastMousePosition;

    public void updateRotation(Vector2 velocity)
    {

        // Calculate the angle in radians
        float angleRadians = Mathf.Atan2(velocity.y, velocity.x);

        // Convert radians to degrees
        float angleDegrees = angleRadians * Mathf.Rad2Deg - 360;

        // Adjust to 0-360 degrees if necessary
        if (angleDegrees < 0)
        {
            angleDegrees -= 360;
        }

        Quaternion rotation = Quaternion.Euler(0,0,angleDegrees);
        axisElement.transform.rotation = rotation;
    }

    private static void onDragArrow()
    {
        CelestialBodyArrowIndicator self = draggingTarget.GetComponent<CelestialBodyArrowIndicator>();
        CelestialBodyScript cBodyScript = self.celestialBodyElement.GetComponent<CelestialBodyScript>();

        Vector2 currentMousePosition = Input.mousePosition;
        Vector3 wPos = UIUnitsConverter.UiPosToWorldPos(currentMousePosition);
        Vector3 cBodyPos = self.celestialBodyElement.transform.position;
        Vector2 direction = wPos - cBodyPos;
        direction.Normalize();
        float angleRadians = (float)Math.Atan2(direction.y, direction.x);
        float speed = (float)Math.Sqrt(Math.Pow(cBodyScript.velocity.x, 2) + Math.Pow(cBodyScript.velocity.y, 2));
        Vector2 velocity = new Vector2(speed * (float)Math.Cos(angleRadians), speed * (float)Math.Sin(angleRadians));
        cBodyScript.velocity = velocity;
        self.updateRotation(cBodyScript.velocity);
        lastMousePosition = currentMousePosition;
    }

    private static void dragCBody()
    {

        Vector2 currentMousePosition = Input.mousePosition;
        Vector3 wPos = UIUnitsConverter.UiPosToWorldPos(currentMousePosition);
        draggingTarget.transform.position = wPos;
        celestialBodyScript.calculateMetricPosition();
    }


    public static void onWorldClick()
    {
        if (draggingTarget != null)
        {
            if (draggingTarget.tag == "ArrowIndicator")
            {
                onDragArrow();
            } else if (draggingTarget.tag == "CelestialBody")
            {
                dragCBody();
            }
            return;
        }

        RaycastHit2D[] worldHits = UIUnitsConverter.raycastMousePos();
        GameObject arrowHit = null;
        GameObject celestialBodyHit = null;
        CelestialBodyScript celestialBodyHitScript = null;
        foreach (RaycastHit2D hit in worldHits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.gameObject.tag == "ArrowIndicator")
            {
                arrowHit = hit.collider.gameObject;
            } else if (hit.collider.gameObject.tag == "CelestialBody")
            {
                CelestialBodyScript _celestialBodyHitScript = hit.collider.gameObject.GetComponent<CelestialBodyScript>();
                if (_celestialBodyHitScript.isSelected)
                {
                    celestialBodyHit = hit.collider.gameObject;
                    celestialBodyHitScript = _celestialBodyHitScript;
                }
            }
        }

        if (arrowHit != null)
        {
            draggingTarget = arrowHit;
            lastMousePosition = Input.mousePosition;
            CameraDrag.disableDragging();
        } else if (celestialBodyHit != null)
        {
            celestialBodyScript = celestialBodyHitScript;
            celestialBodyScript.isManuallyMoving = true;
            draggingTarget = celestialBodyHit;
            lastMousePosition = Input.mousePosition;
            CameraDrag.disableDragging();
        }
    }

    public static void onWorldRelease()
    {
        if (draggingTarget != null)
        {
            if (celestialBodyScript != null)
            {
                celestialBodyScript.isManuallyMoving = false;
                celestialBodyScript = null;
            }
            draggingTarget = null;
            CameraDrag.enableDragging();
        }
    }
}
