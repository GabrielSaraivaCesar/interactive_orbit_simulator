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
    private SpriteRenderer spriteRenderer;

    private float positionsLogTimer = 0.0f;
    private float positionsLogUpdateRate = 0.1f;
    private int positionsLogLimit = 200;
    private List<Vector3> positionsLog = new List<Vector3>();
    
    private LineRenderer lineRenderer;
    private float lineRendererDefaultWidth;

    public GameObject SelectedIndicator;
    public GameObject DirectionArrow;
    public GameObject DirectionArrowAxis;
    public Renderer DirectionArrowAxisRenderer;
    public BoxCollider2D DirectionArrowBoxCollider;
    private CelestialBodyArrowIndicator celestialBodyArrowIndicator;

    public bool isSelected;

    // Start is called before the first frame update
    void Start()
    {
        GameObjectListCache.addCelestialBodyToCacheList(gameObject); // Provide caching

        mainCam = Camera.main;
        cameraBehaviour = mainCam.GetComponent<CameraBehaviour>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        spriteRenderer.color = GameObjectListCache.getRandomCelestialBodyColor();
        Color darkerColor = Color.Lerp(spriteRenderer.color, Color.black, 0.2f);
        lineRenderer.endColor = darkerColor;
        lineRenderer.startColor = mainCam.backgroundColor;
        lineRendererDefaultWidth = lineRenderer.endWidth;


        SelectedIndicator.SetActive(false);
        DirectionArrowAxisRenderer = DirectionArrow.GetComponent<Renderer>();
        DirectionArrowBoxCollider = DirectionArrow.GetComponent<BoxCollider2D>();
        DirectionArrowAxisRenderer.enabled = false;
        DirectionArrowBoxCollider.enabled = false;

        celestialBodyArrowIndicator = DirectionArrow.GetComponent<CelestialBodyArrowIndicator>();
        calculateMetricPosition();
        calculateScale();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isManuallyMoving && !PhysicsUtils.isPaused)
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
            if (isSelected)
            {
                celestialBodyArrowIndicator.updateRotation(velocity);
                UIBodySelection.updateSelectedCelestialBodySpeed();
            }


            positionsLogTimer += Time.deltaTime;
            if (positionsLogTimer >= positionsLogUpdateRate)
            {
                positionsLog.Add(metricPosition);
                if (positionsLog.Count > positionsLogLimit)
                {
                    positionsLog.RemoveAt(0);
                }
                lineRenderer.positionCount = positionsLog.Count;
                calculateLineRendererPositions();
                positionsLogTimer = 0.0f;
            }
        }

        if (isSelected && Input.GetKeyDown(KeyCode.Delete))
        {
            UIBodySelection.clearBodySelection();
            GameObjectListCache.removeCelestialBodyFromCacheList(gameObject);
            Destroy(gameObject);
        }
    }

    public void calculateLineRendererPositions()
    {
        Vector3[] array = new Vector3[positionsLog.Count];
        for (int i = 0; i < positionsLog.Count; i++)
        {
            // Perform the transformation here; in this case, adding 1
            array[i] = cameraBehaviour.metersToUnits(positionsLog[i]);
            array[i].z = 10f;
        }

        lineRenderer.SetPositions(array);
    }

    public void calculateMetricPosition()
    {
        metricPosition = cameraBehaviour.unitsToMeters(transform.position);
    }

    public void calculateScale()
    {
        float ScaleFactor = 109 / (PhysicsUtils.sunMass / PhysicsUtils.earthMass);
        float scale = (mass / PhysicsUtils.earthMass) * ScaleFactor;

        // Constrain scale to be within the specified range
        if (scale < 0.2)
        {
            scale = 0.2f;
        }
        else if (scale > 109)
        {
            scale = 109;
        }

        diameterInMeters = (12_742_000.0f) * scale;
        float newScale = diameterInMeters / cameraBehaviour.unitsToMetersMultiplier;
        

        
        if (newScale <= 0.2f) { 
            newScale = 0.2f;
        }
        gameObject.transform.localScale = new Vector3(newScale, newScale);
        lineRenderer.endWidth = newScale * lineRendererDefaultWidth;
    }

    private void accelerateToBody(CelestialBodyScript targetBody)
    {
        Vector2 acceleration = PhysicsUtils.getGravityAcceleration(this, targetBody);
        acceleration.x = acceleration.x * Time.deltaTime * PhysicsUtils.timeWarp;
        acceleration.y = acceleration.y * Time.deltaTime * PhysicsUtils.timeWarp;

        velocity.x -= acceleration.x;
        velocity.y -= acceleration.y;
    }

    private void moveToByCurrentVelocity()
    {
        metricPosition.x += velocity.x * Time.deltaTime * PhysicsUtils.timeWarp;
        metricPosition.y += velocity.y * Time.deltaTime * PhysicsUtils.timeWarp;
        transform.position = cameraBehaviour.metersToUnits(metricPosition);
    }
}
