using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GuideImageController : MonoBehaviour
{
    [Header("UI Components")]
    public Image guideImage; // Image component hiển thị ảnh
    public GameObject guideCanvas; // Canvas chứa hướng dẫn

    [Header("Buttons")]
    public Button nextButton;
    public Button previousButton;
    public Button closeButton;

    [Header("Image List")]
    public List<Sprite> guideSprites = new List<Sprite>(); // Danh sách ảnh

    private int currentIndex = 0;

    void Start()
    {
        // Đăng ký sự kiện nút
        nextButton.onClick.AddListener(ShowNextImage);
        previousButton.onClick.AddListener(ShowPreviousImage);
        closeButton.onClick.AddListener(CloseGuide);

        // Hiển thị ảnh đầu tiên nếu có
        if (guideSprites.Count > 0)
        {
            guideImage.sprite = guideSprites[0];
        }
    }

    void ShowNextImage()
    {
        if (guideSprites.Count == 0) return;

        currentIndex = (currentIndex + 1) % guideSprites.Count;
        guideImage.sprite = guideSprites[currentIndex];
    }

    void ShowPreviousImage()
    {
        if (guideSprites.Count == 0) return;

        currentIndex = (currentIndex - 1 + guideSprites.Count) % guideSprites.Count;
        guideImage.sprite = guideSprites[currentIndex];
    }

    void CloseGuide()
    {
        guideCanvas.SetActive(false);
        Time.timeScale = 1; // Resume game nếu trước đó tạm dừng
    }

    // Gọi hàm này nếu muốn mở lại hướng dẫn từ nơi khác
    public void OpenGuide()
    {
        guideCanvas.SetActive(true);
        currentIndex = 0;
        guideImage.sprite = guideSprites[currentIndex];
        Time.timeScale = 0; // Dừng game nếu cần
    }
}
