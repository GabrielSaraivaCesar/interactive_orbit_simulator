using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyScript : MonoBehaviour
{
    
    public Vector3 metricPosition;
    public float diameterInMeters = 12_742_000.0f;
    public float mass = 5.9722e24f;

    private Camera mainCam;
    private CameraBehaviour cameraBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        cameraBehaviour = mainCam.GetComponent<CameraBehaviour>();
        calculateMetricPosition
            ();
        calculateScale();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
