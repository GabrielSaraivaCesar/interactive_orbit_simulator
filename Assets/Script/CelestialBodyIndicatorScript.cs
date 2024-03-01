using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyIndicatorScript : MonoBehaviour
{
    public Sprite[] sprites; // Assign this array in the Inspector with your sprites
    public float animationSpeed = 0.04f; // Time in seconds between frames
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void runAnimation()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(AnimateSprite());
    }

    IEnumerator AnimateSprite()
    {
        int index = 0;
        while (true) // Loop to keep the animation going
        {
            if (sprites.Length == 0) yield break; // Exit if no sprites are assigned

            spriteRenderer.sprite = sprites[index]; // Set the sprite to the current index
            index = (index + 1) % sprites.Length; // Move to the next sprite, loop back to 0 at the end
            yield return new WaitForSeconds(animationSpeed); // Wait for the specified time before continuing
        }
    }
}
