using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadedUpwardMovement : MonoBehaviour
{
    public float targetOpacity = 0.5f; // Target opacity to reach (0 = fully transparent, 1 = fully opaque)
    public float fadeDuration = 2.0f; // Duration of the fade effect in seconds
    public float moveSpeed = 100f;
    SpriteRenderer spriteRenderer;
    Rigidbody rb;
    private float currentOpacity;
    private float fadeSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the starting opacity
        currentOpacity = spriteRenderer.color.a;
        // Calculate fade speed based on fade duration
        fadeSpeed = (targetOpacity - currentOpacity) / fadeDuration;
        Debug.Log("fade speed: " + fadeSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = Vector3.up * moveSpeed * Time.deltaTime;

        // Gradually decrease opacity towards target opacity
        currentOpacity += fadeSpeed * Time.deltaTime;

        // Ensure opacity stays within bounds
        currentOpacity = Mathf.Clamp01(currentOpacity);

        // Update the sprite's color with the new opacity
        Color newColor = spriteRenderer.color;
        newColor.a = currentOpacity;
        spriteRenderer.color = newColor;

        // If opacity reaches target opacity, stop updating
        if (Mathf.Approximately(currentOpacity, targetOpacity))
        {
            enabled = false;
        }

    }
}
