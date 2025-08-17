using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    public float moveSpeed = 3f; // Movement speed
    public SpriteRenderer spriteRenderer; // Assign in Inspector if needed

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Find player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            Vector2 currentPosition = rb.position;
            Vector2 targetPosition = playerTransform.position;
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);

            // Flip sprite based on movement direction
            float direction = newPosition.x - currentPosition.x;
            if (Mathf.Abs(direction) > 0.01f && spriteRenderer != null)
            {
                spriteRenderer.flipX = direction < 0;
            }

            rb.MovePosition(newPosition);
        }
    }
}
