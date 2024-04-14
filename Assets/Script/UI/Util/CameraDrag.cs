using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraDrag
{

    private static Vector3? _dragPositionLastFrame = null;
    private static bool _isDragEnabled = true;

    public static void onWorldClick()
    {
        if (!_isDragEnabled)
        {
            if (_dragPositionLastFrame != null) _dragPositionLastFrame = null;
            return; // Ignore drag behaviour
        }

        Vector3 currentPos = UIUnitsConverter.UiPosToWorldPos(Input.mousePosition);
        if (_dragPositionLastFrame != null)
        {
            Vector3 lastPositionW = UIUnitsConverter.UiPosToWorldPos(_dragPositionLastFrame ?? Vector3.zero);
            Vector3 positionDiff = new Vector3(
                    currentPos.x - lastPositionW.x,
                    currentPos.y - lastPositionW.y,
                    currentPos.z - lastPositionW.z
            );

            float deltaTime = Time.deltaTime;
            UIUnitsConverter.mainCamera.transform.position = new Vector3(
                UIUnitsConverter.mainCamera.transform.position.x - positionDiff.x,
                UIUnitsConverter.mainCamera.transform.position.y - positionDiff.y,
                UIUnitsConverter.mainCamera.transform.position.z - positionDiff.z
                );
        }
        _dragPositionLastFrame = Input.mousePosition;
    }


    public static void stopDragging()
    {
        if (_dragPositionLastFrame != null)
        {
            _dragPositionLastFrame = null;
        }
    }
    
    public static void disableDragging()
    {
        _isDragEnabled = false;
    }
    public static void enableDragging()
    {
        _isDragEnabled = true;
    }
}
