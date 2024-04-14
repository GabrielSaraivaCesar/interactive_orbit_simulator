using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;
using TMPro;


public class UIBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Camera mainCamera;
    private GameObject mouseDownUITarget;

    public GameObject celestialBodyPrefab;
    private GameObject celestialBodyPreview;

    public GameObject celestialBodyUIContainer;
    public GameObject dialogueBalloonPrefab;

    private GameObject dialogueBalloon;
    private bool isHoveringCelestialBodyUIContainer = false;

    public GameObject playIcon;
    public GameObject pauseIcon;

    private float lastClickTime;

    public GameObject fpsText;

    // Start is called before the first frame update
    void Start()
    {
        onPlayPauseChange(); // Initiate display

        FPSIndicator.setUp(fpsText);
        UIUnitsConverter.setUp(mainCamera);
    }

    // Update is called once per frame
    void Update()
    {
        FPSIndicator.updateFPS();

        if (Input.GetKey(KeyCode.Mouse0) && lastClickTime == -1)
        {
            lastClickTime = Time.time;
        }

        // Move the celestial body preview around
        if (mouseDownUITarget != null && celestialBodyPreview != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = UIUnitsConverter.UiPosToWorldPos(mousePosition);

            celestialBodyPreview.transform.position = worldPosition;
        }

        if (Input.GetKey(KeyCode.Mouse0) && mouseDownUITarget == null && celestialBodyPreview == null) // Clicking in the game world, not on UI elements
        {
            UIBodySelection.onWorldClick();
            CameraDrag.onWorldClick();
            CelestialBodyArrowIndicator.onWorldClick();

        } else  // Stopped dragging
        {
            CameraDrag.stopDragging();
        }
        if (!Input.GetKey(KeyCode.Mouse0) && mouseDownUITarget == null && lastClickTime != -1) // Mouse release after clicking a celestial body
        {
            UIBodySelection.onWorldClickRelease();
            CelestialBodyArrowIndicator.onWorldRelease();
        }

        if (!Input.GetKey(KeyCode.Mouse0) && lastClickTime != -1)
        {
            lastClickTime = -1;
        }
    }

    private void loadTestSubjects()
    {
        int x = 0;
        int y = 0;
        for (float i = 0; i < 225; i++)
        {
            if (i % Math.Sqrt(225) == 0)
            {
                y += 1;
                x = 0;
            }
            else
            {
                x += 1;
            }
            Vector3 pos = Vector3.zero;
            pos.x = x + mainCamera.transform.position.x;
            pos.y = y + mainCamera.transform.position.y;

            GameObject cBody = Instantiate(celestialBodyPrefab, pos, Quaternion.identity);
            CelestialBodyScript script = cBody.GetComponent<CelestialBodyScript>();
            script.isManuallyMoving = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDownUITarget = eventData.pointerCurrentRaycast.gameObject;

        if (mouseDownUITarget.name == "AddBodyContainer")
        {
            mouseDownUITarget.SetActive(false);
            celestialBodyPreview = Instantiate(celestialBodyPrefab, UIUnitsConverter.UiPosToWorldPos(eventData.position), Quaternion.identity);
            CelestialBodyScript cBodyScript = celestialBodyPreview.GetComponent<CelestialBodyScript>();
            cBodyScript.isManuallyMoving = true;
        } else if (mouseDownUITarget.name == "PlayPauseButton")
        {
            PhysicsUtils.isPaused = !PhysicsUtils.isPaused;
            onPlayPauseChange();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == mouseDownUITarget)
        {
            // Simple click handlers

        } else
        {
            // Drag handlers
            if (mouseDownUITarget.name == "AddBodyContainer")
            {
                // Drop celestial body
                Vector3 mousePosition = eventData.position;
                Vector3 worldPosition = UIUnitsConverter.UiPosToWorldPos(mousePosition);
                celestialBodyPreview.transform.position = worldPosition;

                CelestialBodyScript cBodyScript = celestialBodyPreview.GetComponent<CelestialBodyScript>();
                cBodyScript.calculateMetricPosition();
                cBodyScript.isManuallyMoving = false;
                mouseDownUITarget.SetActive(true);
            }
        }

        mouseDownUITarget = null;
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

    private void onPlayPauseChange()
    {
        playIcon.SetActive(PhysicsUtils.isPaused);
        pauseIcon.SetActive(!PhysicsUtils.isPaused);
    }
}
