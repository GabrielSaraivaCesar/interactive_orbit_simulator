using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIUnitsConverter
{
    public static Camera mainCamera;

    public static void setUp(Camera cam)
    {
        mainCamera = cam;
    }

    public static Vector3 UiPosToWorldPos(Vector3 uiPosition)
    {
        return UiPosToWorldPos(new Vector2(uiPosition.x, uiPosition.y));
    }
    public static Vector3 UiPosToWorldPos(Vector2 uiPosition)
    {
        Vector3 screenPoint = new Vector3(uiPosition.x, uiPosition.y, mainCamera.nearClipPlane);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPoint);

        // In a 2D world, we ignore the Z component
        worldPosition.z = 0;

        return worldPosition;
    }

    public static RaycastHit2D[] raycastMousePos()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
        return hits;
    }
}
