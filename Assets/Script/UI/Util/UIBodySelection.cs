using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIBodySelection
{
    private static Vector3? _dragPositionLastFrame = null;
    private static float _bodyPreSelectionTime;
    private static GameObject _bodyPreSelection;
    private static GameObject _selectedBody;
    private static CelestialBodyScript _selectedBodyScript;
    private static float _lastClickTime = -1;

    public static void onWorldClick()
    {
        _lastClickTime = Time.time;
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

        if (_bodyPreSelection == null)
        {
            RaycastHit2D[] worldHits = UIUnitsConverter.raycastMousePos();

            // Check if the raycast hit anything
            if (worldHits.Length > 0)
            {
                if (_selectedBody != null) // If theres already a selected item
                {
                    // Find the selected item inside the hits
                    int selectedIndex = -1;
                    int index = 0;
                    foreach (RaycastHit2D hit in worldHits)
                    {
                        if (hit.collider.gameObject == _selectedBody)
                        {
                            selectedIndex = index;
                            break;
                        }
                        index++;
                    }

                    // Selected item was clicked again, so let's check for more clicked items to switch between hit items
                    if (selectedIndex != -1 && worldHits.Length > 1)
                    {
                        int nextSelectedIndex = selectedIndex + 1;
                        if (nextSelectedIndex > worldHits.Length - 1)
                        {
                            nextSelectedIndex = 0;
                        }
                        _preSelectBody(worldHits[nextSelectedIndex].collider.gameObject);
                    }
                    else
                    { // Selected item was not clicked again
                        _preSelectBody(worldHits[0].collider.gameObject);
                    }
                }
                else
                {
                    _preSelectBody(worldHits[0].collider.gameObject);
                }
            }
        }
    }

    public static void onWorldClickRelease()
    {
        if (_selectedBody != null && Time.time - _lastClickTime < 0.15)
        {
            RaycastHit2D[] worldHits = UIUnitsConverter.raycastMousePos();
            if (worldHits.Length == 0)
            {
                _clearBodySelection();
            }
            return;
        }
        if (_bodyPreSelection == null) return; // No body is being selected, stop method execution

        RaycastHit2D[] hits = UIUnitsConverter.raycastMousePos();
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == _bodyPreSelection && _selectedBody != _bodyPreSelection && Time.time - _bodyPreSelectionTime < 0.15)
            {
                _selectBody(_bodyPreSelection);
                break;
            }
        }
        _clearPreSelectBody();
        _lastClickTime = -1;
    }

    public static void stopDragging()
    {
        if (_dragPositionLastFrame != null)
        {
            _dragPositionLastFrame = null;
        }
    }

    private static void _preSelectBody(GameObject celestialBody)
    {
        _bodyPreSelectionTime = Time.time;
        _bodyPreSelection = celestialBody;
    }

    private static void _clearPreSelectBody()
    {
        _bodyPreSelection = null;
    }
    private static void _selectBody(GameObject celestialBody)
    {
        if (_selectedBody != null)
        {
            _clearBodySelection();
        }
        _selectedBody = celestialBody;
        _selectedBodyScript = celestialBody.GetComponent<CelestialBodyScript>();
        _selectedBodyScript.SelectedIndicator.SetActive(true);
        _selectedBodyScript.SelectedIndicator.GetComponent<CelestialBodyIndicatorScript>().runAnimation();
        _bodyPreSelectionTime = 0;
        _selectedBody.GetComponent<SpriteRenderer>().sortingOrder = 3;
        _selectedBodyScript.SelectedIndicator.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    private static void _clearBodySelection()
    {
        _selectedBody.GetComponent<SpriteRenderer>().sortingOrder = 1;
        _selectedBodyScript.SelectedIndicator.GetComponent<SpriteRenderer>().sortingOrder = 0;
        _selectedBodyScript.SelectedIndicator.SetActive(false);
        _selectedBodyScript = null;
        _selectedBody = null;
    }

}
