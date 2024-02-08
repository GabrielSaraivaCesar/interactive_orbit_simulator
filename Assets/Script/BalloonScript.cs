using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BalloonScript : MonoBehaviour
{
    // Public
    public string textValue = "Hello World";
    public float targetWidth = 200f;
    public float growthTimeInSeconds = 0.3f;
        
    // Util
    private float growthPerSecond;
    private bool growthComplete = false;
    private bool textAnimationStarted = false;
    private bool textAnimationComplete = false;

    // Prefabs
    public GameObject leftSection;
    public GameObject midSection;
    public GameObject rightSection;
    public GameObject textObject;

    // Prefabs Components
    private RectTransform midRectTransform;
    private TextMeshProUGUI textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        midRectTransform = midSection.GetComponent<RectTransform>();
        textMeshPro = textObject.GetComponent<TextMeshProUGUI>();

        textMeshPro.text = textValue;
        growthPerSecond = targetWidth / growthTimeInSeconds;
    }

    // Update is called once per frame
    void Update()
    {

        if (!growthComplete)
        {
            calculateGrowth();
        }
        if (textAnimationStarted && !textAnimationComplete)
        {
            calculateTextOpacity();
        }

    }

    private void calculateGrowth()
    {
        float delta = Time.deltaTime;
        float amountToGrow = delta * growthPerSecond;
        float currentWidth = midRectTransform.rect.width + amountToGrow;

        if (currentWidth > targetWidth) { currentWidth = targetWidth; }
        midRectTransform.sizeDelta = new Vector2(currentWidth, midRectTransform.rect.height);

        Vector3 rightSectionNewPos = new Vector3(midSection.transform.localPosition.x + midRectTransform.rect.width, rightSection.transform.localPosition.y, 0);
        rightSection.transform.localPosition = rightSectionNewPos;

        if (midRectTransform.rect.width >= targetWidth)
        {
            growthComplete = true;
            textAnimationStarted = true;
        }
    }

    private void calculateTextOpacity()
    {
        float delta = Time.deltaTime;

        float currentAlpha = textMeshPro.color.a;
        currentAlpha += delta * 5;

        if (currentAlpha >= 1) {
            currentAlpha = 1;
            textAnimationComplete = true;
        }

        Color textColor = textMeshPro.color;
        textColor.a = currentAlpha;
        textMeshPro.color = textColor;

    }
}
