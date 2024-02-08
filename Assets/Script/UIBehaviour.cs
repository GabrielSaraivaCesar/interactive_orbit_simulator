using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;


public class UIBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Camera mainCamera;
    private GameObject mouseDownTarget;

    public GameObject celestialBodyPrefab;
    private GameObject celestialBodyPreview;

    public GameObject celestialBodyUIContainer;
    public GameObject dialogueBalloonPrefab;

    private GameObject dialogueBalloon;
    private bool isHoveringCelestialBodyUIContainer = false;

    private Vector3? dragPositionLastFrame = null;

    public static int timeWarp = 1000;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseDownTarget != null && celestialBodyPreview != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = UiPosToWorldPos(mousePosition);

            celestialBodyPreview.transform.position = worldPosition;
        }

        

        if (Input.GetKey(KeyCode.Mouse0) && mouseDownTarget == null && celestialBodyPreview == null)
        {
            Vector3 currentPos = UiPosToWorldPos(Input.mousePosition);
            if (dragPositionLastFrame != null)
            {
                Vector3 lastPositionW = UiPosToWorldPos(dragPositionLastFrame ?? Vector3.zero);
                Vector3 positionDiff = new Vector3(
                        currentPos.x - lastPositionW.x,
                        currentPos.y - lastPositionW.y,
                        currentPos.z - lastPositionW.z
                );

                float deltaTime = Time.deltaTime;
                mainCamera.transform.position = new Vector3(
                    mainCamera.transform.position.x - positionDiff.x,
                    mainCamera.transform.position.y - positionDiff.y,
                    mainCamera.transform.position.z - positionDiff.z
                    );
            }
            dragPositionLastFrame = Input.mousePosition;
        } else if (dragPositionLastFrame != null) // Stopped dragging
        {
            dragPositionLastFrame = null;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDownTarget = eventData.pointerCurrentRaycast.gameObject;

        if (mouseDownTarget.name == "AddBodyContainer")
        {
            mouseDownTarget.SetActive(false);
            celestialBodyPreview = Instantiate(celestialBodyPrefab, UiPosToWorldPos(eventData.position), Quaternion.identity);
            CelestialBodyScript cBodyScript = celestialBodyPreview.GetComponent<CelestialBodyScript>();
            cBodyScript.isManuallyMoving = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == mouseDownTarget)
        {
            // Simple click handlers

        } else
        {
            // Drag handlers
            if (mouseDownTarget.name == "AddBodyContainer")
            {
                // Drop celestial body
                Vector3 mousePosition = eventData.position;
                Vector3 worldPosition = UiPosToWorldPos(mousePosition);
                celestialBodyPreview.transform.position = worldPosition;

                CelestialBodyScript cBodyScript = celestialBodyPreview.GetComponent<CelestialBodyScript>();
                cBodyScript.calculateMetricPosition();
                cBodyScript.isManuallyMoving = false;
                mouseDownTarget.SetActive(true);
            }
        }

        mouseDownTarget = null;
        celestialBodyPreview = null;
    }

    public void OnPointerEnter(PointerEventData eventData) // Hover start
    {

        if (eventData.pointerCurrentRaycast.gameObject.name == "AddBodyContainer")
        {
            isHoveringCelestialBodyUIContainer = true;
            createCelestialBodyContainerTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData) // Hover end
    {
        if (isHoveringCelestialBodyUIContainer)
        {
            destroyCelestialBodyContainerTooltip();
            isHoveringCelestialBodyUIContainer = false;
        }
    }

    public Vector3 UiPosToWorldPos(Vector3 uiPosition)
    {
        return UiPosToWorldPos(new Vector2(uiPosition.x, uiPosition.y));
    }
    public Vector3 UiPosToWorldPos(Vector2 uiPosition)
    {
        Vector3 screenPoint = new Vector3(uiPosition.x, uiPosition.y, mainCamera.nearClipPlane);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPoint);

        // In a 2D world, we ignore the Z component
        worldPosition.z = 0;

        return worldPosition;
    }

    private void createCelestialBodyContainerTooltip()
    {
        dialogueBalloon = Instantiate(dialogueBalloonPrefab, new Vector3(celestialBodyUIContainer.transform.position.x + 90, celestialBodyUIContainer.transform.position.y + 120, celestialBodyUIContainer.transform.position.z), Quaternion.identity);
        dialogueBalloon.transform.SetParent(gameObject.transform);

        BalloonScript balloonScript = dialogueBalloon.GetComponent<BalloonScript>();
        balloonScript.textValue = "Clique e arraste para criar um novo objeto";
    }

    private void destroyCelestialBodyContainerTooltip()
    {
        Destroy(dialogueBalloon);
    }
}
