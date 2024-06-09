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

    public GameObject accelerationText;
    public GameObject[] accelerationButtons;
    public Sprite selectedTimeWarpSprite;
    public Sprite unselectedTimeWarpSprite;

    public GameObject celestialBodyInputsContainer;
    public TMP_InputField massInput;
    public TMP_Dropdown massUnitInput;
    public TMP_InputField speedInput;
    public TMP_Dropdown speedUnitInput;

    // Start is called before the first frame update
    void Start()
    {
        onPlayPauseChange(); // Initiate display

        //FPSIndicator.setUp(fpsText);
        UIUnitsConverter.setUp(mainCamera);
        UIBodySelection.setUp(massInput, massUnitInput, speedInput, speedUnitInput, celestialBodyInputsContainer);
    }

    // Update is called once per frame
    void Update()
    {
        //FPSIndicator.updateFPS();

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
        } else if (mouseDownUITarget.name == "accl_1")
        {
            int newTimeWarp = 1;
            updateTimeWarpImages(0);
            updateTimeWarpValue(newTimeWarp);
        }
        else if (mouseDownUITarget.name == "accl_2")
        {
            int newTimeWarp = 4;
            updateTimeWarpImages(1);
            updateTimeWarpValue(newTimeWarp);
        }
        else if (mouseDownUITarget.name == "accl_3")
        {
            int newTimeWarp = 16;
            updateTimeWarpImages(2);
            updateTimeWarpValue(newTimeWarp);
        }
        else if (mouseDownUITarget.name == "accl_4")
        {
            int newTimeWarp = 256;
            updateTimeWarpImages(3);
            updateTimeWarpValue(newTimeWarp);
        }
        else if (mouseDownUITarget.name == "accl_5")
        {
            int newTimeWarp = 1000;
            updateTimeWarpImages(4);
            updateTimeWarpValue(newTimeWarp);
        }
        else if (mouseDownUITarget.name == "accl_6")
        {
            int newTimeWarp = 2000;
            updateTimeWarpImages(5);
            updateTimeWarpValue(newTimeWarp);
        }
        else if (mouseDownUITarget.name == "accl_7")
        {
            int newTimeWarp = 10_000;
            updateTimeWarpImages(6);
            updateTimeWarpValue(newTimeWarp);
        }
        else if (mouseDownUITarget.name == "accl_8")
        {
            int newTimeWarp = 30_000;
            updateTimeWarpImages(7);
            updateTimeWarpValue(newTimeWarp);
        }
        else if (mouseDownUITarget.name == "accl_9")
        {
            int newTimeWarp = 60_000;
            updateTimeWarpImages(8);
            updateTimeWarpValue(newTimeWarp);
        }
    }

    private void updateTimeWarpValue(int value)
    {
        accelerationText.GetComponent<TextMeshProUGUI>().text = value.ToString()+"x";
        PhysicsUtils.timeWarp = value;
    } 
    private void updateTimeWarpImages(int selectedIndex)
    {
        int i = 0;
        foreach (GameObject o in accelerationButtons)
        {
            Image img = o.GetComponent<Image>();
            if (i <= selectedIndex)
            {
                img.sprite = selectedTimeWarpSprite;
            } else
            {
                img.sprite = unselectedTimeWarpSprite;
            }
            i++;
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
