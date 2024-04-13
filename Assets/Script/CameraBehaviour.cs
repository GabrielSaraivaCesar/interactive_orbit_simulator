using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public float unitsToMetersMultiplier = 12_742_000.0f;
    public float zoomSpped = 1.2742e9f;

    public GameObject uiCanvas;
    private UIBehaviour uiBehaviour;

    private void Start()
    {
        uiBehaviour = uiCanvas.GetComponent<UIBehaviour>();
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            onScroll();
        }
    }

    private void onScroll()
    {
        Vector3 mousePos = UIUnitsConverter.UiPosToWorldPos(Input.mousePosition);
        Vector3 mouseMetricPos = unitsToMeters(mousePos);
        float delta = (Input.mouseScrollDelta.y * -1 * zoomSpped) * Time.deltaTime;
        unitsToMetersMultiplier += delta;
        if (unitsToMetersMultiplier < 2000000)
        {
            unitsToMetersMultiplier = 2000000;
        } 

        Vector3 mouseNewMetricPos = unitsToMeters(mousePos);

        Vector3 mouseMectricDelta = new Vector3(
            mouseMetricPos.x - mouseNewMetricPos.x,
            mouseMetricPos.y - mouseNewMetricPos.y,
            mouseMetricPos.z - mouseNewMetricPos.z
            );

        Vector3 mouseRealDelta = metersToUnits(mouseMectricDelta);
        transform.position = new Vector3(
            transform.position.x + mouseRealDelta.x,
            transform.position.y + mouseRealDelta.y,
            transform.position.z + mouseRealDelta.z
            );

        fixBodiesPosition();
    }

    private void fixBodiesPosition()
    {
        foreach (GameObject cBody in GameObjectListCache.CelestialBodies)
        {
            CelestialBodyScript bodyScript = cBody.GetComponent<CelestialBodyScript>();
            cBody.transform.position = metersToUnits(bodyScript.metricPosition);
            bodyScript.calculateScale();
            bodyScript.calculateLineRendererPositions();
        }
    }

    public float unitsToMeters(float units)
    {
        return units * unitsToMetersMultiplier;
    }
    public Vector3 unitsToMeters(Vector3 units)
    {
        return new Vector3(
            units.x * unitsToMetersMultiplier,
            units.y * unitsToMetersMultiplier,
            units.z * unitsToMetersMultiplier
        );
    }


    public float metersToUnits(float meters)
    {
        return meters / unitsToMetersMultiplier;
    }
    public Vector3 metersToUnits(Vector3 meters)
    {
        return new Vector3(
            meters.x / unitsToMetersMultiplier,
            meters.y / unitsToMetersMultiplier,
            meters.z / unitsToMetersMultiplier
        );
    }
}
