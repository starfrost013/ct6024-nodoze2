using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Draw some sprites onto the screen in a basic sequence. Avoids overhead of the canvas system 
/// </summary>
class AnimatedSpriteSet : MonoBehaviour
{
    // Publics

    public List<Sprite> frames = new();

    public long timeBetweenFramesMs;

    private SpriteRenderer spriteRenderer;

    private Timer timer;

    private int currentId; 

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (!spriteRenderer)
        {
            Debug.LogWarning("AnimatedSpriteSet - no SpriteRenderer component, creating one...");
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // lol
        timer.Start(timeBetweenFramesMs);
    }

    private void FixedUpdate()
    {
        if (timer.GetElapsedTime() > (timeBetweenFramesMs * (currentId + 1)))
        {
            currentId++;

            if (currentId >= frames.Count)
                currentId = 0;
        }
    }
}