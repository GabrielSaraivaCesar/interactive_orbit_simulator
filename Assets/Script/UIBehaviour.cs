using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;


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

    public GameObject playIcon;
    public GameObject pauseIcon;

    private float bodyPreSelectionTime;
    private GameObject bodyPreSelection;
    private GameObject selectedBody;
    private CelestialBodyScript selectedBodyScript;

    private float lastClickTime;

    // Start is called before the first frame update
    void Start()
    {
        onPlayPauseChange(); // Initiate display
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && lastClickTime == -1)
        {
            lastClickTime = Time.time;
        }

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

            if (bodyPreSelection == null)
            {
                RaycastHit2D[] hits = raycastMousePos();

                // Check if the raycast hit anything
                if (hits.Length > 0)
                {
                    if (selectedBody != null) // If theres already a selected item
                    {
                        // Find the selected item inside the hits
                        int selectedIndex = -1;
                        int index = 0;
                        foreach (RaycastHit2D hit in hits)
                        {
                            if (hit.collider.gameObject == selectedBody)
                            {
                                selectedIndex = index;
                                break;
                            }
                            index++;
                        }

                        // Selected item was clicked again, so let's check for more clicked items to switch between hit items
                        if (selectedIndex != -1 && hits.Length > 1)
                        {
                            int nextSelectedIndex = selectedIndex + 1;
                            if (nextSelectedIndex > hits.Length - 1)
                            {
                                nextSelectedIndex = 0;
                            }
                            preSelectBody(hits[nextSelectedIndex].collider.gameObject);
                        } else
                        { // Selected item was not clicked again
                            preSelectBody(hits[0].collider.gameObject);
                        }
                    } else
                    {
                        preSelectBody(hits[0].collider.gameObject);
                    }
                }
            }
        } else if (dragPositionLastFrame != null) // Stopped dragging
        {
            dragPositionLastFrame = null;
        }


        if (!Input.GetKey(KeyCode.Mouse0) && mouseDownTarget == null && bodyPreSelection != null) // Mouse release after clicking a celestial body
        {
            RaycastHit2D[] hits = raycastMousePos();
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == bodyPreSelection && selectedBody != bodyPreSelection && Time.time - bodyPreSelectionTime < 0.15)
                {
                    selectBody(bodyPreSelection);
                    break;
                }
            }
            clearPreSelectBody();
        }

        if (!Input.GetKey(KeyCode.Mouse0) && selectedBody != null && Time.time - lastClickTime < 0.15)
        {
            RaycastHit2D[] hits = raycastMousePos();
            if (hits.Length == 0)
            {
                clearBodySelection();
            }
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

    private void preSelectBody(GameObject celestialBody)
    {
        bodyPreSelectionTime = Time.time;
        bodyPreSelection = celestialBody;
    }
    private void clearPreSelectBody()
    {
        bodyPreSelection = null;
    }
    private void selectBody(GameObject celestialBody)
    {
        if (selectedBody != null)
        {
            clearBodySelection();
        }
        selectedBody = celestialBody;
        selectedBodyScript = celestialBody.GetComponent<CelestialBodyScript>();
        selectedBodyScript.SelectedIndicator.SetActive(true);
        selectedBodyScript.SelectedIndicator.GetComponent<CelestialBodyIndicatorScript>().runAnimation();
        bodyPreSelectionTime = 0;
        selectedBody.GetComponent<SpriteRenderer>().sortingOrder = 3;
        selectedBodyScript.SelectedIndicator.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }
    private void clearBodySelection()
    {
        selectedBody.GetComponent<SpriteRenderer>().sortingOrder = 1;
        selectedBodyScript.SelectedIndicator.GetComponent<SpriteRenderer>().sortingOrder = 0;
        selectedBodyScript.SelectedIndicator.SetActive(false);
        selectedBodyScript = null;
        selectedBody = null;
    }


    private RaycastHit2D[] raycastMousePos()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
        return hits;
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
        } else if (mouseDownTarget.name == "PlayPauseButton")
        {
            PhysicsUtils.isPaused = !PhysicsUtils.isPaused;
            onPlayPauseChange();
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

    private void onPlayPauseChange()
    {
        playIcon.SetActive(PhysicsUtils.isPaused);
        pauseIcon.SetActive(!PhysicsUtils.isPaused);
    }
}
