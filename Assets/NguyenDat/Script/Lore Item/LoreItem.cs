using UnityEngine;

public class LoreItem : MonoBehaviour
{
    public Canvas Canvas;
    private void Start()
    {
        Canvas.enabled = false; // Disable the canvas at the start
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Canvas.enabled = true; // Enable the canvas when the player enters the trigger
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Canvas.enabled = false; // Disable the canvas when the player exits the trigger
        }
    }
}
