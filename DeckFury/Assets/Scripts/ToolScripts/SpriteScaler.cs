using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteScaler : MonoBehaviour
{
    public Vector2 desiredSize = new Vector2(1, 1); // Set the desired on-screen size here

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;

        // Calculate the scale factor
        float scaleX = desiredSize.x / sprite.bounds.size.x;
        float scaleY = desiredSize.y / sprite.bounds.size.y;

        // Apply the scale
        transform.localScale = new Vector3(scaleX, scaleY, 1);
    }
}