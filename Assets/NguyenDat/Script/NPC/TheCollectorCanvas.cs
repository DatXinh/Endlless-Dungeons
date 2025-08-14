using UnityEngine;

public class TheCollectorCanvas : MonoBehaviour
{
    public Canvas canvas; // Canvas to be hidden
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void HideCanvas()
    {
        if (canvas != null)
        {
            canvas.enabled = false; // Hide the canvas
        }
    }
}
