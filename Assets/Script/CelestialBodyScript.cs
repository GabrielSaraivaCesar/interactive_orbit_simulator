using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyScript : MonoBehaviour
{
    
    public Vector3 metricPosition;
    public Vector2 velocity = Vector2.zero;
    public float diameterInMeters = 12_742_000.0f/2;
    public float mass = 5.9722e24f;

    private Camera mainCam;
    private CameraBehaviour cameraBehaviour;
    

    public bool isManuallyMoving = true;

    // Start is called before the first frame update
    void Start()
    {
        GameObjectListCache.addCelestialBodyToCacheList(gameObject); // Provide caching

        mainCam = Camera.main;
        cameraBehaviour = mainCam.GetComponent<CameraBehaviour>();

        calculateMetricPosition();
        calculateScale();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isManuallyMoving)
        {
            foreach (GameObject cBody in GameObjectListCache.CelestialBodies)
            {
                if (cBody == gameObject) continue; // Ignore itself
                CelestialBodyScript bodyScript = cBody.GetComponent<CelestialBodyScript>();
                if (bodyScript.isManuallyMoving)
                {
                    continue;
                }
                accelerateToBody(bodyScript);
            }
            moveToByCurrentVelocity();
        }
    }

    public void calculateMetricPosition()
    {
        metricPosition = cameraBehaviour.unitsToMeters(transform.position);
    }

    public void calculateScale()
    {
        float newScale = diameterInMeters / cameraBehaviour.unitsToMetersMultiplier;
        gameObject.transform.localScale = new Vector3(newScale, newScale);
    }

    private void accelerateToBody(CelestialBodyScript targetBody)
    {
        Vector2 acceleration = PhysicsUtils.getGravityAcceleration(this, targetBody);
        acceleration.x = acceleration.x * Time.deltaTime * UIBehaviour.timeWarp;
        acceleration.y = acceleration.y * Time.deltaTime * UIBehaviour.timeWarp;

        velocity.x -= acceleration.x;
        velocity.y -= acceleration.y;
    }

    private void moveToByCurrentVelocity()
    {
        metricPosition.x += velocity.x * Time.deltaTime * UIBehaviour.timeWarp;
        metricPosition.y += velocity.y * Time.deltaTime * UIBehaviour.timeWarp;
        transform.position = cameraBehaviour.metersToUnits(metricPosition);
    }
}
