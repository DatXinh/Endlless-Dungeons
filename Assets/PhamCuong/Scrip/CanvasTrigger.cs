using UnityEngine;
using UnityEngine.UI;

public class CanvasTrigger : MonoBehaviour
{
    public GameObject canvasUI; // Gán Canvas ở đây từ Inspector

    private void Start()
    {
        if (canvasUI != null)
            canvasUI.SetActive(false); // Ẩn canvas khi bắt đầu
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Player phải có tag là "Player"
        {
            canvasUI.SetActive(true); // Bật Canvas lên
        }
    }

    public void CloseCanvas()
    {
        canvasUI.SetActive(false); // Tắt canvas khi nhấn nút
    }
}
