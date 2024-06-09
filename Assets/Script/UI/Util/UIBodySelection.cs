using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

public static class UIBodySelection
{
    private static float _bodyPreSelectionTime;
    private static GameObject _bodyPreSelection;
    private static GameObject _selectedBody;
    private static CelestialBodyScript _selectedBodyScript;
    private static float _lastClickTime = -1;

    private static TMP_InputField _massInput;
    private static TMP_Dropdown _massUnitInput;
    private static TMP_InputField _speedInput;
    private static TMP_Dropdown _speedUnitInput;
    private static GameObject _celestialBodyInputsContainer;

    private static bool _shouldIgnoreWorldClicks = false;

    public static void setUp(TMP_InputField massInput, TMP_Dropdown massUnitInput, TMP_InputField speedInput, TMP_Dropdown speedUnitInput, GameObject celestialBodyInputsContainer)
    {
        _massInput = massInput;
        _massUnitInput = massUnitInput;
        _speedInput = speedInput;
        _speedUnitInput = speedUnitInput;
        _celestialBodyInputsContainer = celestialBodyInputsContainer;


        _massInput.onValueChanged.AddListener(OnChangeMassInput);
        _massUnitInput.onValueChanged.AddListener(OnChangeMassUnitInput);
        _massInput.onSelect.AddListener(onInputSelect);

        _speedInput.onSelect.AddListener(onInputSelect);
        _speedInput.onValueChanged.AddListener(OnChangeSpeedInput);
        _speedUnitInput.onValueChanged.AddListener(OnChangeSpeedUnitInput);
    }

    private static void fillCelestialBodyInputs()
    {
        OnChangeMassUnitInput(_massUnitInput.value);
        OnChangeSpeedUnitInput(_speedUnitInput.value);
    }

    private static void onInputSelect(string v)
    {
        _shouldIgnoreWorldClicks = true;
    }


    private static void OnChangeMassInput(string value)
    {
        if (float.TryParse(value, out float floatValue))
        {
            // Units: [0-Terra] [1-Lua] [2-Sol] [3-Kg]
            float kgValue = floatValue;
            if (_massUnitInput.value == 0)
            {
                kgValue = PhysicsUtils.earthMass * kgValue;
            } else if (_massUnitInput.value == 1)
            {
                kgValue = PhysicsUtils.moonMass * kgValue;
            } else if (_massUnitInput.value == 2)
            {
                kgValue = PhysicsUtils.sunMass * kgValue;
            }
            _selectedBodyScript.mass = kgValue;
            _selectedBodyScript.calculateScale();
        }
    }

    private static void OnChangeMassUnitInput(int value)
    {
        // Units: [0-Terra] [1-Lua] [2-Sol] [3-Kg]
        float unitValue = value;
        float newInputValue = _selectedBodyScript.mass;
        if (unitValue == 0)
        {
            newInputValue = newInputValue / PhysicsUtils.earthMass;
        } else if (unitValue == 1)
        {
            newInputValue = newInputValue / PhysicsUtils.moonMass;
        } else if (unitValue == 2)
        {
            newInputValue = newInputValue / PhysicsUtils.sunMass;
        }

        _massInput.text = newInputValue.ToString();
    }

    private static void OnChangeSpeedInput(string value)
    {
        if (float.TryParse(value, out float floatValue))
        {
            // Units: [0-km/h] [1-m/s]
            float speedValue = floatValue;
            Vector2 newVelocity = _selectedBodyScript.velocity;
            if (_speedUnitInput.value == 0)
            {
                if (newVelocity != Vector2.zero)
                {
                    newVelocity = newVelocity.normalized * (speedValue * 1000 / 3600);
                } else
                {
                    newVelocity.x = speedValue;
                }
            } else
            {
                if (newVelocity != Vector2.zero)
                {
                    newVelocity = newVelocity.normalized * speedValue;
                }
                else
                {
                    newVelocity.x = speedValue;
                }
            }
            _selectedBodyScript.velocity = newVelocity;
            _selectedBodyScript.DirectionArrow.SetActive(floatValue > 0);

        }
    }


    private static void OnChangeSpeedUnitInput(int value)
    {
        // Units: [0-km/h] [1-m/s]
        float unitValue = value;
        float newInputValue = _selectedBodyScript.velocity.magnitude;
        if (unitValue == 0)
        {
            newInputValue = newInputValue * 3600 / 1000;
        }

        _speedInput.text = newInputValue.ToString();
    }


    public static void updateSelectedCelestialBodySpeed()
    {
        if (!_selectedBody) return;
        OnChangeSpeedUnitInput(_speedUnitInput.value);
    }

    public static void onWorldClick()
    {
        if (_lastClickTime != -1) return; // Wait for the release to trigger the click again
        _lastClickTime = Time.time;

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
                        if (hit.collider.gameObject.tag == "ArrowIndicator")
                        {
                            return; // Ignore click
                        }
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
        if (_shouldIgnoreWorldClicks || _massInput.isFocused || _speedInput.isFocused || GameObject.Find("Blocker") != null)
        {
            _shouldIgnoreWorldClicks = false;
            _lastClickTime = -1;
            return;
        }
        if (Time.time - _lastClickTime >= 0.15) // Not a valid click, considered a drag
        {
            _lastClickTime = -1;
            return;
        }
        _lastClickTime = -1;

        RaycastHit2D[] worldHits = UIUnitsConverter.raycastMousePos();
        foreach (RaycastHit2D hit in worldHits)
        {
            if (hit.collider.gameObject.tag == "ArrowIndicator") 
            {
                return;
            } 
        }

        if (worldHits.Length == 0)
        {
            if (_selectedBody != null)
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
        if (_selectedBodyScript.velocity.magnitude > 0 )
        {
            _selectedBodyScript.DirectionArrow.SetActive(true);
        } else
        {
            _selectedBodyScript.DirectionArrow.SetActive(false);
        }
        _selectedBodyScript.DirectionArrowAxisRenderer.enabled = true;
        _selectedBodyScript.DirectionArrowBoxCollider.enabled = true;
        _selectedBodyScript.isSelected = true;
        _selectedBodyScript.SelectedIndicator.GetComponent<CelestialBodyIndicatorScript>().runAnimation();
        _bodyPreSelectionTime = 0;
        _selectedBody.GetComponent<SpriteRenderer>().sortingOrder = 3;
        _selectedBodyScript.SelectedIndicator.GetComponent<SpriteRenderer>().sortingOrder = 2;
        _celestialBodyInputsContainer.SetActive(true);
        fillCelestialBodyInputs();
    }

    private static void _clearBodySelection()
    {
        _selectedBody.GetComponent<SpriteRenderer>().sortingOrder = 1;
        _selectedBodyScript.SelectedIndicator.GetComponent<SpriteRenderer>().sortingOrder = 0;
        _selectedBodyScript.SelectedIndicator.SetActive(false);
        _selectedBodyScript.DirectionArrowAxisRenderer.enabled = false;
        _selectedBodyScript.DirectionArrowBoxCollider.enabled = false;
        _selectedBodyScript.isSelected = false;
        _selectedBodyScript = null;
        _selectedBody = null;
        _celestialBodyInputsContainer.SetActive(false);
    }


}
