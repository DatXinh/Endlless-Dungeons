using UnityEngine;

public class CanvasToggle : MonoBehaviour
{
    public GameObject[] canvases;
    private void Start()
    {
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }
    }
    // Hiện 1 canvas theo index
    public void ShowCanvas(int index)
    {
        if (index >= 0 && index < canvases.Length)
        {
            canvases[index].SetActive(true);
            CheckPauseState();
        }
    }

    // Tắt 1 canvas theo index
    public void HideCanvas(int index)
    {
        if (index >= 0 && index < canvases.Length)
        {
            canvases[index].SetActive(false);
            CheckPauseState();
        }
    }

    // Ẩn tất cả canvas
    public void HideAllCanvases()
    {
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }
        CheckPauseState();
    }

    // Kiểm tra xem có canvas nào đang bật không => dừng/thả thời gian
    private void CheckPauseState()
    {
        bool anyActive = false;

        foreach (GameObject canvas in canvases)
        {
            if (canvas.activeSelf)
            {
                anyActive = true;
                break;
            }
        }

        Time.timeScale = anyActive ? 0f : 1f;
    }
}
